import {Component, EventEmitter, Input, OnInit, Output} from '@angular/core';

import {IActivityItemChange} from './interfaces';
import {IActivityPlanTemplate} from '../interfaces';

@Component({
  selector: 'app-activity-item',
  templateUrl: 'activity-item.component.html',
  styleUrls: ['activity-item.component.css']
})
export class ActivityItemComponent implements OnInit {

  @Output('change') changeNotification = new EventEmitter<IActivityItemChange>();
  @Output('delete') deleteNotification = new EventEmitter<any>();
  @Output('clearUp') clearUpNotification = new EventEmitter<void>();

  @Input('index') index = 0;
  @Input('localId') localId = String(Math.floor(Math.random() * Number.MAX_SAFE_INTEGER));

  @Input('isRoot') isRoot = true;
  @Input('readonly') readonly = false;
  @Input('reportingMode') reportingMode = false;
  @Input('readonlyReporting') readonlyReporting = false;

  @Input('activity') activity: any = {
    schedules: [],
    activityPlanDetails: [],
    children: []
  };

  @Input('progressVariables') progressVariables: any[] = [];
  @Input('progressVariableTypes') progressVariableTypes: any[] = [];
  @Input('statusTypes') statusTypes: any[] = [];
  @Input('variableValueLists') variableValueLists: any[] = [];
  @Input('activityTags') activityTags: string[] = [];
  @Input('activityPlanDetailTags') activityPlanDetailTags: string[] = [];
  @Input('activityPlanTemplates') activityPlanTemplates: IActivityPlanTemplate[] = [];

  // create inputs
  addFromTemplate: 'YES' | 'NO' = 'YES';
  selectedTemplateId = '';
  create_name = '';
  create_description = '';
  create_weight = '1';
  create_tag = '';
  create_scheduleStart = '';
  create_scheduleEnd = '';
  create_varTargets: any = {};
  create_varWeights: any = {};
  create_varNames: any = {};
  create_varTags: any = {};

  create_newVar = '';
  create_newVarName = '';
  create_newVarTarget = '';
  create_newVarWeight = '1';
  create_newVarTag = '';

  // edit inputs
  edit_name = '';
  edit_description = '';
  edit_weight = '1';
  edit_tag = '';
  edit_scheduleStart = '';
  edit_scheduleEnd = '';
  edit_varTargets: any = {};
  edit_varWeights: any = {};
  edit_varNames: any = {};
  edit_varTags: any = {};

  edit_newVar = '';
  edit_newVarName = '';
  edit_newVarTarget = '';
  edit_newVarWeight = '1';
  edit_newVarTag = '';

  constructor() {
  }

  ngOnInit(): void {
    this.updateEditFields(true); // todo: study behavior of true
  }


  onChange(e?: any): void {
    if (e && e.activity && e.index != undefined) {
      this.activity.children.splice(e.index, 1, e.activity);
    }

    this.changeNotification.emit({activity: this.activity, index: this.index});
  }


  getProgressVariable(id: number): any | null {
    return this.progressVariables.find(value => value.id == id) || null;
  }

  getVariableValueList(variableId: number): any[] | null {
    const ret = this.variableValueLists.filter(value => value.variableId == variableId);
    return ret.length ? ret : null;
  }

  getVariableValueItem(variableId: number, value: number): any | null {
    const list = this.getVariableValueList(variableId);
    return !!list && list.find(item => item.value == value) || null;
  }

  getActivityPlanTemplate(id: string): IActivityPlanTemplate | null {
    return this.activityPlanTemplates.find(value => value.id == id) || null;
  }


  get create_valuedProgressVariableTypes(): any[] {
    return this._calculateValuedProgressVariableTypes(this.create_varTargets, this.create_varWeights, this.create_varNames, this.create_varTags);
  }

  get edit_valuedProgressVariableTypes(): any[] {
    return this._calculateValuedProgressVariableTypes(this.edit_varTargets, this.edit_varWeights, this.edit_varNames, this.edit_varTags);
  }

