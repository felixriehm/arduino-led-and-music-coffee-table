/*
 Name:    Arduino.ino
 Created: 01.01.2016 13:04:31
 Author:  frieh
*/
#include <SerialCommand.h>

#include <SimpleSDAudio.h>

#include <Adafruit_NeoPixel.h>

#define PIN 6

SerialCommand SCmd;

Adafruit_NeoPixel strip = Adafruit_NeoPixel(144, PIN, NEO_GRB + NEO_KHZ800);

boolean audioInitDone = false;
boolean CancelModi = false;
boolean modiIsPlaying = false;
byte userInput = 0;

void Connected()
{
  SendMsg("Connection established.");

  if (!audioInitDone)
  {

    SdPlay.setSDCSPin(53);

    // Init SdPlay and set audio mode and activate autoworker
    if (!SdPlay.init(SSDA_MODE_FULLRATE | SSDA_MODE_MONO | SSDA_MODE_AUTOWORKER))
    {
      SendMsg("Sd init failed.");
    }
    else
    {
      SendMsg("Sd init succeeded.");
      audioInitDone = true;
    }
  }
}

uint16_t convertColor(int color)
{
  //sourceRangeMin
  float rmin = 0;
  //sourceRangeMMax
  float rmax = 360;
  //targetRangeMin
  float tmin = 0;
  //targetRangeMax
  float tmax = 65535;
  uint16_t hue = (color - rmin) / (rmax - rmin) * (tmax - tmin) + tmin;
  return hue;
}

void SetPixels(int colorIndex, uint8_t tile)
{
  int led = tile * 3;
  if (colorIndex <= 360)
  {
    uint16_t hue = convertColor(colorIndex);
    uint32_t color = strip.gamma32(strip.ColorHSV(hue));
    strip.setPixelColor(led, color);
    strip.setPixelColor(led + 1, color);
    strip.setPixelColor(led + 2, color);
  }

  // black
  if (colorIndex == 370)
  {
    strip.setPixelColor(led, 0, 0, 0);
    strip.setPixelColor(led + 1, 0, 0, 0);
    strip.setPixelColor(led + 2, 0, 0, 0);
  }

  //white
  if (colorIndex == 371)
  {
    strip.setPixelColor(led, 255, 255, 255);
    strip.setPixelColor(led + 1, 255, 255, 255);
    strip.setPixelColor(led + 2, 255, 255, 255);
  }
}

void SetTile()
{
  char *arg;
  uint8_t led;
  int colorIndex;

  arg = SCmd.next();
  led = atoi(arg);
  arg = SCmd.next();
  colorIndex = atoi(arg);

  SetPixels(colorIndex, led);

  strip.show();
}

void SetTiles( )
{
  SendMsg("Set Tiles beginning");

SendMsg("Set Tiles end");
  strip.show();
}

void SetRandom() {
  for (int i = 0; i < 48; i++)
  {
    int hue = random(0, 65535);
    int led = i * 3;
    uint32_t color = strip.gamma32(strip.ColorHSV(hue));
    strip.setPixelColor(led, color);
    strip.setPixelColor(led + 1, color);
    strip.setPixelColor(led + 2, color);
  }
  strip.show();
}

void SetBrightness( )
{
  char *arg;
  uint8_t brightness;
  SendMsg("Set brightness.");

  arg = SCmd.next();
  brightness = atoi(arg);

  strip.setBrightness(brightness);

  strip.show();
}

void ModiRandomHsv( ) {
  if(modiIsPlaying) {
    return;
  } else {
    modiIsPlaying = true;
  }
  
  char *arg;
  uint16_t delayTime;
  arg = SCmd.next();
  delayTime = atoi(arg);
  
  SendMsg("ModiRandomHsv started.");
  CancelModi = false;
  randomSeed(analogRead(0));
  int i;
  while (!CancelModi)
  {
    for (i = 0; i< 48; i++)
    {
      uint16_t hue = random(0, 65535);
      int led = i * 3;
      uint32_t color = strip.gamma32(strip.ColorHSV(hue));
      strip.setPixelColor(led, color);
      strip.setPixelColor(led+1, color);
      strip.setPixelColor(led+2, color);
    }
    strip.show();
    delay(delayTime);
    SCmd.readSerial();
  }
  SendMsg("ModiRandomHsv ended.");
}

