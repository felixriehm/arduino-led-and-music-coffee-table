import { PLAY_GAME, STOP_GAME, SET_GAME_SPEED, SET_GAME } from "../actionTypes";
import { GAME_SPEED, GAMES } from '../../Utility/constants';

const initialState = {
  gameIsPlaying: false,
  currentGamenPlaying: null,
  delay: GAME_SPEED.NORMAL,
  game: GAMES.SNAKE
};

export default function (state = initialState, action) {
  switch (action.type) {
    case PLAY_GAME: {
      const { game } = action.payload;
      return {
        ...state,
        gameIsPlaying: true,
        currentGamePlaying: game,
      };
    }
    case STOP_GAME: {
      return {
        ...state,
        gameIsPlaying: false,
        currentGamePlaying: null,
      };
    }
    case SET_GAME_SPEED: {
      const { delay } = action.payload;
      return {
        ...state,
        delay: delay
      };
    }
    case SET_GAME: {
      const { game } = action.payload;
      return {
        ...state,
        game: game
      };
    }
    default:
      return state;
  }
}