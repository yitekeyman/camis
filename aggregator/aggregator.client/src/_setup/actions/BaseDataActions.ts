import { BaseData } from "../../_infrastructure/state/baseDataState";

export enum BaseDataActions {
  SetData = "SET",
  ClearData = "ClEAR"
}

export const baseDataActionCreators = {
    set : (data : BaseData) => ({ type : BaseDataActions.SetData, baseData : data}),
    clear : () => ({ type : BaseDataActions.ClearData })
  }