import * as ActionType from "./actionTypes";
import { $CombinedState } from "redux";
import $ from 'jquery';

// connection
export const connectToBluetooth = (device) => ({ type: ActionType.CONNECT, payload: { deviceName: device.name, deviceId: device.id } });

export const finilizeBluetoothConnection = () => {
    $(".alert.alert-info").slideDown(400).delay(1000).slideUp(500);
    return { type: ActionType.FINILIZE_CONNECTION }
};

export const disconnectFromBluetooth = () => {
    $(".alert.alert-info").slideDown(400).delay(1000).slideUp(500);
    return { type: ActionType.DISCONNECT }
};

export const searchBluetoothDevice = () => ({ type: ActionType.REQUEST_DEVICE });

export const changeServiceUuid = (serviceUuid) => ({ type: ActionType.CHANGE_SERVICE_UUID, payload: { serviceUuid } });

export const changeCharacteristicUuid = (characteristicUuid) => ({ type: ActionType.CHANGE_CHARACTERISTIC_UUID, payload: { characteristicUuid } });

export const changeLogOutputText = (text, type) => ({ type: ActionType.CHANGE_LOG_OUTPUT_TEXT, payload: { text, type } });

export const flushLogOutputText = () => ({ type: ActionType.FLUSH_LOG_OUTPUT_TEXT });

// settings
export const setBrightness = (brightness) => ({ type: ActionType.BRIGHTNESS, payload: { brightness } });

// animations
export const playAnimation = (animation) => ({ type: ActionType.PLAY_ANIMATION, payload: { animation } });
export const stopAnimation = () => ({ type: ActionType.STOP_ANIMATION });
export const setDelay = (delay) => ({ type: ActionType.SET_DELAY, payload: { delay } });
export const setColor = (color) => ({ type: ActionType.SET_COLOR, payload: { color } });
export const setAnimation = (animation) => ({ type: ActionType.SET_ANIMATION, payload: { animation } });

// games 
export const playGame = (game) => ({ type: ActionType.PLAY_GAME, payload: { game } });
export const stopGame = () => ({ type: ActionType.STOP_GAME });
export const setGameSpeed = (delay) => ({ type: ActionType.SET_GAME_SPEED, payload: { delay } });
export const setGame = (game) => ({ type: ActionType.SET_GAME, payload: { game } });