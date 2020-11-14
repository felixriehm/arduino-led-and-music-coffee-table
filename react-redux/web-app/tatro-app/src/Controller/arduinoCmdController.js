import * as ConnectionController from './connectionController'
import { GRID_X, GRID_Y, COMMANDS, SHOWS, CMD_DELIMITER, GAMES } from '../Utility/constants'
import { playAnimation, stopAnimation, playGame, stopGame } from "../redux/actions";
import store from '../redux/store';


function convertColorandCoordinate(x, y, colorIndex) {
    var arduinoLed;
    // Leds inside the table are layed out in a "S" pattern. It starts in the corner where the power cord is and
    // travels along the long side of the table before making a turn. Each tile consists of three leds.

    if (y % 2 === 0) {
        arduinoLed = ((GRID_Y - y) * GRID_X) - x - 1;
    } else {
        arduinoLed = ((GRID_Y - 1 - y) * GRID_X) + x;
    }

    return { arduinoLed, colorIndex }
}

export function setTile(x, y, colorIndex) {
    var convertedValues = convertColorandCoordinate(x, y, colorIndex)

    ConnectionController.send(COMMANDS.SET_TILE.cmdNumber + CMD_DELIMITER + convertedValues.arduinoLed + CMD_DELIMITER + Math.floor(convertedValues.colorIndex));
}

export function connect() {
    ConnectionController.send(COMMANDS.CONNECTED.cmdNumber);
}

export function setAllTiles(tiles) {
    var convertedValues = tiles.map(value => convertColorandCoordinate(value.x, value.y, value.color.colorIndex)).sort((a, b) => a.arduinoLed - b.arduinoLed);
    var cmdArray = COMMANDS.SET_ALL_TILES.cmdNumber + CMD_DELIMITER;
    convertedValues.forEach(element => {
        cmdArray = cmdArray + ("000" + element.colorIndex).substr(-3,3);;
    });
    ConnectionController.send(cmdArray);
}

export function playShow(show, delayTime, colorIndex) {
    var convertedValues = convertColorandCoordinate(0, 0, colorIndex)
    switch (show) {
        case SHOWS.RANDOM:
            ConnectionController.send(COMMANDS.MODI_RANDOM_HSV.cmdNumber + CMD_DELIMITER + delayTime);
            break;
        case SHOWS.TRANSITION:
            ConnectionController.send(COMMANDS.MODI_TRANSITION_HSV.cmdNumber + CMD_DELIMITER + delayTime);
            break;
        case SHOWS.RAINBOW_ONE:
            ConnectionController.send(COMMANDS.MODI_RAINBOW_ONE.cmdNumber + CMD_DELIMITER + delayTime);
            break;
        case SHOWS.RAINBOW_TWO:
            ConnectionController.send(COMMANDS.MODI_RAINBOW_TWO.cmdNumber + CMD_DELIMITER + delayTime);
            break;
        case SHOWS.THEATER_CHASE:
            ConnectionController.send(COMMANDS.MODI_THEATER_CHASE.cmdNumber + CMD_DELIMITER + delayTime + CMD_DELIMITER + Math.floor(convertedValues.colorIndex));
            break;
        case SHOWS.THEATER_CHASE_RAINBOW:
            ConnectionController.send(COMMANDS.MODI_THEATER_CHASE_RAINBOW.cmdNumber + CMD_DELIMITER + delayTime);
            break;
        default:
            break;
    }
    store.dispatch(playAnimation(show));
}

export function stopShow() {
    ConnectionController.send(COMMANDS.MODI_STOP.cmdNumber);
    store.dispatch(stopAnimation());
}

export function playArduinoGame(game, delayTime) {
    switch (game) {
        case GAMES.SNAKE:
            ConnectionController.send(COMMANDS.SNAKE.cmdNumber + CMD_DELIMITER + delayTime);
            break;
        default:
            break;
    }
    store.dispatch(playGame(game));
}

export function stopArduinoGame() {
    ConnectionController.send(COMMANDS.MODI_STOP.cmdNumber);
    store.dispatch(stopGame());
}

export function doGameControl(gameInput) {
    ConnectionController.send(COMMANDS.GET_USER_INPUT.cmdNumber + CMD_DELIMITER + gameInput.cmdNumber);
}

export function playMusic(track) {
    ConnectionController.send(COMMANDS.PLAY_MUSIC.cmdNumber + CMD_DELIMITER + track.storageName);
}

export function stopMusic() {
    ConnectionController.send(COMMANDS.STOP_MUSIC.cmdNumber);
}

export function pauseMusic() {
    ConnectionController.send(COMMANDS.PAUSE_MUSIC.cmdNumber);
}

export function printFreeRam() {
    ConnectionController.send(COMMANDS.PRINT_FREE_RAM.cmdNumber);
}

export function setBrightness(brightness) {
    ConnectionController.send(COMMANDS.SET_BRIGHTNESS.cmdNumber + CMD_DELIMITER + brightness);
}

export function setRandom() {
    ConnectionController.send(COMMANDS.SET_RANDOM.cmdNumber);
}