  private _calculateValuedProgressVariableTypes(targets: any, weights: any, customVariableNames: any, tags: any): any[] {
    const ret: any = [];
    const details = this.extractDetails(targets, weights, customVariableNames, tags);
    for (const detail of details) {
      const variable = this.getProgressVariable(detail.variableId);
      if (!variable || !variable.typeId || !variable.type) {
        continue;
      }

      if (!ret.find((oldType: any) => oldType.id == variable.typeId)) {
        ret.push(variable.type);
      }
    }
    return ret;
  }

  updateEditFields(newChange = true): void {
    try {
      this.edit_name = this.activity.name;
      this.edit_description = this.activity.description;
      this.edit_weight = this.activity.weight;
      this.edit_tag = this.activity.tag;

      if (this.activity.schedules && this.activity.schedules.length) {
        this.edit_scheduleStart = new Date(this.activity.schedules[0].from).toISOString().slice(0, 10);
        this.edit_scheduleEnd = new Date(this.activity.schedules[0].to).toISOString().slice(0, 10);
      }

      if (this.activity.activityPlanDetails) {
        for (const detail of this.activity.activityPlanDetails) {
          this.edit_varTargets[String(detail.variableId)] = detail.target;
          this.edit_varWeights[String(detail.variableId)] = detail.weight;
          this.edit_varNames[String(detail.variableId)] = detail.customVariableName;
          this.edit_varTags[String(detail.variableId)] = detail.tag;
        }
      }

      if (newChange) {
        this.checkForClearUp();
      }
    } catch (e) {
      console.error(e);
    }
  }

  checkForClearUp() {
    if (this.activity.schedules.length || this.activity.activityPlanDetails.length) {
      this.emitClearUp();
    }
  }


  removeDetail(targets: any, weights: any, variableId: string): void {
    targets[String(variableId)] = undefined;
    delete targets[String(variableId)];

    weights[String(variableId)] = undefined;
    delete weights[String(variableId)];
  }


  change_create_newVar() {
    const progressVariable = this.progressVariables.find(progressVariable =>
      !!this.create_newVar && progressVariable.id == this.create_newVar);

    this.create_newVarName = progressVariable ? progressVariable.name : '';
    this.create_newVarTarget = '';
    this.create_newVarWeight = '1';
    this.create_newVarTag = ''
  }

  change_edit_newVar() {
    const progressVariable = this.progressVariables.find(progressVariable =>
      !!this.edit_newVar && progressVariable.id == this.edit_newVar);

    this.edit_newVarName = progressVariable ? progressVariable.name : '';
    this.edit_newVarTarget = '';
    this.edit_newVarWeight = '1';
    this.edit_newVarTag = '';
  }


  add_create_newVar() {
    this.create_varTargets[String(this.create_newVar)] = this.create_newVarTarget;
    this.create_varWeights[String(this.create_newVar)] = this.create_newVarWeight;
    this.create_varNames[String(this.create_newVar)] = this.create_newVarName;
    this.create_varTags[String(this.create_newVar)] = this.create_newVarTag;

    this.create_newVar = '';
    this.create_newVarName = '';
    this.create_newVarTarget = '';
    this.create_newVarWeight = '1';
    this.create_newVarTag = '';
  }

  add_edit_newVar() {
    this.edit_varTargets[String(this.edit_newVar)] = this.edit_newVarTarget;
    this.edit_varWeights[String(this.edit_newVar)] = this.edit_newVarWeight;
    this.edit_varNames[String(this.edit_newVar)] = this.edit_newVarName;
    this.edit_varTags[String(this.edit_newVar)] = this.edit_newVarTag;

    this.edit_newVar = '';
    this.edit_newVarName = '';
    this.edit_newVarTarget = '';
    this.edit_newVarWeight = '1';
    this.edit_newVarTag = '';
  }


