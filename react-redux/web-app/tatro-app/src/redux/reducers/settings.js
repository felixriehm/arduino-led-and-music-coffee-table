import { BRIGHTNESS } from "../actionTypes";

const initialState = {
    brightness: 255,
};

export default function (state = initialState, action) {
    switch (action.type) {
        case BRIGHTNESS: {
            const { brightness } = action.payload;
            return {
                ...state,
                brightness: brightness,
            };
        }
        default:
            return state;
    }
}