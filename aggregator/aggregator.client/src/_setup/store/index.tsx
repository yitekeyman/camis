import { createStore, applyMiddleware, compose, Middleware } from 'redux'
import thunk from 'redux-thunk'
import rootReducer from '../reducer/index'

const initialState = {}

const thunkMiddleware = ((thunk as any).default || thunk) as Middleware
const middleware: Middleware[] = [thunkMiddleware] //Add more middleware here

const store = createStore(
  rootReducer,
  initialState,
  compose(
    applyMiddleware(...middleware),
    window['devToolsExtension'] ? window['devToolsExtension']() : (f: any) => f
  )
)

export default store

