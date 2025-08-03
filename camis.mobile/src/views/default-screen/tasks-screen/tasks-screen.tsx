import * as React from "react"
import { observer, inject } from "mobx-react"
import { ViewStyle, View, ScrollView, Alert, TextStyle, RefreshControl } from "react-native"
import { Icon } from "react-native-elements"
import { NavigationScreenProps } from "react-navigation"
import { Header, Text } from "../../shared"
import { presets as textPresets } from "../../shared/text/text.presets"
import { TasksTaskCard } from "./tasks-task-card"
import { RootStore } from "../../../app/root-store"
import { spacing, color } from "../../../theme"
import { translate } from "../../../i18n"

export interface TasksScreenScreenProps extends NavigationScreenProps<{}> {
  rootStore?: RootStore
}

interface TasksScreenState {
  isLoading: boolean
}

@inject("rootStore")
@observer
export class TasksScreen extends React.Component<TasksScreenScreenProps, TasksScreenState> {
  state: TasksScreenState = {
    isLoading: true,
  }

  componentDidMount() {
    this._loadTasks(false)
  }

  render() {
    const { navigation, rootStore } = this.props
    const { isLoading } = this.state

    const tasks = rootStore.tasksStore.tasks

    return (
      <View style={ROOT}>
        <Header
          leftIcon="menu"
          onLeftPress={() => navigation.toggleDrawer()}
          headerTx="tasks.header"
          titleStyle={textPresets.header}
          rightIcon="refresh"
          onRightPress={() => this._loadTasks(true)}
        />
        <ScrollView
          style={CONTENT}
          overScrollMode="always"
          refreshControl={
            <RefreshControl
              refreshing={isLoading}
              onRefresh={() => this._loadTasks(true)}
              tintColor={color.primary}
              progressBackgroundColor={color.background}
              colors={[color.primary, color.primaryDarker]}
            />
          }
        >
          {(!rootStore.tasksStore.tasks.length && (
            <View style={NO_TASKS}>
              <Icon iconStyle={NO_TASKS_ICON} name="done" />
              <Text preset="bold" style={NO_TASKS_TEXT}>
                {translate("tasks.noTasks")}
              </Text>
              <Text style={{ ...NO_TASKS_TEXT, color: color.dim }}>
                {translate("tasks.noTasksDesc")}
              </Text>
            </View>
          )) || (
            <View style={IN_CONTENT}>
              {tasks.map((task, i) => (
                <TasksTaskCard key={i} task={task} onOpen={this._openTask} />
              ))}
              <Text preset="secondary" style={CONTENT_END}>
                --- SHOWING {tasks.length} TASK{tasks.length !== 1 && "S"} ---
              </Text>
            </View>
          )}
        </ScrollView>
      </View>
    )
  }

  private _loadTasks = async (showFail = true): Promise<void> => {
    const { rootStore } = this.props

    this.setState({ isLoading: true })
    try {
      await rootStore.tasksStore.refreshTasks(rootStore.settingsStore)
    } catch (e) {
      if (showFail)
        Alert.alert(translate("common.failed"), `${translate("common.error")}: ${e.message}`)
    }

    this.setState({ isLoading: false })
  }

  private _openTask = async (workflowId: string): Promise<void> => {
    const { rootStore } = this.props

    rootStore.navigationStore.navigateTo("progressScreen", { workflowId })
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

const NO_TASKS: ViewStyle = {
  ...FULL,
  margin: spacing[2],
  padding: spacing[4],
}

const NO_TASKS_ICON: TextStyle = {
  marginVertical: spacing[4],
  padding: spacing[3],
  width: spacing[8],
  height: spacing[8],
  borderRadius: spacing[8] / 2,
  fontSize: spacing[8] - 2 * spacing[3],
  backgroundColor: color.palette.white,
  color: color.primary,
  elevation: 1,
}

const NO_TASKS_TEXT: TextStyle = {
  marginTop: spacing[2],
  textAlign: "center",
}

const IN_CONTENT: ViewStyle = {
  margin: spacing[1],
}

const CONTENT_END: TextStyle = {
  margin: spacing[1],
  padding: spacing[4],
  paddingTop: spacing[3],
  textAlign: "center",
}
