import { applyMiddleware, createStore } from "redux";
import rootReducer from "./reducers";
import logger from 'redux-logger'

/*const middlewares = [];

if (process.env.NODE_ENV === `development`) {
  const { logger } = require(`redux-logger`);

  middlewares.push(logger);
}

const compose = (...fns) =>
  fns.reduceRight((prevFn, nextFn) =>
    (...args) => nextFn(prevFn(...args)),
    value => value
  );*/

  const store = createStore(
    rootReducer,
    applyMiddleware(logger)
  )
 //const store = compose(applyMiddleware(...middlewares))(createStore)(rootReducer);

 export default store;