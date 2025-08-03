import { types } from "mobx-state-tree"
import { DocumentRequest } from "./requests"

/**
 * SYSTEM
 */

export const UserActionResponse = types.model("UserActionResponse").props({
  id: types.maybe(types.number),
  username: types.string,
  timeStr: types.string,
})

/**
 * WORKFLOW
 */

export const WorkflowTypeResponse = types.model("WorkflowTypeResponse").props({
  id: types.number,
  name: types.string,
  description: types.string,
})

export const WorkflowResponse = types.model("WorkflowResponse").props({
  id: types.string,
  currentState: types.number,
  description: types.string,
  typeId: types.number,
  aid: types.number,

  type: WorkflowTypeResponse,
  action: UserActionResponse,
})

export const WorkItemResponse = types.model("WorkItemResponse").props({
  id: types.string,
  workflowId: types.string,
  seqNo: types.number,
  fromState: types.maybe(types.number),
  toState: types.maybe(types.number),
  trigger: types.number,
  dataType: types.string,
  description: types.maybe(types.string),
  assignedRole: types.maybe(types.number),
  assignedUser: types.maybe(types.number),
  aid: types.maybe(types.number),
})

/**
 * FARM
 */

export const FarmOperatorTypeResponse = types.model("FarmOperatorTypeResponse").props({
  id: types.number,
  name: types.string,
})

export const FarmTypeResponse = types.model("FarmTypeResponse").props({
  id: types.number,
  name: types.string,
})

export const RegistrationAuthorityResponse = types.model("RegistrationAuthorityResponse").props({
  id: types.number,
  name: types.string,
})

export const RegistrationTypeResponse = types.model("RegistrationTypeResponse").props({
  id: types.number,
  name: types.string,
})

export const FarmOperatorOriginResponse = types.model("FarmOperatorOriginResponse").props({
  id: types.number,
  name: types.string,
})

export const FarmOperatorRegistrationResponse = types
  .model("FarmOperatorRegistrationResponse")
  .props({
    id: types.maybe(types.number),
    registrationNumber: types.string,
    authorityId: types.number,
    typeId: types.number,
    documentId: types.maybe(types.string),

    authority: types.maybe(RegistrationAuthorityResponse),
    type: types.maybe(RegistrationTypeResponse),
    document: types.maybe(DocumentRequest),
  })

export const FarmOperatorResponse = types.model("FarmOperatorResponse").props({
  id: types.maybe(types.string),
  name: types.string,
  nationality: types.string,
  typeId: types.number,
  addressId: types.string,
  phone: types.maybe(types.string),
  email: types.maybe(types.string),
  originId: types.number,
  capital: types.maybe(types.number),

  registrations: types.optional(types.array(FarmOperatorRegistrationResponse), []),
})

export const FarmRegistrationResponse = types.model("FarmRegistrationResponse").props({
  id: types.maybe(types.number),
  registrationNumber: types.string,
  authorityId: types.number,
  typeId: types.number,
  documentId: types.maybe(types.string),

  authority: types.maybe(RegistrationAuthorityResponse),
  type: types.maybe(RegistrationTypeResponse),
  document: types.maybe(DocumentRequest),
})

export const FarmResponse = types.model("FarmResponse").props({
  id: types.maybe(types.string),
  operatorId: types.maybe(types.string), // if it is an existing operator
  typeId: types.number,
  activityId: types.maybe(types.string),
  investedCapital: types.maybe(types.number),
  description: types.maybe(types.string),
  landId: types.maybe(types.string),

  registrations: types.optional(types.array(FarmRegistrationResponse), []),

  operator: types.maybe(FarmOperatorResponse),
  type: types.maybe(FarmTypeResponse),
})

/**
 * LAND BANK
 */

export const LatLng = types.model("LatLng").props({
  lat: types.number,
  lng: types.number,
})

/**
 * PROJECT
 */

export const ActivityProgressMeasuringUnitResponse = types
  .model("ActivityProgressMeasuringUnitResponse")
  .props({
    id: types.number,
    name: types.string,
    convertFrom: types.maybe(types.number),
    conversionFactor: types.optional(types.number, 1),
    conversionOffset: types.optional(types.number, 0),
  })

export const ActivityProgressVariableTypeResponse = types
  .model("ActivityProgressVariableTypeResponse")
  .props({
    id: types.number,
    name: types.string,
  })

export const ActivityProgressVariableResponse = types
  .model("ActivityProgressVariableResponse")
  .props({
    id: types.number,
    name: types.string,
    defaultUnitId: types.maybe(types.number),
    typeId: types.maybe(types.number),

    defaultUnit: types.maybe(ActivityProgressMeasuringUnitResponse),
    type: types.maybe(ActivityProgressVariableTypeResponse),
  })

export const ActivityVariableValueListResponse = types
  .model("ActivityVariableValueListResponse")
  .props({
    variableId: types.number,
    order: types.number,
    value: types.number,
    name: types.string,
  })

/*
 * MODELS NOT USED HERE, YET (OR EVER): ...

// NOTE: this hard-coded in this app. DISCUSS and DECIDE, if it should be dynamic.
// this is hard-coded in encoding-activity.tsx and progress-options.tsx
export const ActivityStatusTypeResponse = types.model("ActivityStatusTypeResponse").props({
  id: types.number,
  name: types.string,
})

export const ActivityScheduleResponse = types.model("ActivityScheduleResponse").props({
  id: types.maybe(types.string),
  from: types.number,
  to: types.number,
})

export const ActivityPlanDetailResponse = types.model("ActivityPlanDetailResponse").props({
  id: types.maybe(types.string),
  target: types.number,
  weight: types.optional(types.number, 1),
  variableId: types.maybe(types.number),
  customVariableName: types.maybe(types.string),

  variable: types.maybe(ActivityProgressVariableResponse),
})

export let ActivityResponse = types.model("ActivityResponse").props({
  id: types.maybe(types.string),
  name: types.string,
  description: types.maybe(types.string),
  weight: types.optional(types.number, 1),
  parentActivityId: types.maybe(types.string),

  schedules: types.optional(types.array(ActivityScheduleResponse), []),
  activityPlanDetails: types.optional(types.array(ActivityPlanDetailResponse), []),

  children: types.optional(types.array(types.late(() => ActivityResponse)), []),
})

export const DocumentResponse = types.model("DocumentResponse").props({
  id: types.maybe(types.string),
  date: types.number,
  ref: types.string,
  note: types.maybe(types.string),
  mimetype: types.string,
  type: types.maybe(types.number),
  filename: types.maybe(types.string),
})

export const ActivityPlanResponse = types.model("ActivityPlanResponse").props({
  id: types.maybe(types.string),
  note: types.string,
  statusId: types.number,
  rootActivityId: types.string,

  status: types.maybe(ActivityStatusTypeResponse),
  rootActivity: types.maybe(ActivityResponse),
  documents: types.maybe(types.array(DocumentResponse)),

  calculatedProgress: types.maybe(types.number),
})

  */
