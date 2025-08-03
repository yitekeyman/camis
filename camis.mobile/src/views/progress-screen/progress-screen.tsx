import * as React from "react"
import { observer, inject } from "mobx-react"
import { Alert, ScrollView, TextStyle, View, ViewStyle } from "react-native"
import { NavigationScreenProps } from "react-navigation"
import { Button, Header, Text } from "../shared"
import { presets as textPresets } from "../shared/text/text.presets"
import { color, spacing } from "../../theme"
import { RootStore } from "../../app/root-store"
import { ProgressInvestmentInfo } from "./progress-investment-info"
import { ProgressOptions } from "./progress-options"
import { ProgressDocuments } from "./progress-documents"
import { translate } from "../../i18n"

export interface ProgressScreenScreenProps extends NavigationScreenProps<{}> {
  rootStore?: RootStore
}

@inject("rootStore")
@observer
export class ProgressScreen extends React.Component<ProgressScreenScreenProps, {}> {
  get _workflowId(): string | null {
    const params = this.props.navigation.state.params
    return (params && params["workflowId"]) || null
  }

  render() {
    const { navigation, rootStore } = this.props

    return (
      this._workflowId && (
        <View style={ROOT}>
          <Header
            leftIcon="arrow-back"
            onLeftPress={() => navigation.goBack()}
            headerTx="progress.header"
            titleStyle={textPresets.header}
            rightIcon="send"
            onRightPress={this._submitProgress}
          />
          <ScrollView style={CONTENT} overScrollMode="always">
            <Button tx="progress.encodingLink" onPress={this._goToEncoding} style={ENCODING_LINK} />
            <ProgressInvestmentInfo
              tasksStore={rootStore.tasksStore}
              workflowId={this._workflowId}
            />
            <ProgressDocuments tasksStore={rootStore.tasksStore} workflowId={this._workflowId} />
            <ProgressOptions tasksStore={rootStore.tasksStore} workflowId={this._workflowId} />
            <Text preset="secondary" tx="progress.footer" style={CONTENT_END} />
          </ScrollView>
        </View>
      )
    )
  }

  private _submitProgress = async (): Promise<void> => {
    const { navigation, rootStore } = this.props
    const { tasksStore } = rootStore

    Alert.alert(
      translate("progress.submitDialogTitle"),
      translate("progress.submitDialogMessage"),
      [
        {
          text: translate("common.yesSure"),
          style: "default",
          onPress: async () => {
            await tasksStore.submitTask(this._workflowId, rootStore.settingsStore)
            tasksStore.refreshTasks(rootStore.settingsStore)
            navigation.goBack()
          },
        },
        { text: translate("common.cancel"), style: "cancel" },
      ],
    )
  }

  private _goToEncoding = async (): Promise<void> => {
    this.props.rootStore.navigationStore.navigateTo("encodingScreen", {
      workflowId: this._workflowId,
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

const ENCODING_LINK: ViewStyle = {
  margin: spacing[1],
}

const CONTENT_END: TextStyle = {
  margin: spacing[1],
  padding: spacing[4],
  paddingTop: spacing[3],
  textAlign: "center",
}
