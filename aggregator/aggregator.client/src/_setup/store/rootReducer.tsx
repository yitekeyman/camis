import { combineReducers } from 'redux';
import authReducer from '../reducer/authReducer';
import loadReducer from '../reducer/loadReducer';
import { routerReducer } from 'react-router-redux';
import baseReducer from '../reducer/baseReducer';

  export const rootReducer = combineReducers({

       auth : authReducer,
      loader : loadReducer,
      base : baseReducer,
      route : routerReducer
  })
