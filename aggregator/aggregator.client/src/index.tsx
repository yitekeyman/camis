import React from 'react'
import ReactDOM from 'react-dom'
import { App } from './_app/App'
import { Provider } from 'react-redux';
import { ConnectedRouter } from 'connected-react-router'
import { createBrowserHistory, BrowserHistoryBuildOptions } from 'history';
import * as serviceWorker from './serviceWorker';

import { PersistGate } from 'redux-persist/integration/react'
// import { configureStore } from './_setup/store/configureStore';
import { configureStore } from './_setup/store/configureStore';
import { Router } from 'react-router';
import store2 from './_setup/store';
// import store from './_setup/store';
// Create browser history to use in the Redux store
const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');
const browHistory : BrowserHistoryBuildOptions = {
  basename : baseUrl || undefined
}

declare global {
  interface Window { initialReduxState : any }
}
const history = createBrowserHistory(browHistory);
const initialState = window.initialReduxState;
console.log(initialState);
const { store , persistor } = configureStore(history);
console.log(store.getState());


const rootElement = document.getElementById('root') as HTMLElement;

ReactDOM.render(
  <Provider store={store}>
  <PersistGate loading={null} persistor={persistor}>
  <Router history={history}>
  <App />
  </Router>
  </PersistGate>
</Provider>

  ,rootElement);

serviceWorker.unregister();