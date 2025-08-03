import { combineReducers } from 'redux'
import authReducer from './authReducer';
import loadReducer from './loadReducer';
import baseReducer from './baseReducer';


const rootReducer = combineReducers({
     auth : authReducer,
    loader : loadReducer,
    base : baseReducer
    // routing : routerReducer,
})

export default rootReducer;

