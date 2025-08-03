import { types } from "mobx-state-tree"
import { load, save, remove, clear } from "../../../lib/storage"
import * as keychain from "../../../lib/keychain"
import axios from "axios"

interface ISettingsData {
  username: string
  password: string

  domainUrl: string
}

/**
 * An SettingsStore model.
 */
export const SettingsStoreModel = types
  .model("SettingsStore")
  .props({})
  .views(self => ({
    async getFreshData(): Promise<ISettingsData> {
      const credentials = await keychain.load("settings.credentials")
      const serverOptions = await load("settings.serverOptions")

      return {
        username: (credentials && credentials.username) || "",
        password: (credentials && credentials.password) || "",

        domainUrl:
          (serverOptions &&
            serverOptions.domainUrl &&
            serverOptions.domainUrl.replace(/\/$/i, "")) ||
          "",
      }
    },
  }))
  .views(self => ({
    async getUrlPrefix(): Promise<string> {
      return (await self.getFreshData()).domainUrl + "/api/"
    },
  }))
  .actions(self => ({
    async saveCredentials(username: string, password: string): Promise<boolean> {
      const data = await self.getFreshData()

      const u = username != null ? username : data.username
      const p = password != null ? password : data.password

      if (!u || !p) return false

      return keychain.save(u, p, "settings.credentials")
    },

    async saveServerOptions(domainUrl: string): Promise<boolean> {
      const data = await self.getFreshData()
      return save("settings.serverOptions", {
        domainUrl: domainUrl != null ? domainUrl : data.domainUrl,
      })
    },

    async testLogin(): Promise<boolean> {
      const urlPrefix = await self.getUrlPrefix()
      const settings = await self.getFreshData()
      if (!urlPrefix || !settings || !settings.username || !settings.password)
        throw new Error("Some settings are missing.")

      const response = await axios.post<{ message: string }>(urlPrefix + "Admin/Login", {
        username: settings.username,
        password: settings.password,
        role: 8, // M&E Expert role
      })

      return response.data && response.data.message === "success"
    },

    async clearSettingsData(): Promise<void> {
      await Promise.all([
        keychain.reset("settings.credentials"),
        remove("settings.serverOptions"),
      ])
    },

    async clearAllData(): Promise<void> {
      await clear()
      await keychain.reset("settings.credentials")
    },
  }))

/**
 * The SettingsStore instance.
 */
export type SettingsStore = typeof SettingsStoreModel.Type

/**
 * The data of an SettingsStore.
 */
export type SettingsStoreSnapshot = typeof SettingsStoreModel.SnapshotType
