import { types } from "mobx-state-tree"
import { NavigationStoreModel } from "../navigation/navigation-store"
import { SettingsStoreModel } from "./stores/settings-store"
import { TasksStoreModel } from "./stores/tasks-store"

/**
 * An RootStore model.
 */
export const RootStoreModel = types.model("RootStore").props({
  navigationStore: types.optional(NavigationStoreModel, {}),
  settingsStore: types.optional(SettingsStoreModel, {}),
  tasksStore: types.optional(TasksStoreModel, {}),
})

/**
 * The RootStore instance.
 */
export type RootStore = typeof RootStoreModel.Type

/**
 * The data of an RootStore.
 */
export type RootStoreSnapshot = typeof RootStoreModel.SnapshotType
