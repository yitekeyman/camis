export interface LandModel {
    upins: Array<string>;
    accessablity: Array<number>;
    soil_test: Array<SoilTestType>;
    climate: Array<Climate>;
    description: string;

}
export interface Climate {
    precipitation: number;
    temp_low: number;
    temp_high: number;
    temp_avg: number;

}
export class Accessablity {
     id: number;
     name: string;
     checked: boolean;
}

export class AgroEchologicalZone {
    id: number;
    name: string;
}

export class Topography {
    id: number;
    name: string;
}


export class SoilTestType {
     id: number;
     name: string;
}

export class InvestmentType {
    id: number;
    name: string;
}

export class Month {
     id: number;
     name: string;
}

export class LandType {
     id: number;
     name: string;
}

export class MoistureSource {
    id: number;
    name: string;
}

export class WaterTestParameters {
    id: number;
    name: string;
}

export class GroundWater {
    id: number;
    name: string;
}export class SurfaceWater {
    id: number;
    name: string;
}

export class ExistingLandUse {
    id: number;
    name: string;
}

export interface LandBankWorkItem {
     wiid: String;
     wfid: String;
     workItemDate: Date;
     sentUserID: String;
     sentUserName: String;
     description: String;
     workFlowType: number;
}
export interface LandBankWorkFlowLand {
    accessablity: Array<number>;
    area: number;
    centroidX: number;
    centroidY: number;
    climate: Array<Climate>;
    description: String;
    holdership: String;
    landType: number;
    soilTests: Array<SoilTestType>;
    upins: Array<String>;
}

export interface SearchLandModel {
    upin: string;
    landType: number;
    areaMin: number;
    areaMax: number;
}

export interface LandPreparationModel {
    landID: string;
    n: number;
}

export interface ResultViewModel {
    upins: Array<String>;
    description: number;
    area: number;
    landType: number;
    landID: string;
    holdership: string;
    accessablity: Array<number>;
    centroidX: number;
    centroidY: number;
    climate: Array<Climate>;
    soilTests: Array<SoilTestType>;
    wID:string;
}
export interface SearchResult {
    result: ResultViewModel[];
}
