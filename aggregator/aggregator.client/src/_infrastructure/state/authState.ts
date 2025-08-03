import { UserSession } from "../model/UserSession";

export interface AuthState {
    authenticated : boolean,
    session? : UserSession
}