void ModiTransitionHsv( ) {
  if(modiIsPlaying) {
    return;
  } else {
    modiIsPlaying = true;
  }
  
  char *arg;
  uint16_t colorStep;
  arg = SCmd.next();
  colorStep = atoi(arg);
  
  SendMsg("ModiTransitionHsv started.");
  CancelModi = false;
  randomSeed(analogRead(0));
  uint16_t h[48];
  int i;
  for (int i = 0; i < 48; i++)
  {
    h[i] = random(0, 65535);
  }

  long firstPixelHue= 0;
  while (!CancelModi)
  {
    for (i = 0; i < 48; i++)
    {
      h[i] = colorStep + h[i];
      int led = i * 3;
      strip.setPixelColor(led, strip.gamma32(strip.ColorHSV(h[i])));
      strip.setPixelColor(led + 1, strip.gamma32(strip.ColorHSV(h[i])));
      strip.setPixelColor(led + 2, strip.gamma32(strip.ColorHSV(h[i])));
    }
    strip.show();
    delay(10);
    SCmd.readSerial();
  }
  SendMsg("ModiTransitionHsv ended.");
}

void ModiRainbowOne( ) {
  if(modiIsPlaying) {
    return;
  } else {
    modiIsPlaying = true;
  }
  
  char *arg;
  uint16_t colorStep;
  arg = SCmd.next();
  colorStep = atoi(arg);

  SendMsg("ModiRainbowOne started.");
  CancelModi = false;
  int i;

  long firstPixelHue= 0;
  while (!CancelModi)
  {
    for (i = 0; i < 48; i++)
    {
      int pixelHue = firstPixelHue + (i * 65536L / 48);
      int led = i * 3;
      strip.setPixelColor(led, strip.gamma32(strip.ColorHSV(pixelHue)));
      strip.setPixelColor(led + 1, strip.gamma32(strip.ColorHSV(pixelHue)));
      strip.setPixelColor(led + 2, strip.gamma32(strip.ColorHSV(pixelHue)));
    }
    strip.show();
    delay(10);
    firstPixelHue = firstPixelHue + colorStep;
    SCmd.readSerial();
  }
  SendMsg("ModiRainbowOne ended.");
}

void ModiRainbowTwo( ) {
  if(modiIsPlaying) {
    return;
  } else {
    modiIsPlaying = true;
  }
  
  char *arg;
  uint16_t colorStep;
  arg = SCmd.next();
  colorStep = atoi(arg);

  SendMsg("ModiRainbowTwo started.");
  CancelModi = false;
  int i;

  long firstPixelHue= 0;
  while (!CancelModi)
  {
    for (i = 0; i < 48; i++)
    {
      int led = i * 3;
      strip.setPixelColor(led, strip.gamma32(strip.ColorHSV(firstPixelHue)));
      strip.setPixelColor(led + 1, strip.gamma32(strip.ColorHSV(firstPixelHue)));
      strip.setPixelColor(led + 2, strip.gamma32(strip.ColorHSV(firstPixelHue)));
    }
    strip.show();
    delay(10);
    firstPixelHue = firstPixelHue + colorStep;
    SCmd.readSerial();
  }
  SendMsg("ModiRainbowTwo ended.");
}

void ModiTheaterChase( ) {
  if(modiIsPlaying) {
    return;
  } else {
    modiIsPlaying = true;
  }
  
  char *arg;
  uint16_t delayTime;
  int colorIndex;
  arg = SCmd.next();
  delayTime = atoi(arg);
  arg = SCmd.next();
  colorIndex = atoi(arg);
  
  SendMsg("ModiTheaterChase started.");
  CancelModi = false;
  
  uint16_t hue = convertColor(colorIndex);
  while (!CancelModi)
  {
    for (int b = 0; b < 3; b++)
    {
        strip.clear();
        for (int c = b; c < 48; c += 3)
        {
            int led = c * 3;
            strip.setPixelColor(led, hue);
            strip.setPixelColor(led + 1, hue);
            strip.setPixelColor(led + 2, hue);
        }
        strip.show();
        delay(delayTime);
        SCmd.readSerial();
    }
  }
  SendMsg("ModiTheaterChase ended.");
}

void ModiTheaterChaseRainbow( ) {
  if(modiIsPlaying) {
    return;
  } else {
    modiIsPlaying = true;
  }
  
  char *arg;
  uint16_t delayTime;
  arg = SCmd.next();
  delayTime = atoi(arg);
  
  SendMsg("ModiTheaterChaseRainbow started.");
  CancelModi = false;
  
  int firstPixelHue = 0;
  while (!CancelModi)
  {
    for (int b = 0; b < 3; b++)
    {
      strip.clear();
      for (int c = b; c < 48; c += 3)
      {
        int hue = firstPixelHue + c * 65536L / 48;
        uint32_t color = strip.gamma32(strip.ColorHSV(hue));
        int led = c * 3;
        strip.setPixelColor(led, color);
        strip.setPixelColor(led + 1, color);
        strip.setPixelColor(led + 2, color);
      }
      strip.show();
      firstPixelHue += 65536 / 90;
      delay(delayTime); 
      SCmd.readSerial();
     }
  }
  SendMsg("ModiTheaterChaseRainbow ended.");
}

