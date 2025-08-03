import { Reducer } from 'redux';
import {AuthActions} from '../actions/AuthActions';
import  {AuthState} from '../../_infrastructure/state/authState';

let initialState : AuthState = {
    authenticated : false,
    session : undefined
}

const authReducer : Reducer<AuthState> = (
   state = initialState,
   action
): AuthState => {
    if(action.type == AuthActions.LoginType){
        return { ...state, authenticated : true, session : action.session };
    }
    else if(action.type == AuthActions.LogoutType){
        return { ...state, authenticated : false, session : undefined }
    }
    else{
        return state;
    }
}

export default authReducer;