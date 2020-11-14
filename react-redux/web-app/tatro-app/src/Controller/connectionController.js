import { connectToBluetooth, finilizeBluetoothConnection, disconnectFromBluetooth, searchBluetoothDevice, changeLogOutputText } from "../redux/actions";
import store from '../redux/store';
import { CMD_TERMINATION } from '../Utility/constants'

// Selected device object cache
let deviceCache = null;
// Characteristic object cache
let characteristicCache = null;

let readBuffer = '';


// Launch Bluetooth device chooser and connect to the selected
export function connect() {
  return (deviceCache ? Promise.resolve(deviceCache) :
    requestBluetoothDevice()).
    then(device => connectDeviceAndCacheCharacteristic(device)).
    then(characteristic => startNotifications(characteristic)).
    catch(error => log(error));
}

function requestBluetoothDevice() {
  store.dispatch(searchBluetoothDevice());
  log('Requesting bluetooth device...');

  return navigator.bluetooth.requestDevice({
    filters: [{ services: [parseInt(store.getState().connection.serviceUuid)] }],
  }).
    then(device => {
      log('"' + device.name + '" bluetooth device selected');
      deviceCache = device;

      deviceCache.addEventListener('gattserverdisconnected', handleDisconnection);

      return deviceCache;
    }, reason => {
      store.dispatch(disconnectFromBluetooth());
    });
}

// Connect to the device specified, get service and characteristic
function connectDeviceAndCacheCharacteristic(device) {
  if (device.gatt.connected && characteristicCache) {
    return Promise.resolve(characteristicCache);
  }

  store.dispatch(connectToBluetooth(deviceCache));
  log('Connecting to GATT server...');

  return device.gatt.connect().
    then(server => {
      log('GATT server connected, getting service...');

      return server.getPrimaryService(parseInt(store.getState().connection.serviceUuid));
    }).
    then(service => {
      log('Service found, getting characteristic...');

      return service.getCharacteristic(parseInt(store.getState().connection.characteristicUuid));
    }).
    then(characteristic => {
      store.dispatch(finilizeBluetoothConnection());
      log('Characteristic found');
      characteristicCache = characteristic;

      return characteristicCache;
    });
}

// Enable the characteristic changes notification
function startNotifications(characteristic) {
  log('Starting notifications...');

  return characteristic.startNotifications().
    then(() => {
      log('Notifications started');
      characteristic.addEventListener('characteristicvaluechanged',
        handleCharacteristicValueChanged);
    });
}

// Output to terminal
function log(data, type = 'system') {
  store.dispatch(changeLogOutputText(data.toString(), type))
}

function handleDisconnection(event) {
  let device = event.target;

  log('"' + device.name +
    '" bluetooth device disconnected, trying to reconnect...');

  connectDeviceAndCacheCharacteristic(device).
    then(characteristic => startNotifications(characteristic)).
    catch(error => log(error));
}

export function disconnect() {
  if (deviceCache) {
    log('Disconnecting from "' + deviceCache.name + '" bluetooth device...');
    deviceCache.removeEventListener('gattserverdisconnected',handleDisconnection);

    if (deviceCache.gatt.connected) {
      deviceCache.gatt.disconnect();
      store.dispatch(disconnectFromBluetooth());
      log('"' + deviceCache.name + '" bluetooth device disconnected');
    }
    else {
      log('"' + deviceCache.name +
        '" bluetooth device is already disconnected');
    }
  }
  if (characteristicCache) {
    log('removed characteristicvaluechanged eventhandler');
    characteristicCache.removeEventListener('characteristicvaluechanged',
      handleCharacteristicValueChanged);
    characteristicCache = null;
  }
  deviceCache = null;
}

function handleCharacteristicValueChanged(event) {
  let value = new TextDecoder().decode(event.target.value);

  for (let c of value) {
    if (c === '\r') {
      let data = readBuffer.trim();
      readBuffer = '';

      if (data) {
        receive(data);
      }
    }
    else {
      readBuffer += c;
    }
  }
}

// Received data handling
function receive(data) {
  log(data, 'in');
}

export function send(data) {
  data = String(data);
  
  if (!data || !characteristicCache) {
    return;
  }

  data += '\r';

  if (data.length > 20) {
    let chunks = data.match(/(.|[\r]){1,20}/g);

    writeToCharacteristic(characteristicCache, chunks[0]);

    for (let i = 1; i < chunks.length; i++) {
      setTimeout(() => {
        writeToCharacteristic(characteristicCache, chunks[i]);
      }, i * 100);
    }
  }
  else {
    writeToCharacteristic(characteristicCache, data);
  }
}

function writeToCharacteristic(characteristic, data) {
  var finaleString = data + CMD_TERMINATION;
  log(finaleString, 'out');
  characteristic.writeValue(new TextEncoder().encode(finaleString));
}