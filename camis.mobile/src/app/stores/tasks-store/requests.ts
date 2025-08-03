import { types } from "mobx-state-tree"

/**
 * DOCUMENT
 */

export const DocumentRequest = types.model("DocumentRequest").props({
  date: types.number,
  ref: types.string,
  note: types.maybe(types.string),
  mimetype: types.string,
  type: types.maybe(types.number),
  filename: types.maybe(types.string),
  file: types.maybe(types.string),

  // only for viewing documents from a custom url (esp. in work items)
  id: types.maybe(types.string),
  overrideFilePath: types.maybe(types.string),
})

/**
 * PROJECT
 */

export const ActivityScheduleRequest = types.model("ActivityScheduleRequest").props({
  from: types.number,
  to: types.number,
})

export const ActivityPlanDetailRequest = types
  .model("ActivityPlanDetailRequest")
  .props({
    target: types.number,
    weight: types.optional(types.number, 1),
    variableId: types.number,
    customVariableName: types.maybe(types.string),

    // only for progress reporting...
    progress: types.maybe(types.number),
  })
  .actions(self => ({
    setProgress: (value: number): void => {
      self.progress = value
    },
  }))

export let ActivityRequest = types
  .model("ActivityRequest")
  .props({
    name: types.string,
    description: types.maybe(types.string),
    weight: types.optional(types.number, 1),
    parentActivityId: types.maybe(types.string),

    schedules: types.optional(types.array(ActivityScheduleRequest), []),
    activityPlanDetails: types.optional(types.array(ActivityPlanDetailRequest), []),

    children: types.optional(types.array(types.late(() => ActivityRequest)), []),

    // only for progress reporting...
    id: types.maybe(types.string),
    progressStatusId: types.maybe(types.number),

    // only on this mobile app
    isDetailUiOpen: types.optional(types.boolean, false),
  })
  .actions(self => ({
    setProgressStatusId: (value: number): void => {
      self.progressStatusId = value
    },

    toggleDetailUi: (): void => {
      self.isDetailUiOpen = !self.isDetailUiOpen
    },
  }))

export const ActivityPlanRequest = types.model("ActivityPlanRequest").props({
  note: types.string,
  statusId: types.number,
  rootActivity: ActivityRequest,

  documents: types.optional(types.array(DocumentRequest), []),

  // only for progress reporting...
  rootActivityId: types.maybe(types.string),
  reportStatusId: types.maybe(types.number),
  reportNote: types.maybe(types.string),
  reportDate: types.maybe(types.number),
  reportDocuments: types.maybe(types.array(DocumentRequest)),
  isAdditional: types.optional(types.boolean, false),
})
