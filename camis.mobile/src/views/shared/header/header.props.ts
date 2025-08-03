import { ViewStyle, TextStyle } from "react-native"

export interface HeaderProps {
  /**
   * Main header, e.g. POWERED BY BOWSER
   */
  headerTx?: string

  /**
   * header non-i18n
   */
  headerText?: string

  /**
   * Icon that should appear on the left (a react-native-elements Icon)
   */
  leftIcon?: string

  /**
   * What happens when you press the left icon
   */
  onLeftPress?(): void

  /**
   * Icon that should appear on the right (a react-native-elements Icon)
   */
  rightIcon?: string

  /**
   * What happens when you press the right icon
   */
  onRightPress?(): void

  /**
   * Container style overrides.
   */
  style?: ViewStyle

  /**
   * Title style overrides.
   */
  titleStyle?: TextStyle
}
