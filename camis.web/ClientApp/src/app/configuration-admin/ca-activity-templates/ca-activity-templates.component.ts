import {Component, OnInit} from '@angular/core';
import {Subscription} from 'rxjs/Subscription';
import {ProjectApiService} from '../../_services/project-api.service';
import {IActivityPlanTemplate} from '../../_shared/project/activities/interfaces';
import dialog from '../../_shared/dialog';

@Component({
  selector: 'app-ca-activity-templates',
  templateUrl: 'ca-activity-templates.component.html',
})
export class CaActivityTemplatesComponent implements OnInit {

  loading = true;
  templates: IActivityPlanTemplate[] = [];

  create_name: string = "";
  update_names: any = {};

  constructor(private projectApi: ProjectApiService) {
  }

  ngOnInit(): void {
    dialog.loading();
    this.load();
  }

  doUpdateNames() {
    this.templates.map(template => this.update_names[template.id] = template.name);
  }


  private load(): Subscription {
    return this.projectApi.getAllActivityPlanTemplates().subscribe(templates => {
      this.templates = templates;
      this.doUpdateNames();
      this.loading = false;
      dialog.close()
    }, dialog.error);
  }

  createTemplate(): Subscription {
    if (!this.create_name) {
      return;
    }

    dialog.loading();
    this.loading = true;

    return this.projectApi.createActivityPlanTemplates({
      name: this.create_name,
      data: {
        name: this.create_name,
        description: '',
        weight: 1,
        schedules: [],
        activityPlanDetails: [],
        children: []
      }
    }).subscribe(template  => {
      this.templates.push(template);
      this.doUpdateNames();

      this.create_name = "";

      this.loading = false;
      dialog.success("Your new empty template has been created successfully.");
    }, err => {
      this.loading = false;
      dialog.error(err);
    });
  }

  updateTemplate(template: IActivityPlanTemplate): Subscription {
    dialog.loading();
    this.loading = true;

    if (template.tempActivity) {
      template.data = template.tempActivity;
      delete template.tempActivity;
    }

    return this.projectApi.updateActivityPlanTemplates(template.id, template).subscribe(template2 => {
      this.templates = this.templates.map(t => t.id != template2.id ? t : template2);
      this.doUpdateNames();

      this.loading = false;
      dialog.success("Your template has been updated successfully.");
    }, err => {
      this.loading = false;
      dialog.error(err);
    });
  }

  async deleteTemplate(template: IActivityPlanTemplate): Promise<Subscription> {
    if (!await dialog.confirm('Are you sure you want to delete this template?')) {
      return;
    }

    dialog.loading();
    this.loading = true;

    if (template.tempActivity) {
      template.data = template.tempActivity;
      delete template.tempActivity;
    }

    return this.projectApi.deleteActivityPlanTemplates(template.id).subscribe(template => {
      this.templates = this.templates.filter(t => t.id != template.id);
      this.doUpdateNames();

      this.loading = false;
      dialog.success('Your template has been deleted successfully.');
    }, err => {
      this.loading = false;
      dialog.error(err);
    });
  }
}