void Snake( ) {
  if(modiIsPlaying) {
    return;
  } else {
    modiIsPlaying = true;
  }
  
  char *arg;
  uint16_t delayTime;
  arg = SCmd.next();
  delayTime = atoi(arg);
  
  SendMsg("Snake started.");
  CancelModi = false;
  boolean ateFood = false;
  userInput = 0;
  uint16_t snakeColor = 43690;
  uint16_t foodColor = 0;
  byte snakeSize = 1;
  unsigned long start;
  unsigned long end;
  boolean free;

  struct Point {
    int8_t x;
    int8_t y;
    int8_t currentDirection;
    int8_t oldDirection;
  } Point;

  randomSeed(analogRead(0));

  //initialisiere Hintergrund
  for (uint16_t i = 0; i<strip.numPixels(); i++) {
    strip.setPixelColor(i, 255, 255, 255);
  }

  //initialisiere Schlange
  struct Point snake[48];
  snake[0].x = random(0, 6);
  snake[0].y = random(0, 8);
  snake[0].currentDirection = 0;

  //initialisiere Futter
  struct Point food;
  food.x = random(0, 6);
  food.y = random(0, 8);
  while (food.x == snake[0].x && food.y == snake[0].y) {
    food.x = random(0, 6);
    food.y = random(0, 8);
  }

  while (!CancelModi)
  {
    start = millis();
    SCmd.readSerial();
    if (CancelModi) {
      SendMsg("Snake ended.");
      return;
    }

    //bewege schlange
    snake[0].oldDirection = userInput;
    snake[0].currentDirection = userInput;
    for (byte i = 0; i < snakeSize; i++)
    {
      switch (snake[i].currentDirection)
      {
      case 0: //right
        snake[i].x = mod(snake[i].x + 1,6);
        break;
      case 1: //up
        snake[i].y = mod(snake[i].y - 1, 8);
        break;
      case 2: //left
        snake[i].x = mod(snake[i].x - 1, 6);
        break;
      case 3: //down
        snake[i].y = mod(snake[i].y + 1, 8);
        break;
      default: //right
        snake[i].x = mod(snake[i].x + 1, 6);
        break;
      }
      if (i != 0) {
        snake[i].oldDirection = snake[i].currentDirection;
        snake[i].currentDirection = snake[i - 1].oldDirection;
      }
    }

    //vergrößere Schlange
    if (snake[0].x == food.x && snake[0].y == food.y) {
      switch (snake[snakeSize - 1].oldDirection)
      {
      case 0: //right
        snake[snakeSize].x = mod(snake[snakeSize - 1].x - 1,6);
        snake[snakeSize].y = snake[snakeSize - 1].y;
        snake[snakeSize].currentDirection = 0;
        break;
      case 1: //up
        snake[snakeSize].x = snake[snakeSize - 1].x;
        snake[snakeSize].y = mod(snake[snakeSize - 1].y + 1 , 8);
        snake[snakeSize].currentDirection = 1;
        break;
      case 2: //left
        snake[snakeSize].x = mod(snake[snakeSize - 1].x + 1, 6);
        snake[snakeSize].y = snake[snakeSize - 1].y;
        snake[snakeSize].currentDirection = 2;
        break;
      case 3: //down
        snake[snakeSize].x = snake[snakeSize - 1].x;
        snake[snakeSize].y = mod(snake[snakeSize - 1].y - 1, 8);
        snake[snakeSize].currentDirection = 3;
        break;
      default:
        break;
      }
      snakeSize++;
      ateFood = true;
    }

    //Futter neu setzen
    if (ateFood) {
      randomSeed(analogRead(0));
      food.x = random(0, 6);
      food.y = random(0, 8);

      for (byte i = 0; i < snakeSize; i++)
      {
        if (food.x == snake[i].x && food.y == snake[i].y) {
          for (byte y = 0; y < 8; y++) {
            for (byte x = 0; x < 6; x++) {
              free = true;
              for (byte l = 0; l < snakeSize; l++)
              {
                if (x == snake[l].x && y == snake[l].y) {
                  free = false;
                  break;
                }
              }
              if (free) {
                food.x = x;
                food.y = y;
                goto abort;
              }
            }
          }
        }
      }
      abort:
      ateFood = false;
    }

    //Überprüfe Tod
    for (byte i = 0; i < snakeSize-1; i++)
    {
      if ((snake[snakeSize - 1].x == snake[i].x && snake[snakeSize - 1].y == snake[i].y) ||
        (snake[0].x == snake[i+1].x && snake[0].y == snake[i+1].y)) {
        CancelModi = true;
      }
    }

    //Zeichne Hintergrund
    for (uint16_t i = 0; i<strip.numPixels(); i++) {
      strip.setPixelColor(i, 255, 255, 255);
    }

    //Zeichne Futter
    byte startpixel = ConvertMatrixToStrip(food.y, food.x);
    strip.setPixelColor(startpixel, strip.gamma32(strip.ColorHSV(foodColor)));
    strip.setPixelColor(startpixel + 1, strip.gamma32(strip.ColorHSV(foodColor)));
    strip.setPixelColor(startpixel + 2, strip.gamma32(strip.ColorHSV(foodColor)));

    //Zeichne Schlange
    for (byte i = 0; i < snakeSize; i++)
    {
      byte startpixel = ConvertMatrixToStrip(snake[i].y, snake[i].x);
      strip.setPixelColor(startpixel, strip.gamma32(strip.ColorHSV(snakeColor)));
      strip.setPixelColor(startpixel + 1, strip.gamma32(strip.ColorHSV(snakeColor)));
      strip.setPixelColor(startpixel + 2, strip.gamma32(strip.ColorHSV(snakeColor)));
    }

    strip.show();

    end = millis();
    delay(delayTime - (end-start));
  }
}

