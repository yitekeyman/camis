import React, { Component } from "react"
import { View, ViewStyle, TextStyle, Alert } from "react-native"
import { Text, Button } from "../../shared"
import { spacing, color } from "../../../theme"
import { translate } from "../../../i18n"
import { SettingsStore } from "../../../app/stores/settings-store"
import { NavigationStore } from "../../../navigation/navigation-store"
import { TasksStore } from "../../../app/stores/tasks-store"

export interface SettingsStorageProps {
  settingsStore: SettingsStore
  navigationStore: NavigationStore
  tasksStore: TasksStore
  onClear: () => Promise<void>
}

export class SettingsStorage extends Component<SettingsStorageProps, {}> {
  render() {
    const { tasksStore, settingsStore } = this.props

    return (
      <View style={ROOT}>
        <Text preset="bold" style={TITLE} tx="settings.storage" />
        <Text preset="fieldLabel" style={LABEL} tx="settings.dataClearingOptionsLabel" />
        <Button
          preset="link"
          style={ACTION}
          textStyle={ACTION_TEXT}
          tx="settings.clearSettings"
          onPress={() => this._askAndDoClear(settingsStore.clearSettingsData, "Settings")}
        />
        <Button
          preset="link"
          style={ACTION}
          textStyle={ACTION_TEXT}
          tx="settings.clearTasks"
          onPress={() => this._askAndDoClear(tasksStore.clearAllTasks, "Tasks")}
        />
        <Button
          preset="link"
          style={ACTION}
          textStyle={ACTION_TEXT}
          tx="settings.clearAll"
          onPress={() => this._askAndDoClear(settingsStore.clearAllData, "All")}
        />
      </View>
    )
  }

  private _askAndDoClear = async (
    handler: () => Promise<void>,
    key: "Settings" | "Tasks" | "All",
  ): Promise<void> => {
    Alert.alert(translate(`settings.clear${key}`), translate(`common.areYouSure`), [
      {
        style: "destructive",
        text: translate("common.yesSure"),
        onPress: () => this._doClear(handler, key),
      },
      { style: "cancel", text: translate("common.cancel") },
    ])
  }

  private _doClear = async (
    handler: () => Promise<void>,
    key: "Settings" | "Tasks" | "All",
  ): Promise<void> => {
    const { settingsStore, tasksStore, onClear } = this.props

    try {
      await handler()
      await onClear()

      Alert.alert(translate("common.success"), translate("settings.clearSuccess"))

      if (key === "Tasks" || key === "All") tasksStore.setTasks([] as typeof tasksStore.tasks)
      tasksStore.refreshTasks(settingsStore).catch(() => {})
    } catch (e) {
      Alert.alert(translate("common.failed"), `${translate("common.error")}: ${e.message}`)
    }
  }
}

const ROOT: ViewStyle = {
  margin: spacing[1],
  padding: spacing[4],
  borderRadius: spacing[1],
  backgroundColor: color.background,
  elevation: 1,
}

const TITLE: TextStyle = {
  paddingBottom: spacing[1],
}

const LABEL: TextStyle = {
  marginTop: spacing[4] - spacing[1],
  marginBottom: spacing[2],
}

const ACTION: ViewStyle = {
  marginVertical: spacing[2],
  marginLeft: spacing[2],
  alignSelf: "flex-start",
}

const ACTION_TEXT: TextStyle = {
  color: color.palette.angry,
}
