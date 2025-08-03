import { types } from "mobx-state-tree"
import axios from "axios"
import { TaskModel, Task } from "./task"
import { DocumentRequest, ActivityRequest, ActivityPlanDetailRequest } from "./requests"
import { SettingsStore } from "../settings-store"
import { camelCase } from "../../../lib/object-casing"
import { load, remove, save } from "../../../lib/storage"
import {
  ActivityProgressVariableResponse,
  ActivityVariableValueListResponse,
  WorkflowResponse,
} from "./responses"

const WORKFLOW_IDS_STORAGE_KEY = "workflowIds"

/**
 * An TasksStore model.
 */
export const TasksStoreModel = types
  .model("TasksStore")
  .props({
    // tasks
    tasks: types.optional(types.array(TaskModel), []),

    // dynamic enumerations
    variables: types.optional(types.array(ActivityProgressVariableResponse), []),
    valueLists: types.optional(types.array(ActivityVariableValueListResponse), []),
  })
  // self-only operations
  .actions(self => ({
    setTasks(tasks: typeof self.tasks): void {
      self.tasks = tasks
    },
    getTask(workflowId: string): Task | null {
      return self.tasks.find(task => task.workflow.id === workflowId) || null
    },

    setVariables(variables: typeof self.variables): void {
      self.variables = variables
    },

    setValueLists(valueLists: typeof self.valueLists): void {
      self.valueLists = valueLists
    },
  }))
  // storage operations
  .actions(self => ({
    updateTask: async (workflowId: string, task: Task): Promise<boolean> => {
      return save(`task.${workflowId}`, task)
    },

    removeTask: async (workflowId: string): Promise<void> => {
      return remove(`task.${workflowId}`)
    },
    clearAllTasks: async (): Promise<void> => {
      const workflowIds = (await load(WORKFLOW_IDS_STORAGE_KEY)) || []

      const parallel: Promise<any>[] = [save(WORKFLOW_IDS_STORAGE_KEY, [])]
      for (const workflowId of workflowIds) parallel.push(remove(`task.${workflowId}`))

      await Promise.all(parallel)

      self.setTasks([] as typeof self.tasks)
    },
  }))
  // refresh the dynamic enumerations
  .actions(self => ({
    refreshEnumerations: async (
      settingsStore: SettingsStore,
      urlPrefix?: string,
      skipLogin = false,
    ): Promise<void> => {
      if (!skipLogin && !(await settingsStore.testLogin())) throw new Error("Failed to login.")
      urlPrefix = urlPrefix || (await settingsStore.getUrlPrefix())

      // variables
      self.setVariables(
        (await axios.get(urlPrefix + "Projects/ActivityProgressVariables", {
          withCredentials: true,
        })).data,
      )

      // valueLists
      self.setValueLists(
        (await axios.get(urlPrefix + "Projects/ActivityVariableValueLists", {
          withCredentials: true,
        })).data,
      )
    },
  }))
  // refresh the tasks (and the dynamic enumerations too)
  .actions(self => ({
    refreshTasks: async (settingsStore: SettingsStore): Promise<void> => {
      if (!(await settingsStore.testLogin())) throw new Error("Failed to login.")
      const urlPrefix = await settingsStore.getUrlPrefix()

      const tasks = []

      const workflows: (typeof WorkflowResponse.Type)[] = (await axios.get(
        urlPrefix + "Workflows/UserWorkflows",
        { withCredentials: true },
      )).data
      const workflowIds: string[] = (await load(WORKFLOW_IDS_STORAGE_KEY)) || []

      // promises that can execute parallely
      const parallel: Promise<any>[] = []

      for (const workflow of workflows) {
        // skip is workflow is not in the right state
        if ([1, 2, 3].indexOf(workflow.currentState) < 0) continue

        // push & skip if found locally and the state hasn't changes
        const localTask = self.getTask(workflow.id)
        if (localTask && localTask.workflow.currentState === workflow.currentState) {
          tasks.push(localTask)
          continue
        }

        parallel.push(
          new Promise<void>(async (resolve, reject) => {
            try {
              // last work_item
              const lastWorkItem = (await axios.get(
                urlPrefix + "Workflows/LastWorkItem/" + workflow.id,
                { withCredentials: true },
              )).data

              // activity_plan
              camelCase(lastWorkItem.data)
              const activityPlan = lastWorkItem.data

              // farm
              const farm =
                (activityPlan &&
                  activityPlan.rootActivityId &&
                  (await axios.get(
                    urlPrefix + "Farms/FarmByActivity/" + activityPlan.rootActivityId,
                    { withCredentials: true },
                  )).data) ||
                null
              if (farm && !farm.operator && farm.operatorId)
                farm.operator =
                  (await axios.get(urlPrefix + "Farms/FarmOperator/" + farm.operatorId, {
                    withCredentials: true,
                  })).data || farm.operator

              // farmLocations
              const farmLandCoordinates = []
              if (farm.farmLands) {
                const parallelLocationRequests = []
                for (const farmLand of farm.farmLands)
                  parallelLocationRequests.push(
                    axios.get(urlPrefix + "LandBank/GetLandCoordinate?landId=" + farmLand.landId),
                  )
                ;(await Promise.all(parallelLocationRequests)).map(resp =>
                  farmLandCoordinates.push(resp.data),
                )
              }

              // create the task
              const task = TaskModel.create({
                workflow,
                lastWorkItem,
                activityPlan,
                farm,
                farmLandCoordinates,
              })

              // storage save/update
              await self.updateTask(workflow.id, task)

              // pushes
              if (workflowIds.indexOf(workflow.id) < 0) workflowIds.push(workflow.id)
              tasks.push(task)

              resolve()
            } catch (e) {
              reject(e)
            }
          }),
        )
      }

      parallel.push(self.refreshEnumerations(settingsStore, urlPrefix, true))

      // execute parallel promises
      await Promise.all(parallel)

      await save(WORKFLOW_IDS_STORAGE_KEY, workflowIds)
      self.setTasks(tasks as typeof self.tasks)
    },
  }))
  // progress report documents
  .actions(self => ({
    addReportDocument: async (
      workflowId: string,
      doc: typeof DocumentRequest.Type,
    ): Promise<void> => {
      const task = self.getTask(workflowId)
      if (!task.activityPlan.reportDocuments) task.activityPlan.reportDocuments = [] as any
      task.activityPlan.reportDocuments.push(doc)
      await self.updateTask(workflowId, task)
    },
    deleteReportDocument: async (
      workflowId: string,
      doc: typeof DocumentRequest.Type,
    ): Promise<void> => {
      const task = self.getTask(workflowId)
      if (!task.activityPlan.reportDocuments) task.activityPlan.reportDocuments = [] as any

      const index = task.activityPlan.reportDocuments.findIndex(d => d.ref === doc.ref)
      if (index < 0) return
      task.activityPlan.reportDocuments.splice(index, 1)

      await self.updateTask(workflowId, task)
    },
  }))
  // progress report options
  .actions(self => ({
    updateReportStatusId: async (workflowId: string, value: number): Promise<void> => {
      const task = self.getTask(workflowId)
      task.activityPlan.reportStatusId = value
      await self.updateTask(workflowId, task)
    },
    updateReportIsAdditional: async (workflowId: string, value: boolean): Promise<void> => {
      const task = self.getTask(workflowId)
      task.activityPlan.isAdditional = value
      await self.updateTask(workflowId, task)
    },
    updateReportDate: async (workflowId: string, value: number): Promise<void> => {
      const task = self.getTask(workflowId)
      task.activityPlan.reportDate = value
      await self.updateTask(workflowId, task)
    },
    updateReportNote: async (workflowId: string, value: string): Promise<void> => {
      const task = self.getTask(workflowId)
      task.activityPlan.reportNote = value
      await self.updateTask(workflowId, task)
    },
  }))
  // progress report encoding
  .actions(self => ({
    updateActivityProgressStatusId: async (
      workflowId: string,
      activity: typeof ActivityRequest.Type,
      value: number,
    ): Promise<void> => {
      activity.setProgressStatusId(value)
      const task = self.getTask(workflowId) || null
      await self.updateTask(workflowId, task)
    },
    updateActivityPlanDetailProgress: async (
      workflowId: string,
      detail: typeof ActivityPlanDetailRequest.Type,
      value: number,
    ): Promise<void> => {
      detail.setProgress(value)
      const task = self.getTask(workflowId) || null
      await self.updateTask(workflowId, task)
    },
  }))
  // submit and remove task
  .actions(self => ({
    submitTask: async (workflowId: string, settingsStore: SettingsStore): Promise<void> => {
      if (!(await settingsStore.testLogin())) throw new Error("Failed to login.")
      const urlPrefix = await settingsStore.getUrlPrefix()

      const { activityPlan } = self.getTask(workflowId)
      await axios.post(urlPrefix + "Projects/SubmitProgressReport/" + workflowId, activityPlan, {
        withCredentials: true,
      })

      await self.removeTask(workflowId)
    },
  }))

/**
 * The TasksStore instance.
 */
export type TasksStore = typeof TasksStoreModel.Type

/**
 * The data of an TasksStore.
 */
export type TasksStoreSnapshot = typeof TasksStoreModel.SnapshotType
