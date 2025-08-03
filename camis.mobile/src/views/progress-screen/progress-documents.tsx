import React, { Component } from "react"
import { Image, TextStyle, View, ViewStyle } from "react-native"
import ImagePicker from "react-native-image-picker"
import { observer } from "mobx-react"
import { TasksStore, DocumentRequest } from "../../app/stores/tasks-store"
import { Button, Text } from "../shared"
import { spacing, color } from "../../theme"
import { translate } from "../../i18n"
import { Icon } from "react-native-elements"

export interface ProgressDocumentsProps {
  tasksStore: TasksStore
  workflowId: string
}

@observer
export class ProgressDocuments extends Component<ProgressDocumentsProps, {}> {
  render() {
    const { tasksStore, workflowId } = this.props
    const task = tasksStore.getTask(workflowId) || null
    const plan = (task && task.activityPlan) || null
    const docs = ((plan && plan.reportDocuments) || []) as (typeof DocumentRequest.Type)[]

    return (
      plan && (
        <View style={ROOT}>
          <Text preset="bold" style={TITLE} tx="progress.reportDocumentsHeader" />

          {!docs.length && <Text tx="progress.noReportDocuments" style={NO_DOCS} />}

          {docs.map((doc, i) => (
            <View key={i} style={IMAGE_VAULT}>
              <Image
                source={{
                  uri: `data:${doc.mimetype};base64,${doc.file}`,
                  height: 42,
                  width: 42,
                }}
              />
              <View style={IMAGE_DESC}>
                <Text
                  text={`${translate("progress.date")}: ${new Date(doc.date).toDateString()}`}
                  style={IMAGE_DESC_TEXT}
                />
                <Text text={`${translate("progress.refNo")}: ${doc.ref}`} style={IMAGE_DESC_TEXT} />
              </View>
              <Button
                preset="link"
                onPress={() => tasksStore.deleteReportDocument(workflowId, doc)}
              >
                <Icon name="delete" color={color.palette.angry} />
              </Button>
            </View>
          ))}

          <Button
            preset="link"
            tx="progress.addPhoto"
            onPress={this._addPhoto}
            style={ADD_PHOTO}
            textStyle={ADD_PHOTO_TEXT}
          />
        </View>
      )
    )
  }

  private _addPhoto = (): void => {
    const { tasksStore, workflowId } = this.props

    ImagePicker.showImagePicker(
      {
        cameraType: "back",
        maxHeight: 1080,
        maxWidth: 1080,
        mediaType: "photo",
        storageOptions: {
          path: "CAMIS",
          skipBackup: false,
          waitUntilSaved: true,
        },
        title: translate("progress.addPhoto"),
      },
      response => {
        if (!response.data) return

        tasksStore.addReportDocument(
          workflowId,
          DocumentRequest.create({
            date: Date.now(),
            ref: Date.now().toString(),
            mimetype: response.type,
            filename: response.fileName,
            file: response.data,
          }),
        )
      },
    )
  }
}

const FULL = { flex: 1 }

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

const NO_DOCS: TextStyle = {
  paddingTop: spacing[1],
  paddingBottom: spacing[2],
  color: color.dim,
}

const IMAGE_VAULT: ViewStyle = {
  flexDirection: "row",
  alignItems: "center",
  paddingVertical: spacing[2],
}

const IMAGE_DESC: ViewStyle = {
  ...FULL,
  paddingHorizontal: spacing[2],
}

const IMAGE_DESC_TEXT: TextStyle = {
  color: color.dim,
}

const ADD_PHOTO: ViewStyle = {
  alignItems: "flex-end",
  marginTop: spacing[2],
  paddingVertical: spacing[2],
  paddingHorizontal: spacing[0],
}

const ADD_PHOTO_TEXT: TextStyle = {
  color: color.primary,
}
