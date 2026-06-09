import React from 'react'
import ReactDOM from 'react-dom'
import { App } from './_app/App'
import { Provider } from 'react-redux';
import { ConnectedRouter } from 'connected-react-router'
import { createBrowserHistory } from 'history';
import * as serviceWorker from './serviceWorker';

import { PersistGate } from 'redux-persist/integration/react'
// import { configureStore } from './_setup/store/configureStore';
import { configureStore } from './_setup/store/configureStore';
// using ConnectedRouter from connected-react-router to provide history to redux
import store2 from './_setup/store';
// import store from './_setup/store';
// Create browser history to use in the Redux store
const baseUrl = document.getElementsByTagName('base')[0].getAttribute('href');

declare global {
  interface Window { initialReduxState : any }
}
const history = createBrowserHistory({
  basename: baseUrl || undefined
} as any);
const initialState = window.initialReduxState;
console.log(initialState);
const { store , persistor } = configureStore(history);
console.log(store.getState());


const rootElement = document.getElementById('root') as HTMLElement;

ReactDOM.render(
  <Provider store={store}>
  <PersistGate loading={null} persistor={persistor}>
  <ConnectedRouter history={history}>
    <App />
  </ConnectedRouter>
  </PersistGate>
</Provider>

  ,rootElement);

serviceWorker.unregister();