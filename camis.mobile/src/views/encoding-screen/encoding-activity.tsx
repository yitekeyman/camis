import React, { Component } from "react"
import { Picker, StyleProp, TextStyle, TouchableOpacity, View, ViewStyle } from "react-native"
import { Icon } from "react-native-elements"
import { TasksStore, ActivityVariableValueListResponse } from "../../app/stores/tasks-store"
import { ActivityRequest } from "../../app/stores/tasks-store"
import { Text, TextField } from "../shared"
import { spacing, color } from "../../theme"
import { ActivityProgressVariableResponse } from "../../app/stores/tasks-store"
import { observer } from "mobx-react"
import { translate } from "../../i18n"

export interface EncodingActivityProps {
  tasksStore: TasksStore
  workflowId: string
  activity: typeof ActivityRequest.Type
  style?: StyleProp<ViewStyle>
}

@observer
export class EncodingActivity extends Component<EncodingActivityProps, {}> {
  render() {
    const { activity, tasksStore, style, workflowId } = this.props
    const { isDetailUiOpen } = activity

    const overrideOpenStyleForROOT = isDetailUiOpen
      ? {
          marginBottom: spacing[2],
          marginRight: spacing[2],
          paddingLeft: 0,
          borderTopWidth: 0,
          borderLeftWidth: spacing[1],
          borderLeftColor: color.primary,
          backgroundColor: color.background,
          elevation: 1,
        }
      : {}

    return (
      <View style={[ROOT, style, overrideOpenStyleForROOT]}>
        <View style={CARD}>
          <TouchableOpacity onPress={() => activity.toggleDetailUi()} style={[CARD_PAD, CARD_HEAD]}>
            <Text text={activity.name} style={ACTIVITY_NAME} />
            <Icon
              name={isDetailUiOpen ? "arrow-drop-up" : "arrow-drop-down"}
              iconStyle={DROP_ICON}
            />
          </TouchableOpacity>

          {isDetailUiOpen && (
            <View style={[CARD_PAD, PAD_TOP_NONE]}>
              <Text preset="fieldLabel" tx="encoding.activityStatusLabel" />
              <Picker
                selectedValue={activity.progressStatusId}
                onValueChange={value =>
                  tasksStore.updateActivityProgressStatusId(workflowId, activity, value)
                }
              >
                <Picker.Item label={translate("common.active")} value={1} />
                <Picker.Item label={translate("common.inactive")} value={2} />
                <Picker.Item label={translate("common.complete")} value={3} />
              </Picker>

              {activity.progressStatusId !== 3 &&
                activity.activityPlanDetails.map(
                  (detail, i) =>
                    !this._getValueList(detail.variableId) ? (
                      <TextField
                        keyboardType="numeric"
                        key={i}
                        label={`${detail.customVariableName ||
                          (this._getVariable(detail.variableId) &&
                            this._getVariable(detail.variableId).name) ||
                          "?"}:`}
                        placeholder={`${translate("common.target")} = ${
                          detail.target
                        } ${this._getVariable(detail.variableId) &&
                          this._getVariable(detail.variableId).defaultUnit &&
                          this._getVariable(detail.variableId).defaultUnit.name}`}
                        defaultValue={detail.progress != null ? String(detail.progress) : null}
                        onChangeText={value =>
                          tasksStore.updateActivityPlanDetailProgress(
                            workflowId,
                            detail,
                            value === "" ? null : Number(value),
                          )
                        }
                      />
                    ) : (
                      <View key={i}>
                        <Text
                          preset="fieldLabel"
                          text={`${detail.customVariableName ||
                            (this._getVariable(detail.variableId) &&
                              this._getVariable(detail.variableId).name) ||
                            "?"}${this._getValueItem(detail.variableId, detail.target) &&
                            ` (${translate("common.target")} = ${
                              this._getValueItem(detail.variableId, detail.target).name
                            })`}:`}
                          style={LABEL}
                        />
                        <Picker
                          selectedValue={detail.progress}
                          onValueChange={value =>
                            tasksStore.updateActivityPlanDetailProgress(workflowId, detail, value)
                          }
                        >
                          <Picker.Item label={translate("common.noneSelected")} value={null} />
                          {this._getValueList(detail.variableId).map((item, i) => (
                            <Picker.Item key={i} label={item.name} value={item.value} />
                          ))}
                        </Picker>
                      </View>
                    ),
                )}
            </View>
          )}
        </View>

        {activity.progressStatusId !== 3 &&
          activity.children.map((child, i) => (
            <View key={i} style={PAD_LEFT}>
              <EncodingActivity tasksStore={tasksStore} workflowId={workflowId} activity={child} />
            </View>
          ))}
      </View>
    )
  }

  private _getVariable(variableId: number): (typeof ActivityProgressVariableResponse.Type) | null {
    const { tasksStore } = this.props
    return tasksStore.variables.find(value => value.id == variableId) || null
  }

  private _getValueList(
    variableId: number,
  ): (typeof ActivityVariableValueListResponse.Type)[] | null {
    const { tasksStore } = this.props
    const ret = tasksStore.valueLists.filter(value => value.variableId == variableId)
    return ret.length ? ret : null
  }

  private _getValueItem(
    variableId: number,
    value: number,
  ): (typeof ActivityVariableValueListResponse.Type) | null {
    const list = this._getValueList(variableId)
    return (list && list.find(item => item.value == value)) || null
  }
}

const FULL = { flex: 1 }

const ROOT: ViewStyle = {
  paddingLeft: spacing[1] / 2,
  borderLeftWidth: spacing[1] / 2,
  borderLeftColor: "rgba(70,140,220,0.75)",
  backgroundColor: "rgba(70,140,220,0.03)",
}

const CARD: ViewStyle = {
  borderTopWidth: 1,
  borderTopColor: "rgba(70,140,220,0.14)",
}

const CARD_PAD: ViewStyle = {
  paddingVertical: spacing[3],
  paddingHorizontal: spacing[4],
  paddingLeft: spacing[4] - spacing[1] / 2,
}

const CARD_HEAD: ViewStyle = {
  flexDirection: "row",
}

const ACTIVITY_NAME: TextStyle = {
  ...FULL,
  fontWeight: "bold",
  color: color.primaryDarker,
}

const DROP_ICON: TextStyle | ViewStyle = {
  color: color.dim,
  fontSize: 15,
}

const PAD_TOP_NONE: ViewStyle = {
  paddingTop: 0,
}

const LABEL: TextStyle = {
  marginTop: spacing[2],
}

const PAD_LEFT: ViewStyle = {
  paddingLeft: spacing[4],
}
