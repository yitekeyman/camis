import React, { Component } from "react"
import {
  DatePickerAndroid,
  Picker,
  TextStyle,
  TouchableOpacity,
  View,
  ViewStyle,
} from "react-native"
import { observer } from "mobx-react"
import { Text, TextField } from "../shared"
import { color, spacing } from "../../theme"
import { translate } from "../../i18n"
import { TasksStore } from "../../app/stores/tasks-store"

export interface ProgressOptionsProps {
  tasksStore: TasksStore
  workflowId: string
}

@observer
export class ProgressOptions extends Component<ProgressOptionsProps, {}> {
  componentDidMount() {
    const { tasksStore, workflowId } = this.props
    const task = tasksStore.getTask(workflowId) || null
    const plan = (task && task.activityPlan) || null

    if (!plan) return

    if (!plan.reportStatusId) tasksStore.updateReportStatusId(workflowId, 1)
    if (plan.isAdditional == null) tasksStore.updateReportIsAdditional(workflowId, false)
    if (!plan.reportDate) tasksStore.updateReportDate(workflowId, Date.now())
    if (!plan.reportNote) tasksStore.updateReportNote(workflowId, "")
  }

  render() {
    const { tasksStore, workflowId } = this.props
    const task = tasksStore.getTask(workflowId) || null
    const plan = (task && task.activityPlan) || null

    return (
      plan && (
        <View style={ROOT}>
          <Text preset="bold" style={TITLE} tx="progress.reportOptionsHeader" />

          <Text preset="fieldLabel" tx="progress.reportStatusLabel" style={LABEL} />
          <Picker
            selectedValue={plan.reportStatusId}
            onValueChange={value => tasksStore.updateReportStatusId(workflowId, value)}
          >
            <Picker.Item label={translate("common.active")} value={1} />
            <Picker.Item label={translate("common.inactive")} value={2} />
            <Picker.Item label={translate("common.complete")} value={3} />
          </Picker>

          <Text preset="fieldLabel" tx="progress.reportTypeLabel" style={LABEL} />
          <Picker
            selectedValue={plan.isAdditional}
            onValueChange={value => tasksStore.updateReportIsAdditional(workflowId, value)}
          >
            <Picker.Item label={translate("progress.additional")} value={true} />
            <Picker.Item label={translate("progress.cumulative")} value={false} />
          </Picker>

          <TouchableOpacity onPress={() => this._askAndUpdateReportDate(plan.reportDate)}>
            <TextField
              labelTx="progress.reportDateLabel"
              value={new Date(plan.reportDate).toDateString()}
              editable={false}
            />
          </TouchableOpacity>

          <TextField
            labelTx="progress.reportNoteLabel"
            placeholderTx="progress.reportNotePlaceholder"
            defaultValue={plan.reportNote}
            onChangeText={value => tasksStore.updateReportNote(workflowId, value)}
            multiline
          />
        </View>
      )
    )
  }

  private _askAndUpdateReportDate = async (reportDate): Promise<void> => {
    const { tasksStore, workflowId } = this.props

    const picker = await DatePickerAndroid.open({
      date: reportDate,
      maxDate: Date.now(),
    })
    if (picker.action !== DatePickerAndroid.dateSetAction) return

    return tasksStore.updateReportDate(
      workflowId,
      new Date(picker.year, picker.month, picker.day).getTime(),
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

const LABEL: TextStyle = {
  marginTop: spacing[2],
}
