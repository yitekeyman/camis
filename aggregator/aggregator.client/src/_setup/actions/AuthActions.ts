import { UserSession } from "../../_infrastructure/model/UserSession";

export enum AuthActions {
  LoginType = "LOGIN",
  LogoutType = "LOGOUT"
}

export const AuthActionCreators = {
  login : (session : UserSession) => ({ type : AuthActions.LoginType , session : session}),
  logout : () => ({ type : AuthActions.LogoutType })
}

