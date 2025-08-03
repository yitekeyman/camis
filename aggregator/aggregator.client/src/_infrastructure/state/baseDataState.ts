import { AccessibiltyType, AgroType, InvestmentType, SoilTestType, SurfaceWaterType,
 TopographyType, UsageType, WaterSourceType, MoistureSource, GroundWaterType } from '../model/accessibility_type';

export interface BaseData {
    accessibiltyType: AccessibiltyType[] | undefined;
    agroType  : AgroType[] | undefined;
    investmentType : InvestmentType[] | undefined;
    soilTestType : SoilTestType[] | undefined;
    surfaceWaterType : SurfaceWaterType[] | undefined;
    topographyType : TopographyType[] | undefined;
    usageType : UsageType[] | undefined;
    waterSourceType : WaterSourceType[] | undefined;
    moistureSource : MoistureSource[] | undefined;
    groundWaterType : GroundWaterType[] | undefined;
}
