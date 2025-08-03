export interface ReportType {
    id : number;
    title : string;
}

export interface Region {
    title : string;
    value : string;
}

export enum ReportTypes {
    SummaryOfLandByLandStatus = 1,
    ListOfLands = 2,
    ListOfCommercialFarmOwners = 3,
    AnnualLeaseAmountByAdministrativeLocationInvType =4,
    LandTransferredAtDifferentTimes = 5,
    LandTransferreToInvestorsByFarmSize = 6,
    SummaryOfAgricultureInvestmentByOriginOfInvestors = 7,
    SummaryOfLandByAdministrativeLocationandWaterSource = 8

}

export enum SummerizedBy {
    Region = 1,
    Zone = 2,
    Woreda = 3
}

export enum FilteredBy {
    None = 0,
    Region = 1,
    Zone = 2,
    Woreda = 3
}




export interface ReportRequestModel
{
    selectedReportType: ReportTypes;
 
    dates : Object[] | null;
    farmSizes : Object[] | null;
    region : string | null;
    zone : string | null;
    woreda : string | null;

    summerizedBy : SummerizedBy ;
    filteredBy : FilteredBy;
    
    fromDate: Date | string | null;
    endDate: Date | string | null;

    farmId: string | null;
    startYear: number;
    endYear: number;

}

export interface ReportResponseModel
{
    nRecords: number;
    Request: ReportRequestModel;
    Farms: FarmResponse[];
    Lands: LandData[];
    FarmOperators: FarmOperatorResponse[];
}


export interface FarmResponse
{
    Id: string;
    OperatorId: string;
    TypeId: number;
    ActivityId: string; // only Id
    InvestedCapital: number | null;
    Description: string;
    OtherTypeIds: number[];

    Registrations: any[];

    Operator: FarmOperatorResponse;
    Type: FarmTypeResponse;

 
}

export interface FarmOperatorResponse
    {
        Id: string;
        Name: string;
        Nationality: string;
        TypeId: number;
        AddressId: string; // only Id
        Phone: string;
        Email: string;
        OriginId: number;
        Capital: number | null;

        // if TypeId == 1:
        Gender: string; // 'F' = Female, 'M' = Male
        MartialStatus: number | null; // 1 = Not Married, 2 = Married, 3 = Divorced, 4 = Widowed
        Birthdate: number | null; // javascript date format (in millis)
        
        // if TypeId == 6:
        Ventures: string[];


        Registrations: any[];

        Type: FarmOperatorTypeResponse;
        Origin: FarmOperatorOriginResponse;
    }

    export interface FarmOperatorTypeResponse
    {
        Id: number;
        Name: string;
    }

    export interface FarmTypeResponse
    {
        Id: number;
        Name: string;
    }

    export interface FarmOperatorOriginResponse
    {
        Id: number;
        Name: string;
    }

    export interface LandData
    {
        new(): LandData;
        LandID: string;
        WID: string;
        LandType: number;
        Accessablity: number[];
        SoilTests: any[];
        Upins: string[];
        Description: string;
        Area: number;
        CentroidX: number;
        CentroidY: number;
        Climate: any[];
        Holdership: string;
        FarmID: string;
        landHolderType: number;//1:private, 3:state land
        AgroEchologyZone: any[];
        InvestmentType: number[];
        MoistureSource: number;
        IrrigationValues: any;
        Topography: any[];
        ExistLandUse: number[];
        IsAgriculturalZone: string;

    }