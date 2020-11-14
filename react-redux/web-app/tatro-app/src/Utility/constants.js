export const CONNECTION_STATUS = {
  CONNECTED: "connected",
  SEARCHING: "searching",
  CONNECTING: "connecting",
  DISCONNECTED: "disconnected",
};

export const COMMANDS = {
  CONNECTED: { cmdNumber: 1, name: "Connected" },
  SET_TILE: { cmdNumber: 2, name: "SetTile" },
  SET_ALL_TILES: { cmdNumber: 3, name: "SetAllTiles" },
  PLAY_MUSIC: { cmdNumber: 4, name: "PlayMusic" },
  STOP_MUSIC: { cmdNumber: 5, name: "StopMusic" },
  PAUSE_MUSIC: { cmdNumber: 6, name: "PauseMusic" },
  PRINT_FREE_RAM: { cmdNumber: 7, name: "PrintFreeRam" },
  SET_BRIGHTNESS: { cmdNumber: 8, name: "SetBrightness" },
  MODI_RANDOM_HSV: { cmdNumber: 9, name: "ModiRandomHsv" },
  MODI_TRANSITION_HSV: { cmdNumber: 10, name: "ModiTransitionHsv" },
  MODI_RAINBOW_ONE: { cmdNumber: 11, name: "ModiRainbowOne" },
  MODI_RAINBOW_TWO: { cmdNumber: 12, name: "ModiRainbowTwo" },
  MODI_THEATER_CHASE: { cmdNumber: 13, name: "ModiTheaterChase" },
  MODI_THEATER_CHASE_RAINBOW: { cmdNumber: 14, name: "ModiTheaterChaseRainbow" },
  GET_USER_INPUT: { cmdNumber: 15, name: "GetUserInput" },
  SNAKE: { cmdNumber: 16, name: "Snake" },
  MODI_STOP: { cmdNumber: 17, name: "ModiStop" },
  SET_RANDOM: { cmdNumber: 18, name: "SetRandom" }
};

export const SHOW_SPEED = {
  NORMAL: "normal",
  SLOW: "slow",
  SLOWER: "slower"
};

export const GAME_SPEED = {
  NORMAL: "normal",
  SLOW: "slow",
  SLOWER: "slower"
};


export const SHOWS = {
  RANDOM: { cmdNumber: 1, name: "Random" },
  TRANSITION: { cmdNumber: 2, name: "Transition" },
  RAINBOW_ONE: { cmdNumber: 3, name: "RainbowOne" },
  RAINBOW_TWO: { cmdNumber: 4, name: "RainbowTwo" },
  THEATER_CHASE: { cmdNumber: 5, name: "TheaterChase" },
  THEATER_CHASE_RAINBOW: { cmdNumber: 6, name: "TheaterChaseRainbow" },
};

export const MUSIC = {
  GET_LUCKY: { cmdNumber: 1, name: "Get Lucky", duration: "10:00 min", storageName: "gl.afm" },
  WHAT_IS_LOVE: { cmdNumber: 2, name: "What is Love", duration: "4:16 min", storageName: "wil.afm" },
  CANTINA_THEME: { cmdNumber: 3, name: "Cantina Theme", duration: "2:09 min", storageName: "ct.afm" }
};

export const GAMES = {
  SNAKE: { cmdNumber: 1, name: "Snake" },
};

export const GAME_CONTROL = {
  UP: { cmdNumber: 0, name: "Up" },
  DOWN: { cmdNumber: 2, name: "Down" },
  LEFT: { cmdNumber: 1, name: "Left" },
  RIGHT: { cmdNumber: 3, name: "Right" },
  ACTION: { cmdNumber: 4, name: "Action" },
}

export const TOOLS = {
  BRUSH: { name: "Brush" },
  BUCKET: { name: "Bucket" },
  ERASER: { name: "Ereaser" },
};

export const GRID_X = 8;
export const GRID_Y = 6;
export const HSV_HUE_MAX_DEGREE = 360;
export const COLORS = {
  BLACK: { colorIndex: 370, rgb: { r: 0, g: 0, b: 0 } },
  WHITE: { colorIndex: 371, rgb: { r: 255, g: 255, b: 255 } }
}

export const SERVICE_UUID = "0xFFE0";
export const CHARACTERISTC_UUID = "0xFFE1";
export const CMD_DELIMITER = " ";
export const CMD_TERMINATION = "\r";