import React, { Component } from "react"
import { TextStyle, View, ViewStyle } from "react-native"
import { observer } from "mobx-react"
import { Text, Button } from "../../shared"
import { color, spacing } from "../../../theme"
import { Task } from "../../../app/stores/tasks-store"

interface TasksTaskCardProps {
  task: Task
  onOpen: (workflowId: string) => Promise<void>
}

@observer
export class TasksTaskCard extends Component<TasksTaskCardProps, {}> {
  render() {
    const { task, onOpen } = this.props
    const farm = task && task.farm
    const operator = farm && farm.operator

    return (
      operator && (
        <View style={ROOT}>
          <Button preset="link" onPress={() => onOpen(task.workflow.id)} style={BUTTON}>
            <View style={IN_BUTTON}>
              <View style={ROW}>
                <Text style={CARD_TITLE}>
                  {(operator && operator.name) || "?"} ({(operator && operator.nationality) || "?"})
                </Text>
              </View>
              <View style={ROW}>
                <Text style={LABEL}>Investment Type:</Text>
                <Text style={VALUE}>{(farm && farm.type && farm.type.name) || "?"}</Text>
              </View>
              {farm &&
                farm.registrations.map((reg, i) => (
                  <View key={i} style={ROW}>
                    <Text style={LABEL}>{(reg.type && reg.type.name) || "?"}:</Text>
                    <Text style={VALUE}>{reg.registrationNumber}</Text>
                  </View>
                ))}
              {operator &&
                operator.registrations.map((reg, i) => (
                  <View key={i} style={ROW}>
                    <Text style={LABEL}>{(reg.type && reg.type.name) || "?"}:</Text>
                    <Text style={VALUE}>{reg.registrationNumber}</Text>
                  </View>
                ))}
            </View>
          </Button>
        </View>
      )
    )
  }
}

const FULL = { flex: 1 }

const ROOT: ViewStyle = {
  margin: spacing[1],
  borderLeftColor: color.primary,
  borderLeftWidth: spacing[2],
  borderTopRightRadius: spacing[1],
  borderBottomRightRadius: spacing[1],
  backgroundColor: color.background,
  elevation: 1,
}

const BUTTON: ViewStyle = {
  alignItems: "stretch",
}

const IN_BUTTON: ViewStyle = {
  ...FULL,
  padding: spacing[4],
  paddingTop: spacing[3],
}

const ROW: ViewStyle = {
  flexDirection: "row",
  paddingTop: spacing[1],
}

const CARD_TITLE: TextStyle = {
  paddingBottom: spacing[1],
  fontSize: 17,
  fontWeight: "bold",
  color: color.primary,
}

const LABEL: TextStyle = {
  flex: 3,
  color: color.dim,
  paddingRight: spacing[4],
}

const VALUE: TextStyle = {
  flex: 4,
}
