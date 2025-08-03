import * as React from "react"
import { inject, observer } from "mobx-react"
import { ScrollView, TextStyle, View, ViewStyle } from "react-native"
import { NavigationScreenProps } from "react-navigation"
import { RootStore } from "../../../app/root-store"
import { Header, Text } from "../../shared"
import { presets as textPresets } from "../../shared/text/text.presets"
import { SettingsCredentials } from "./settings-credentials"
import { SettingsConnection } from "./settings-connection"
import { SettingsStorage } from "./settings-storage"
import { color, spacing } from "../../../theme"

export interface SettingsScreenScreenProps extends NavigationScreenProps<{}> {
  rootStore?: RootStore
}

interface SettingsState {
  username: string
  password: string

  domainUrl: string
}

@inject("rootStore")
@observer
export class SettingsScreen extends React.Component<SettingsScreenScreenProps, SettingsState> {
  state: SettingsState = {
    username: "",
    password: "",

    domainUrl: "",
  }

  componentDidMount() {
    this._loadData()
  }

  render() {
    const { navigation, rootStore } = this.props
    const { username, password, domainUrl } = this.state

    return (
      <View style={ROOT}>
        <Header
          titleStyle={textPresets.header}
          leftIcon="menu"
          onLeftPress={navigation.toggleDrawer}
          headerTx="settings.header"
        />
        <ScrollView style={CONTENT} overScrollMode="always">
          <SettingsConnection domainUrl={domainUrl} settingsStore={rootStore.settingsStore} />
          <SettingsCredentials
            username={username}
            password={password}
            settingsStore={rootStore.settingsStore}
          />
          <SettingsStorage
            navigationStore={rootStore.navigationStore}
            settingsStore={rootStore.settingsStore}
            tasksStore={rootStore.tasksStore}
            onClear={this._loadData}
          />
          <Text preset="secondary" tx="brand.copyright" style={CONTENT_END} />
        </ScrollView>
      </View>
    )
  }

  private _loadData = async (): Promise<void> => {
    const { rootStore } = this.props

    const data = await rootStore.settingsStore.getFreshData()
    this.setState({
      username: data.username || "",
      password: data.password || "",

      domainUrl: data.domainUrl || "",
    })
  }
}

const FULL = { flex: 1 }

const ROOT: ViewStyle = {
  ...FULL,
  backgroundColor: color.palette.offWhite,
}

const CONTENT: ViewStyle = {
  ...FULL,
  padding: spacing[1],
}

const CONTENT_END: TextStyle = {
  margin: spacing[1],
  padding: spacing[4],
  paddingTop: spacing[3],
  textAlign: "center",
}
