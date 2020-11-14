import { combineReducers } from "redux";
import draw from "./draw";
import animations from "./animations";
import games from "./games";
import music from "./music";
import connection from "./connection";
import settings from "./settings";

export default combineReducers({ music, draw, animations, games, connection, settings });
