import React, { Component } from "react"
import { View, ViewStyle, TextStyle, Alert } from "react-native"
import { Text, TextField, Button } from "../../shared"
import { spacing, color } from "../../../theme"
import { translate } from "../../../i18n"
import { SettingsStore } from "../../../app/stores/settings-store"

export interface SettingsCredentialsProps {
  username: string
  password: string

  settingsStore: SettingsStore
}

interface SettingsCredentialsState {
  isTesting: boolean

  username: string
  password: string
}

export class SettingsCredentials extends Component<
  SettingsCredentialsProps,
  SettingsCredentialsState
> {
  state: SettingsCredentialsState = {
    isTesting: false,

    username: this.props.username || "",
    password: this.props.password || "",
  }

  componentDidMount() {
    this.setState({
      username: this.props.username || "",
      password: this.props.password || "",
    })
  }

  componentDidUpdate(
    prevProps: Readonly<SettingsCredentialsProps>,
    prevState: Readonly<SettingsCredentialsState>,
    prevContext: any,
  ) {
    if (prevProps.username !== this.props.username)
      this.setState({ username: this.props.username || "" })
    if (prevProps.password !== this.props.password)
      this.setState({ password: this.props.password || "" })
  }

  render() {
    const { settingsStore } = this.props
    const { isTesting, username, password } = this.state

    return (
      <View style={ROOT}>
        <Text preset="bold" style={TITLE} tx="settings.credentials" />
        <TextField
          labelTx="settings.usernameLabel"
          placeholderTx="settings.usernamePlaceholder"
          value={username}
          onChangeText={async username => {
            this.setState({ username })
            return settingsStore.saveCredentials(username, password)
          }}
        />
        <TextField
          labelTx="settings.passwordLabel"
          placeholderTx="settings.passwordPlaceholder"
          value={password}
          secureTextEntry
          onChangeText={async password => {
            this.setState({ password })
            return settingsStore.saveCredentials(username, password)
          }}
        />
        <Button
          style={BUTTON}
          tx="settings.testLogin"
          onPress={this._testLogin}
          disabled={isTesting}
        />
      </View>
    )
  }

  private _testLogin = async (): Promise<void> => {
    const { settingsStore } = this.props

    this.setState({ isTesting: true })

    let success = false
    let error = "Unknown error."
    try {
      success = await settingsStore.testLogin()
    } catch (e) {
      error = e.message
    }

    Alert.alert(
      success ? translate("common.success") : translate("common.failed"),
      success ? translate("settings.testLoginSuccess") : `${translate("common.error")}: ${error}`,
    )

    this.setState({ isTesting: false })
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

const BUTTON: ViewStyle = {
  marginTop: spacing[2],
}
