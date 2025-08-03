import * as React from "react"
import { observer, inject } from "mobx-react"
import { ScrollView, TextStyle, View, ViewStyle } from "react-native"
import { NavigationScreenProps } from "react-navigation"
import { Header, Text } from "../shared"
import { presets as textPresets } from "../shared/text/text.presets"
import { color, spacing } from "../../theme"
import { RootStore } from "../../app/root-store"
import { EncodingActivity } from "./encoding-activity"

export interface EncodingScreenScreenProps extends NavigationScreenProps<{}> {
  rootStore?: RootStore
}

@inject("rootStore")
@observer
export class EncodingScreen extends React.Component<EncodingScreenScreenProps, {}> {
  get _workflowId(): string | null {
    const params = this.props.navigation.state.params
    return (params && params["workflowId"]) || null
  }

  render() {
    const { navigation, rootStore } = this.props
    const task = rootStore.tasksStore.getTask(this._workflowId) || null
    const plan = (task && task.activityPlan) || null
    const activity = (plan && plan.rootActivity) || null

    return (
      activity && (
        <View style={ROOT}>
          <Header
            leftIcon="arrow-back"
            onLeftPress={() => navigation.goBack()}
            headerTx="encoding.header"
            titleStyle={textPresets.header}
          />
          <ScrollView style={CONTENT} overScrollMode="always">
            <EncodingActivity
              tasksStore={rootStore.tasksStore}
              workflowId={this._workflowId}
              activity={activity}
              style={ACTIVITY}
            />
            <Text preset="secondary" tx="encoding.footer" style={CONTENT_END} />
          </ScrollView>
        </View>
      )
    )
  }
}

const FULL = { flex: 1 }

const ROOT: ViewStyle = {
  ...FULL,
  backgroundColor: color.palette.offWhite,
}

const CONTENT: ViewStyle = {
  ...FULL,
}

const ACTIVITY: ViewStyle = {
  margin: spacing[2],
  marginRight: 0,
}

const CONTENT_END: TextStyle = {
  margin: spacing[1],
  padding: spacing[4],
  paddingTop: spacing[3],
  textAlign: "center",
}
