import { createStore, applyMiddleware, compose } from "redux";
import thunk from "redux-thunk";
import { createLogger } from "redux-logger";
//import api from "../middleware/api";
import createReducer from "./reducers";

// const composeEnhancers =
//   typeof window === 'object' &&
//     window.__REDUX_DEVTOOLS_EXTENSION_COMPOSE__ ?
//     window.__REDUX_DEVTOOLS_EXTENSION_COMPOSE__({
//       // Specify extension’s options like name, actionsBlacklist, actionsCreators, serialize...
//     }) : compose;


const composeEnhancers = window.__REDUX_DEVTOOLS_EXTENSION_COMPOSE__ || compose;

const store = createStore(
	createReducer(),
	//preloadedState,
	composeEnhancers(
		applyMiddleware(thunk, createLogger({
			predicate: () => process.env.NODE_ENV !== 'production'
		}))
	)
);

store.asyncReducers = {};

export const injectReducer = (key, reducer) => {
	if (store.asyncReducers[key]) {
		return false;
	}
	store.asyncReducers[key] = reducer;
	store.replaceReducer(createReducer(store.asyncReducers));
	return store;
};

// if (module.hot) {
//   // Enable Webpack hot module replacement for reducers
//   module.hot.accept("../reducers", () => {
//     store.replaceReducer(reducers);
//   });
// }


export default store;
