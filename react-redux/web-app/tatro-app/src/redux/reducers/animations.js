import { PLAY_ANIMATION, STOP_ANIMATION, SET_DELAY, SET_COLOR, SET_ANIMATION } from "../actionTypes";
import { SHOWS, SHOW_SPEED } from '../../Utility/constants';

const initialState = {
  animationIsPlaying: false,
  currentAnimationPlaying: null,
  delay: SHOW_SPEED.NORMAL,
  color: { rgb: { r: 0, g: 127, b: 255 }, hsv: { h: 210 } },
  animation: SHOWS.RANDOM
};

export default function (state = initialState, action) {
  switch (action.type) {
    case PLAY_ANIMATION: {
      const { animation } = action.payload;
      return {
        ...state,
        animationIsPlaying: true,
        currentAnimationPlaying: animation,
      };
    }
    case STOP_ANIMATION: {
      return {
        ...state,
        animationIsPlaying: false,
        currentAnimationPlaying: null,
      };
    }
    case SET_DELAY: {
      const { delay } = action.payload;
      return {
        ...state,
        delay: delay
      };
    }
    case SET_COLOR: {
      const { color } = action.payload;
      return {
        ...state,
        color: color
      };
    }
    case SET_ANIMATION: {
      const { animation } = action.payload;
      return {
        ...state,
        animation: animation
      };
    }
    default:
      return state;
  }
}