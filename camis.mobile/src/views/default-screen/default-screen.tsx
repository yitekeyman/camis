import * as React from "react"
import { observer } from "mobx-react"
import { NavigationScreenProps } from "react-navigation"
import { DefaultNavigator } from "../../navigation/default-navigator"

export interface DefaultScreenScreenProps extends NavigationScreenProps<{}> {}

// @inject("mobxstuff")
@observer
export class DefaultScreen extends React.Component<DefaultScreenScreenProps, {}> {
  render() {
    return <DefaultNavigator />
  }
}
