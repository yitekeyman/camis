import * as React from "react"
import { View, ViewStyle, TextStyle } from "react-native"
import { Icon } from "react-native-elements"
import { HeaderProps } from "./header.props"
import { Button } from "../button"
import { Text } from "../text"
import { spacing, color } from "../../../theme"
import { translate } from "../../../i18n/"

// static styles
const ROOT: ViewStyle = {
  flexDirection: "row",
  alignItems: "center",
  justifyContent: "flex-start",
  backgroundColor: color.background,
  elevation: 1,
}
const SIDE_BUTTON: ViewStyle = {
  alignItems: "center",
  width: spacing[6] + 2 * spacing[3],
  height: spacing[6] + 2 * spacing[3],
}
const TITLE_CONTAINER: ViewStyle = {
  flex: 1,
  justifyContent: "center",
}
const TITLE: TextStyle = {
  textAlign: "left",
  paddingVertical: spacing[3],
}

/**
 * Header that appears on many screens. Will hold navigation buttons and screen title.
 */
export class Header extends React.Component<HeaderProps, {}> {
  render() {
    const {
      onLeftPress,
      onRightPress,
      rightIcon,
      leftIcon,
      headerText,
      headerTx,
      titleStyle,
    } = this.props
    const header = headerText || (headerTx && translate(headerTx)) || ""

    return (
      <View style={{ ...ROOT, ...this.props.style }}>
        {leftIcon ? (
          <Button preset={"link"} onPress={onLeftPress} style={SIDE_BUTTON}>
            <Icon name={leftIcon} />
          </Button>
        ) : null}
        <View style={TITLE_CONTAINER}>
          <Text style={{ ...TITLE, ...titleStyle }} text={header} />
        </View>
        {rightIcon ? (
          <Button preset={"link"} onPress={onRightPress} style={SIDE_BUTTON}>
            <Icon name={rightIcon} />
          </Button>
        ) : null}
      </View>
    )
  }
}
