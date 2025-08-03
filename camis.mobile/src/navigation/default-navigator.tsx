import React from "react"
import { ScrollView, TextStyle, View, ViewStyle } from "react-native"
import { createDrawerNavigator, SafeAreaView, DrawerItems } from "react-navigation"
import { SettingsScreen } from "../views/default-screen/settings-screen/settings-screen"
import { TasksScreen } from "../views/default-screen/tasks-screen/tasks-screen"
import { Icon } from "react-native-elements"
import { Text } from "../views/shared"
import { translate } from "../i18n"
import { color, spacing } from "../theme"

export const DefaultNavigator = createDrawerNavigator(
  {
    tasksScreen: {
      screen: TasksScreen,
      navigationOptions: {
        drawerLabel: translate("navigation.tasks"),
        drawerIcon: ({ tintColor }) => (
          <Icon name="list" containerStyle={ICON} iconStyle={{ color: tintColor }} />
        ),
      },
    },
    settingsScreen: {
      screen: SettingsScreen,
      navigationOptions: {
        drawerLabel: translate("navigation.settings"),
        drawerIcon: ({ tintColor }) => (
          <Icon name="settings" containerStyle={ICON} iconStyle={{ color: tintColor }} />
        ),
      },
    },
  },
  {
    initialRouteName: "tasksScreen",
    contentOptions: {
      activeTintColor: color.primary,
    },
    contentComponent: props => (
      <ScrollView style={FULL}>
        <SafeAreaView style={FULL} forceInset={{ top: "always", horizontal: "never" }}>
          <View style={HEADER}>
            <Text preset="header" tx="brand.camis" style={TITLE} />
            <Text tx="brand.camisLong" style={SUBTITLE} />
          </View>

          <DrawerItems {...props} />

          <View style={FOOTER}>
            <Text preset="secondary" tx="brand.copyright" />
          </View>
        </SafeAreaView>
      </ScrollView>
    ),
  },
)

const FULL = { flex: 1 }

const HEADER: ViewStyle = {
  paddingVertical: spacing[7],
  paddingHorizontal: spacing[4],
  backgroundColor: color.primaryDarker,
}

const TITLE: TextStyle = {
  color: color.palette.white,
}

const SUBTITLE: TextStyle = {
  paddingTop: spacing[2],
  color: color.palette.white,
  opacity: 0.84,
}

const ICON: ViewStyle = {
  width: spacing[5],
  height: spacing[5],
}

const FOOTER: ViewStyle = {
  padding: spacing[4],
}