  create(): void {
    let newChildActivity;
    if (this.addFromTemplate == 'YES') {
      const template = this.getActivityPlanTemplate(this.selectedTemplateId);
      const data = template.data;
      if (template.id && data) {
        newChildActivity = {
          name: data.name,
          description: data.description,
          weight: data.weight,
          schedules: data.schedules && [].concat(data.schedules),
          activityPlanDetails: data.activityPlanDetails && [].concat(data.activityPlanDetails),
          children: data.children && [].concat(data.children)
        };
        this.attachTemplateIdTo(newChildActivity, template.id);
      }
    } else {
      const schedules: any[] = [];
      if (this.create_scheduleStart && this.create_scheduleEnd) {
        schedules.push({
          from: new Date(this.create_scheduleStart).getTime(),
          to: new Date(this.create_scheduleEnd).getTime(),
        });
      }
      newChildActivity = {
        name: this.create_name,
        description: this.create_description,
        weight: this.create_weight,
        tag: this.create_tag ? this.create_tag.replace(/ /g, '') : undefined,
        schedules,
        activityPlanDetails: this.extractDetails(this.create_varTargets, this.create_varWeights, this.create_varNames, this.create_varTags),
        children: []
      };
    }
    if (!newChildActivity) {
      return;
    }

    this.activity.children.push(newChildActivity);

    this.selectedTemplateId = '';
    this.create_name = '';
    this.create_description = '';
    this.create_weight = '1';
    this.create_tag = '';
    this.create_scheduleStart = '';
    this.create_scheduleEnd = '';
    this.create_varTargets = {};
    this.create_varWeights = {};

    this.updateEditFields();
    this.onChange();
    this.checkForClearUp();
  }

  private attachTemplateIdTo(activity: any, templateId: string): void {
    if (!activity) { return; }
    activity.templateId = templateId;
    for (const childActivity of activity.children) { this.attachTemplateIdTo(childActivity, templateId); }
  }

  edit(): void {
    const schedules: any[] = [];
    if (this.edit_scheduleStart && this.edit_scheduleEnd) {
      schedules.push({
        from: new Date(this.edit_scheduleStart).getTime(),
        to: new Date(this.edit_scheduleEnd).getTime(),
      });
    }

    this.activity = {
      id: this.activity && this.activity.id || null,
      name: this.edit_name,
      description: this.edit_description,
      weight: this.edit_weight,
      tag: this.edit_tag ? this.edit_tag.replace(/ /g, '') : undefined,
      schedules,
      activityPlanDetails: this.extractDetails(this.edit_varTargets, this.edit_varWeights, this.edit_varNames, this.edit_varTags),
      children: this.activity.children
    };

    this.updateEditFields();
    this.onChange();
    this.checkForClearUp();
  }

  delete(): void {
    this.deleteNotification.emit({index: this.index});

    this.updateEditFields();
  }

  deleteChild(index: number): void {
    this.activity.children.splice(index, 1);

    this.updateEditFields();
    this.onChange();
  }

  emitClearUp(): void {
    this.clearUpNotification.emit();

    this.onChange();
  }

  clearThis(): void {
    this.activity.schedules = [];
    this.activity.activityPlanDetails = [];

    this.updateEditFields();
    this.emitClearUp();
  }


  setProgress() {
    this.updateEditFields();
    this.onChange();
  }


  extractDetails(targets: any, weights: any, customVariableNames: any, tags: any): any[] {
    const ret = [];

    for (const key in targets) {
      if (
        targets.hasOwnProperty(key) && targets[key] != undefined &&
        weights.hasOwnProperty(key) && weights[key] != undefined
      ) {
        ret.push({
          target: Number(targets[key]),
          weight: Number(weights[key]),
          variableId: Number(key),
          customVariableName: customVariableNames[key],
          tag: tags[key] ? tags[key].replace(/ /g, '') : undefined,
        });
      }
    }

    return ret;
  }

  trackExtractedDetails(i, extractedDetails) {
    return `${extractedDetails.variableId}`;
  }

}
