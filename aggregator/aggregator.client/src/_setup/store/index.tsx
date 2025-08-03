import { createStore, applyMiddleware } from 'redux'
import thunk from 'redux-thunk'
import { compose } from 'redux'
import rootReducer from '../reducer/index'

const initialState = {}

const middleware = [thunk] //Add more middleware here

const store = createStore(
  rootReducer,
  initialState,
  compose(
    applyMiddleware(...middleware),
    window['devToolsExtension'] ? window['devToolsExtension']() : (f: any) => f
  )
)

export default store

