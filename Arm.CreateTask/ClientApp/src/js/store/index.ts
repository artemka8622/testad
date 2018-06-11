import { createStore, applyMiddleware, compose, combineReducers, GenericStoreEnhancer, Store, StoreEnhancerStoreCreator, ReducersMapObject } from 'redux'
import thunk from 'redux-thunk'
import * as reducers from './reducers/index'
import { createLogger } from 'redux-logger'

const initialState = (window as any).initialReduxState as APP.ApplicationState

function configureStore(initialState?: APP.ApplicationState) {

    const windowIfDefined = typeof window === 'undefined' ? null : window as any

    const devToolsExtension = windowIfDefined && windowIfDefined.__REDUX_DEVTOOLS_EXTENSION__ as () => GenericStoreEnhancer

    const logger = createLogger()

    const middlewares = [
        thunk,
        logger,
    ]

    const createStoreWithMiddleware : any = compose(
        applyMiddleware(...middlewares),
        devToolsExtension ? devToolsExtension() : <S>(next: StoreEnhancerStoreCreator<S>) => next
    )(createStore);

    const allReducers = buildRootReducer({ ...reducers })
    const store = createStoreWithMiddleware(allReducers, initialState) as Store<APP.ApplicationState>

    // Enable Webpack hot module replacement for reducers
    if ((module as any).hot) {
        (module as any).hot.accept('./reducers', () => {

            const nextRootReducer = { ...require<typeof reducers>('./reducers/index')}

            store.replaceReducer(buildRootReducer(nextRootReducer))
        })
    }

    return store
}

function buildRootReducer(allReducers: ReducersMapObject) {
    return combineReducers<APP.ApplicationState>(Object.assign({}, allReducers))
}

const store = configureStore(initialState)

export default store