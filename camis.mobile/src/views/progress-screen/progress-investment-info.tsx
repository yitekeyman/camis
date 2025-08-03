import React, { Component } from "react"
import { Linking, TextStyle, View, ViewStyle } from "react-native"
import { Button, Text } from "../shared"
import { spacing, color } from "../../theme"
import { TasksStore, LatLng } from "../../app/stores/tasks-store"

export interface ProgressInvestmentInfoProps {
  tasksStore: TasksStore
  workflowId: string
}

export class ProgressInvestmentInfo extends Component<ProgressInvestmentInfoProps, {}> {
  render() {
    const { tasksStore, workflowId } = this.props
    const task = tasksStore.getTask(workflowId) || null
    const plan = (task && task.activityPlan) || null
    const coordinates = (task && task.farmLandCoordinates) || null

    return (
      plan &&
      coordinates && (
        <View style={ROOT}>
          <Text preset="bold" style={TITLE} tx="progress.investmentInfoHeader" />

          <Text preset="fieldLabel" style={LABEL} tx="progress.planNoteLabel" />
          <Text text={plan.note || "?"} style={VALUE} />

          <Text preset="fieldLabel" style={LABEL} tx="progress.coordinatesLabel" />
          {!coordinates.length && <Text tx="progress.noCoordinates" style={NO_COORDINATES} />}
          {coordinates.map((coordinate, i) => (
            <Button
              key={i}
              preset="link"
              style={ACTION}
              textStyle={ACTION_TEXT}
              text={this._parseCoordinateOutput(coordinate)}
              onPress={() =>
                Linking.openURL(
                  `geo:0,0?q=${coordinate.lat},${coordinate.lng}(${
                    task.farm.operator.name
                  }'s Project)`,
                )
              }
            />
          ))}
        </View>
      )
    )
  }

  _parseCoordinateOutput({ lat, lng }: typeof LatLng.Type): string {
    return `${Math.round(lat * 100000) / 100000}° ${lat > 0 ? "N" : "S"}, ${Math.round(
      lng * 100000,
    ) / 100000}°  ${lng > 0 ? "E" : "W"}`
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

const NO_COORDINATES: TextStyle = {
  marginVertical: spacing[2],
  color: color.dim,
}

const LABEL: TextStyle = {
  marginTop: spacing[2],
}

const VALUE: TextStyle = {
  marginVertical: spacing[2],
  marginLeft: spacing[2],
}

const ACTION: ViewStyle = {
  marginVertical: spacing[2],
  marginLeft: spacing[2],
  alignSelf: "flex-start",
}

const ACTION_TEXT: TextStyle = {
  color: color.primary,
}