int8_t mod(int8_t a, int8_t b)
{
  int8_t r = a % b;
  return r < 0 ? r + b : r;
}

void GetUserInput( )
{
  char *arg;
  int aNumber;

  arg = SCmd.next();
  if (arg != NULL)
  {
    aNumber = atoi(arg);    // Converts a char string to an integer
    userInput = aNumber;
  }
  else {
    userInput = -1;
  }
}

byte ConvertMatrixToStrip(byte row, byte column) {
  byte startPixel;
  if (column % 2 == 0)
  {
    startPixel = (column * 8) + row;
  }
  else
  {
    startPixel = (column * 8) + 8 - 1 - row;
  }
  startPixel = startPixel * 3;

  return startPixel;
}

void ModiStop() {
  SendMsg("Modi stopped.");
  modiIsPlaying = false;
  CancelModi = true;
}

void PlayMusic( )
{
  char *arg;
  arg = SCmd.next();

  if (!SdPlay.setFile(arg)) {
    SendMsg("Could not find the file.");
    return;
  }

  SendMsg("Plays music.");
  SdPlay.play();
}

void StopMusic()
{
  SendMsg("Stops music.");
  SdPlay.stop();
}

void PauseMusic()
{
  SendMsg("Pause music.");
  SdPlay.pause();
}

void SendMsg(String myStr)
{
  String finilaizedString = String(myStr + "\r");
  Serial.write(finilaizedString.c_str());
}

// This gets set as the default handler, and gets called when no other command matches.
void unrecognized()
{
  SendMsg("Cant recognize command");
}

void PrintFreeRam()
{
  int space = freeRam();
  SendMsg("Free space between the heap and the stack is: " + String(space) + " bytes");
}

int freeRam()
{
  extern int __heap_start, *__brkval;
  int v;
  return (int)&v - (__brkval == 0 ? (int)&__heap_start : (int)__brkval);
}



void setup()
{
  Serial.begin(115200);

  // Setup callbacks for SerialCommand commands
  SCmd.addCommand("1", Connected);
  SCmd.addCommand("2", SetTile);
  SCmd.addCommand("3", SetTiles);
  SCmd.addCommand("4", PlayMusic);
  SCmd.addCommand("5", StopMusic);
  SCmd.addCommand("6", PauseMusic);
  SCmd.addCommand("7", PrintFreeRam);
  SCmd.addCommand("8", SetBrightness);
  SCmd.addCommand("9", ModiRandomHsv);
  SCmd.addCommand("10", ModiTransitionHsv);
  SCmd.addCommand("11", ModiRainbowOne);
  SCmd.addCommand("12", ModiRainbowTwo);
  SCmd.addCommand("13", ModiTheaterChase);
  SCmd.addCommand("14", ModiTheaterChaseRainbow);
  SCmd.addCommand("15", GetUserInput);
  SCmd.addCommand("16", Snake);
  SCmd.addCommand("17", ModiStop);
  SCmd.addCommand("18", SetRandom);
  SCmd.addDefaultHandler(unrecognized);
  

  strip.begin();
  strip.show(); // Initialize all pixels to 'off'

  audioInitDone = false;

  SetRandom();
}

void loop()
{
  SCmd.readSerial(); // We don't do much, just process serial commands
}
