import React, { Component } from "react"
import { View, ViewStyle, TextStyle } from "react-native"
import { Text, TextField } from "../../shared"
import { spacing, color } from "../../../theme"
import { SettingsStore } from "../../../app/stores/settings-store"

interface SettingsConnectionProps {
  domainUrl: string

  settingsStore: SettingsStore
}

export class SettingsConnection extends Component<SettingsConnectionProps, {}> {
  render() {
    const { domainUrl, settingsStore } = this.props

    return (
      <View style={ROOT}>
        <Text preset="bold" style={TITLE} tx="settings.connection" />
        <TextField
          keyboardType="url"
          labelTx="settings.domainUrlLabel"
          placeholderTx="settings.domainUrlPlaceholder"
          defaultValue={domainUrl}
          onChangeText={text => settingsStore.saveServerOptions(text)}
        />
      </View>
    )
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
