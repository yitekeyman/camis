import {Component, OnInit} from '@angular/core';
import {ProjectApiService} from '../../../_services/project-api.service';
import {IActivityPlanTemplate} from '../../../_shared/project/activities/interfaces';
import dialog from '../../../_shared/dialog';
import {CommonModule} from "@angular/common";
import {FormControl, FormsModule, ReactiveFormsModule} from "@angular/forms";
import {ActivitiesComponent} from "../../../_shared/project/activities/activities.component";
import {Subscription} from "rxjs";
import {ObjectKeyCasingService} from "../../../_services/object-key-casing.service";
import {NgbAccordionModule} from "@ng-bootstrap/ng-bootstrap";

@Component({
  selector: 'app-ca-activity-templates',
  imports:[CommonModule, ReactiveFormsModule, ActivitiesComponent, FormsModule],
  templateUrl: 'ca-activity-templates.component.html',
  styleUrls: ['ca-activity-templates.component.scss']
})
export class CaActivityTemplatesComponent implements OnInit {

  loading = true;
  templates: IActivityPlanTemplate[] = [];

  create_name='';
  update_names: any = {};

  constructor(private projectApi: ProjectApiService, private keyCase:ObjectKeyCasingService) {
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
      this.keyCase.camelCase(templates);
      this.templates = templates;
      this.doUpdateNames();
      this.loading = false;
      dialog.close()
    }, dialog.error);
  }

  createTemplate(): Subscription|void {
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
      this.keyCase.camelCase(template);
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
      this.keyCase.camelCase(template2);
      this.templates = this.templates.map(t => t.id != template2.id ? t : template2);
      this.doUpdateNames();

      this.loading = false;
      dialog.success("Your template has been updated successfully.");
    }, err => {
      this.loading = false;
      dialog.error(err);
    });
  }

  async deleteTemplate(template: IActivityPlanTemplate): Promise<Subscription|void> {
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
      this.keyCase.camelCase(template);
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
