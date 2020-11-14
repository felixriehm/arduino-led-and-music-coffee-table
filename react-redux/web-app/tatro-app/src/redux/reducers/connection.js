import { CONNECT, FINILIZE_CONNECTION, DISCONNECT, REQUEST_DEVICE, CHANGE_SERVICE_UUID, CHANGE_CHARACTERISTIC_UUID, CHANGE_LOG_OUTPUT_TEXT, FLUSH_LOG_OUTPUT_TEXT } from "../actionTypes";
import { CONNECTION_STATUS, SERVICE_UUID, CHARACTERISTC_UUID } from "../../Utility/constants";

const initialState = {
  connectionStatus: CONNECTION_STATUS.DISCONNECTED,
  serviceUuid: SERVICE_UUID,
  characteristicUuid: CHARACTERISTC_UUID,
  logOutputText: [],
  deviceName: "",
  deviceId: ""
};

export default function (state = initialState, action) {
  switch (action.type) {
    case CONNECT: {
      const { deviceName, deviceId } = action.payload;
      return {
        ...state,
        connectionStatus: CONNECTION_STATUS.CONNECTING,
        deviceName: deviceName,
        deviceId: deviceId,
      };
    }
    case FINILIZE_CONNECTION: {
      return {
        ...state,
        connectionStatus: CONNECTION_STATUS.CONNECTED,
      };
    }
    case DISCONNECT: {
      return {
        ...state,
        connectionStatus: CONNECTION_STATUS.DISCONNECTED,
        deviceName: "",
        deviceId: ""
      };
    }
    case REQUEST_DEVICE: {
      return {
        ...state,
        connectionStatus: CONNECTION_STATUS.SEARCHING,
      };
    }
    case CHANGE_SERVICE_UUID: {
      const { serviceUuid } = action.payload;
      return {
        ...state,
        serviceUuid: serviceUuid,
      };
    }
    case CHANGE_CHARACTERISTIC_UUID: {
      const { characteristicUuid } = action.payload;
      return {
        ...state,
        characteristicUuid: characteristicUuid,
      };
    }
    case CHANGE_LOG_OUTPUT_TEXT: {
      const { text, type } = action.payload;
      return {
        ...state,
        logOutputText: [...state.logOutputText, { text, type } ],
      };
    }
    case FLUSH_LOG_OUTPUT_TEXT: {
      return {
        ...state,
        logOutputText: [],
      };
    }
    default:
      return state;
  }
}