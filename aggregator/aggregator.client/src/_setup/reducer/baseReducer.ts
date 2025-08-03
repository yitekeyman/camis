import { Reducer } from 'redux';
import { BaseDataActions, baseDataActionCreators } from '../actions/BaseDataActions';
import  {BaseData} from '../../_infrastructure/state/baseDataState';

let initialState : BaseData = {
    accessibiltyType : undefined,
    agroType : undefined,
    investmentType : undefined,
    soilTestType : undefined,
    surfaceWaterType : undefined,
    topographyType : undefined, 
    usageType : undefined,
    waterSourceType : undefined,
    moistureSource : undefined,
    groundWaterType : undefined,
}

const baseReducer : Reducer<BaseData> = (
    state = initialState,
    action
) : BaseData => {
    if(action.type == BaseDataActions.SetData){
        return { ...state, 
            waterSourceType : action.baseData.waterSourceType,
            accessibiltyType : action.baseData.accessibiltyType,
            agroType : action.baseData.agroType,
            investmentType : action.baseData.investmentType,
            soilTestType : action.baseData.soilTestType,
            surfaceWaterType : action.baseData.surfaceWaterType,
            topographyType : action.baseData.topographyType,
            usageType : action.baseData.usageType,
            moistureSource : action.baseData.moistureSource,
            groundWaterType : action.baseData.groundWaterType,
        }
    }
    else if(action.type == BaseDataActions.ClearData){
        return { ...state }
    }
    return state;
}

export default baseReducer;