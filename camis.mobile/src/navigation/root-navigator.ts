import { createStackNavigator } from "react-navigation"
import { ProgressScreen } from "../views/progress-screen/progress-screen"
import { EncodingScreen } from "../views/encoding-screen/encoding-screen"
import { DefaultScreen } from "../views/default-screen/default-screen"

export const RootNavigator = createStackNavigator(
  {
    defaultScreen: {
      screen: DefaultScreen,
      navigationOptions: {
        title: "CAMIS",
      },
    },
    progressScreen: {
      screen: ProgressScreen,
      navigationOptions: {
        title: "Progress",
      },
    },
    encodingScreen: {
      screen: EncodingScreen,
      navigationOptions: {
        title: "Encoding",
      },
    },
  },
  {
    headerMode: "none",
    initialRouteName: "defaultScreen",
  },
)
