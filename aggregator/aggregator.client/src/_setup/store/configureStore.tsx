import { applyMiddleware, combineReducers, createStore, AnyAction } from 'redux';
import {thunk} from 'redux-thunk';
// import { routerReducer, routerMiddleware } from 'react-router-redux';
import { routerMiddleware, } from 'connected-react-router';
import { compose } from 'redux'
import reducers from '../reducer'
import { persistStore, persistReducer, PersistConfig } from 'redux-persist'
import storage from 'redux-persist/lib/storage' // defaults to localStorage for web and AsyncStorage for react-native
import autoMergeLevel2 from 'redux-persist/es/stateReconciler/autoMergeLevel2';
import { RootState } from './MyTypes';
declare global {
    
  interface Window { devToolsExtension : any | never}
}


export function configureStore(history :any) {

  const middleware = [
    thunk,
    routerMiddleware(history)
  ];

  // In development, use the browser's Redux dev tools extension if installed
  const enhancers = [];
  const isDevelopment = process.env.NODE_ENV === 'development';
  if (isDevelopment && typeof window !== 'undefined' && window.devToolsExtension) {
    enhancers.push(window.devToolsExtension());
  }

  const persistConfig: PersistConfig<RootState> = {
    key: 'admin',
    storage,
    stateReconciler: autoMergeLevel2,
  }
  console.log(persistConfig);
  // persistReducer typing can be strict; use any for reducer state to avoid
  // incompatibilities between reducer inferred types and PersistConfig
  const persistedReducer = persistReducer<any, AnyAction>(persistConfig, reducers)

  let store = createStore(persistedReducer);
  let persistor = persistStore(store)
  return { store, persistor }

}
