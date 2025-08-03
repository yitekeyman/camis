import {Injectable} from "@angular/core";
import {Router} from "@angular/router";
import {ApiService} from "./api.services";
import {HttpClient} from "@angular/common/http";
import {Observable} from "rxjs";
import {Status} from "tslint/lib/runner";

class Pro{
    id:string;
    operatorId:string;
    typeId:number;
    activityId:string;
    investedCapital:number;
    description:string;
    registrations:any[];
    operator:any;
    type:any;
    farmLands:any[];
    
}
class Act {
    id:string;
    note:string;
    statusId:number;
    rootActivityId:string;
    status:any;
    rootActivity:any;
    documents:any[];
    calculatedProgress:number;
}

class Unit{
    id:number;
    name:string;
}

class Variable{
    id:number;
    name:string;
    defaultUnitId:number;
    typeId:number;
    defaultUnit:any;
    type:any;
}

class valueList {
    variableId:number;
    order:number;
    value:number;
    name:string;
}

class PlanTemp{
    id:string;
    name:string;
    data:any;
}

class Reports{
    id:string;
    note:string;
    reportTime:string;
    statusId:string;
    rootActivityId:string;
    reportStatus:any;
    rootActivity:any;
    reportDocuments:any[];
    activityStatuses:any[];
    variableProgresses:any[];
    calculatedProgress:number;
}

class workflow{
    id:string;
    workflowId:string;
    seqNo:number;
    fromState:number;
    data:Reports;
}

@Injectable()
export class ProjectServices {

    project='../assets/data/projects.json';
    activities='../assets/data/activities.json';
    
    statusJson='../assets/data/statusType.json';
    valueJson='../assets/data/valueList.json';
    temJson='../assets/data/planTemplate.json';
    varJson='../assets/data/mesVariable.json';
    unitJson='../assets/data/mesUnit.json';
    
    reportJson='../assets/data/reports.json';
    workflowJson='../assets/data/workflow.json';
    

    constructor(public router: Router, public apiService: ApiService , public http: HttpClient) {}

    getProject():Observable<Pro[]> {
        return this.http.get<Pro[]>(this.project);
    }
    getActivities():Observable<Act[]>{
        return this.http.get<Act[]>(this.activities);
    }
    getUnit():Observable<Unit[]>{
        return this.http.get<Unit[]>(this.unitJson);
    }
    
    getVariable():Observable<Variable[]>{
        return this.http.get<Variable[]>(this.varJson);
    }
    getValueList():Observable<valueList[]>{
        return this.http.get<valueList[]>(this.valueJson);
    }
    getPlanTemp():Observable<PlanTemp[]>{
        return this.http.get<PlanTemp[]>(this.temJson);
    }
    
    getStatus():Observable<Unit[]>{
        return this.http.get<Unit[]>(this.statusJson);
    }
    
    getReports():Observable<Reports[]>{
        return this.http.get<Reports[]>(this.reportJson);
    }
    getWorkflow():Observable<workflow[]>{
        return this.http.get<workflow[]>(this.workflowJson);
    }
    
}