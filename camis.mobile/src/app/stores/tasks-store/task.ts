import { types } from "mobx-state-tree"
import { WorkflowResponse, WorkItemResponse, FarmResponse, LatLng } from "./responses"
import { ActivityPlanRequest } from "./requests"

/**
 * An Task model.
 */
export const TaskModel = types.model("TaskModel").props({
  workflow: WorkflowResponse,
  lastWorkItem: types.maybe(WorkItemResponse),
  activityPlan: types.maybe(ActivityPlanRequest),
  farm: types.maybe(FarmResponse),
  farmLandCoordinates: types.optional(types.array(LatLng), []),
})

/**
 * The TasksStore instance.
 */
export type Task = typeof TaskModel.Type

/**
 * The data of an TasksStore.
 */
export type TaskSnapshot = typeof TaskModel.SnapshotType
