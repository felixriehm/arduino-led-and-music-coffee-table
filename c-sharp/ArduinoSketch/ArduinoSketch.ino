#include <SoftwareSerial.h>   // We need this even if we're not using a SoftwareSerial object
// Due to the way the Arduino IDE compiles
#include <SerialCommand.h>

#include <SimpleSDAudio.h>

#include <easing.h>

#include <Adafruit_NeoPixel.h>
#ifdef __AVR__
#include <avr/power.h>
#endif

#define PIN 6

SerialCommand SCmd;   // The demo SerialCommand object

Adafruit_NeoPixel strip = Adafruit_NeoPixel(144, PIN, NEO_GRB + NEO_KHZ800);

boolean CancelModi = false;
boolean connected = false;
boolean initDone = false;
boolean audioInitDone = false;

struct RGB_set {
	unsigned char r;
	unsigned char g;
	unsigned char b;
} RGB_set;

struct HSV_set {
	signed int h;
	unsigned char s;
	unsigned char v;
} HSV_set;

struct RGB_set colorindex[360];

byte userInput = 0;

void setup()
{
	Serial.begin(115200);

	// Setup callbacks for SerialCommand commands
	SCmd.addCommand("1", SetTile);     // Echos the string argument back
	SCmd.addCommand("99", TestMethod);       // Turns LED on
	SCmd.addCommand("9", ModiRandomHsv);     // Echos the string argument back
	SCmd.addCommand("8", ModiStop);     // Echos the string argument back
	SCmd.addCommand("3", GetUserInput);       // Turns LED on
	SCmd.addCommand("0", Snake);       // Turns LED on
	SCmd.addCommand("2", SetAllTiles);        // Turns LED off
	SCmd.addCommand("4", ModiTransitionHsv);     // Echos the string argument back
	SCmd.addCommand("5", PlayMusic);     // Echos the string argument back
	SCmd.addCommand("6", StopMusic);     // Echos the string argument back
	SCmd.addCommand("7", PauseMusic);     // Echos the string argument back
	SCmd.addCommand("10", Connected);     // Echos the string argument back
	SCmd.addCommand("11", Disconnect);     // Echos the string argument back
	SCmd.addCommand("12", Memory);       // Turns LED on
	SCmd.addCommand("13", PrintFreeRam);       // Turns LED on
	SCmd.addCommand("14", ModiPipes);       // Turns LED on
	SCmd.addCommand("15", Stripes);       // Turns LED on
	SCmd.addDefaultHandler(unrecognized);  // Handler for command that isn't matched  (says "What?")

#if defined (__AVR_ATtiny85__)
	if (F_CPU == 16000000) clock_prescale_set(clock_div_1);
#endif

	strip.begin();
	strip.show(); // Initialize all pixels to 'off'


	struct RGB_set RGB;
	struct HSV_set HSV;
	int h;
	for (h = 0; h < 360; h++) {
		HSV.h = h; HSV.s = 255, HSV.v = 255;
		RGB.r = 0; RGB.g = 0, RGB.b = 0;
		HSV2RGB(HSV, &RGB);
		colorindex[h].r = RGB.r;
		colorindex[h].g = RGB.g;
		colorindex[h].b = RGB.b;
	}

	audioInitDone = false;
	connected = false;

	//InitModi();
	for (int i = 0; i< 48; i++)
	{
		int rnd = random(0, 360);
		strip.setPixelColor(i * 3, colorindex[rnd].r, colorindex[rnd].g, colorindex[rnd].b);
		strip.setPixelColor((i * 3) + 1, colorindex[rnd].r, colorindex[rnd].g, colorindex[rnd].b);
		strip.setPixelColor((i * 3) + 2, colorindex[rnd].r, colorindex[rnd].g, colorindex[rnd].b);
	}
	strip.show();
}

void loop()
{
	SCmd.readSerial();     // We don't do much, just process serial commands
}

void TestMethod() {
	char *arg;

	arg = SCmd.next();

	if (arg != NULL) {
		for (int i = 0; i< 48; i++) {
			char str[3] = { arg[i * 3],arg[(i * 3) + 1],arg[(i * 3) + 2] };
			strip.setPixelColor(i * 3, colorindex[atoi(str)].r, colorindex[atoi(str)].g, colorindex[atoi(str)].b);
			strip.setPixelColor((i * 3) + 1, colorindex[atoi(str)].r, colorindex[atoi(str)].g, colorindex[atoi(str)].b);
			strip.setPixelColor((i * 3) + 2, colorindex[atoi(str)].r, colorindex[atoi(str)].g, colorindex[atoi(str)].b);
		}
		strip.show();
		
	}
}

void Connected() {
	connected = true;
	SendMsg("Connection established.");
	if (initDone) {
		for (uint16_t i = 0; i<strip.numPixels(); i++) {
			strip.setPixelColor(i, colorindex[125].r, colorindex[125].g, colorindex[125].b);
		}
		strip.show();
	}

	if (!audioInitDone) {
		// If your SD card CS-Pin is not at Pin 4, enable and adapt the following line:
		SdPlay.setSDCSPin(53);

		// Init SdPlay and set audio mode and activate autoworker
		if (!SdPlay.init(SSDA_MODE_FULLRATE | SSDA_MODE_MONO | SSDA_MODE_AUTOWORKER)) {
			SendMsg("Sd init failed.");
		}
		SendMsg("Sd init succeeded.");
		audioInitDone = true;
	}
	
}

void SetTile()
{
	int led = GetNextArgAsInteger();
	int r = GetNextArgAsInteger();
	int g = GetNextArgAsInteger();
	int b = GetNextArgAsInteger();
	
	strip.setPixelColor(led, r, g, b);
	strip.setPixelColor(led + 1, r, g, b);
	strip.setPixelColor(led + 2, r, g, b);
	strip.show();
	SendMsg("Tile set.");
}

void SetAllTiles()
{
	int r = GetNextArgAsInteger();
	int g = GetNextArgAsInteger();
	int b = GetNextArgAsInteger();

	for (uint16_t i = 0; i<strip.numPixels(); i++) {
		strip.setPixelColor(i, r, g, b);
	}
	strip.show();
	SendMsg("All pixels set.");
}

void GetUserInput()
{
	userInput = GetNextArgAsInteger();
}

void InitModi() {
	initDone = false;
	for (uint16_t i = 0; i<48; i++) {
		strip.setPixelColor(i * 3, 255, 255, 255);
		strip.setPixelColor((i * 3) +1, 255, 255, 255);
		strip.setPixelColor((i * 3) +2, 255, 255, 255);
		strip.show();
		delay(50);
	}

	struct RGB_set RGB;
	struct HSV_set HSV;
	int i;
	int s;
	float counter = 0.0;
	float intervallSize = 1;
	float intervalSteps = 128;
	float intervalGap = intervallSize / intervalSteps;

	while (!connected) {
		counter = 0.0;
		for (i = 0; i < intervalSteps; i++)
		{
			counter = counter + intervalGap;
			HSV.h = 125;
			s= QuarticEaseInOut(counter) * 255;
			if(s > 255) {
				s = 255;
			}
			HSV.s = s;
			HSV.v = 255;
			RGB.r = 0;
			RGB.g = 0;
			RGB.b = 0;
			HSV2RGB(HSV, &RGB);
			for (uint16_t u = 0; u<strip.numPixels(); u++) {
				strip.setPixelColor(u, RGB.r, RGB.g, RGB.b);
			}
			strip.show();

			delay(20);
		}
		SCmd.readSerial();
		counter = 0.0;
		for (i = 0; i < intervalSteps; i++)
		{
			counter = counter + intervalGap;
			HSV.h = 125;
			s = (1 - QuarticEaseInOut(counter)) * 255;
			if (s < 0) {
				s = 0;
			}
			HSV.s = s;
			HSV.v = 255;
			RGB.r = 0;
			RGB.g = 0;
			RGB.b = 0;
			HSV2RGB(HSV, &RGB);
			for (uint16_t u = 0; u<strip.numPixels(); u++) {
				strip.setPixelColor(u, RGB.r, RGB.g, RGB.b);
			}
			strip.show();

			delay(20);
		}
		SCmd.readSerial();
	}

	for (uint16_t i = 0; i<strip.numPixels(); i++) {
		strip.setPixelColor(i, colorindex[125].r, colorindex[125].g, colorindex[125].b);
	}
	strip.show();
	delay(200);
	for (uint16_t i = 0; i<strip.numPixels(); i++) {
		strip.setPixelColor(i, 255, 255, 255);
	}
	strip.show();
	delay(200);
	for (uint16_t i = 0; i<strip.numPixels(); i++) {
		strip.setPixelColor(i, colorindex[125].r, colorindex[125].g, colorindex[125].b);
	}
	strip.show();
	initDone = true;
}

void Disconnect() {
	for (uint16_t i = 0; i<strip.numPixels(); i++) {
		strip.setPixelColor(i, colorindex[0].r, colorindex[0].g, colorindex[0].b);
	}
	strip.show();
}

void ModiRandomHsv() {
	SendMsg("ModiRandomHsv started.");
	CancelModi = false;
	randomSeed(analogRead(0));
	int i, rnd;
	while (!CancelModi)
	{
		for (i = 0; i< 48; i++)
		{
			rnd = random(0, 360);
			strip.setPixelColor(i*3, colorindex[rnd].r, colorindex[rnd].g, colorindex[rnd].b);
			strip.setPixelColor((i*3)+1, colorindex[rnd].r, colorindex[rnd].g, colorindex[rnd].b);
			strip.setPixelColor((i*3)+2, colorindex[rnd].r, colorindex[rnd].g, colorindex[rnd].b);
		}
		strip.show();
		delay(1000);
		for (size_t n = 0; n < 2; n++)
		{
			for (i = 0; i< 48; i++)
			{
				rnd = random(0, 360);
				strip.setPixelColor(i * 3, colorindex[rnd].r, colorindex[rnd].g, colorindex[rnd].b);
				strip.setPixelColor((i * 3) + 1, colorindex[rnd].r, colorindex[rnd].g, colorindex[rnd].b);
				strip.setPixelColor((i * 3) + 2, colorindex[rnd].r, colorindex[rnd].g, colorindex[rnd].b);
			}
			strip.show();
			delay(500);
		}
		SCmd.readSerial();
	}
	SendMsg("ModiRandomHsv ended.");
}

void SendMsg(char* myStr) {
	Serial.write(strlen(myStr));
	Serial.write(myStr);
}

void ModiStop() {
	CancelModi = true;
}

void ModiTransitionHsv() {
	SendMsg("ModiTransitionHsv started.");
	CancelModi = false;
	randomSeed(analogRead(0));
	int h[48];
	int i;
	for (int i = 0; i < 48; i++)
	{
		h[i] = random(0, 360);
	}

	while (!CancelModi)
	{
		for (i = 0; i < 48; i++)
		{
			strip.setPixelColor(i * 3, colorindex[h[i]].r, colorindex[h[i]].g, colorindex[h[i]].b);
			strip.setPixelColor((i * 3) + 1, colorindex[h[i]].r, colorindex[h[i]].g, colorindex[h[i]].b);
			strip.setPixelColor((i * 3) + 2, colorindex[h[i]].r, colorindex[h[i]].g, colorindex[h[i]].b);

			h[i] = h[i] + 2;
			if (h[i] >= 360)
			{
				h[i] = 0;
			}
		}
		strip.show();
		delay(20);
		SCmd.readSerial();
	}
	SendMsg("ModiTransitionHsv ended.");
}

void ModiPipes() {
	SendMsg("PipeModi started.");
	CancelModi = false;

	struct Point {
		int8_t x;
		int8_t y;
		byte color;
	} Point;

	byte pipeSize = 1;
	const byte pipeMaxSize = 20;
	byte pipeDirection;
	byte pipeColor;
	byte pipeIndex = 1;
	byte singlePipeMinLen = 2;
	byte singlePipeSize = 1;
	byte colors[4] = { 0, 240 ,120 ,60 };

	//initialisiere Pipe
	struct Point pipe[pipeMaxSize];

	randomSeed(analogRead(0));
	pipeDirection = random(0, 4);
	pipeColor = random(0, 4);
	pipe[0].x = random(0, 6);
	pipe[0].y = random(0, 8);
	pipe[0].color = colors[pipeColor];

	while (!CancelModi) {
		if (pipeIndex == pipeMaxSize) {
			pipeIndex = 0;
		}

		boolean change = false;
		if (singlePipeSize >= singlePipeMinLen) {
			change = random(0, 101) > 65;
		}
		if (change) {
			byte newColor = random(0, 4);
			if (pipeColor == newColor) {
				pipeColor = mod(newColor + 1, 4);
			}
			else {
				pipeColor = newColor;
			}
			singlePipeSize = 0;
		}

		switch (pipeDirection)
		{
		case 0: //right
			pipe[pipeIndex].x = mod(pipe[mod(pipeIndex - 1,pipeMaxSize)].x + 1, 6);
			pipe[pipeIndex].y = pipe[mod(pipeIndex - 1, pipeMaxSize)].y;
			break;
		case 1: //up
			pipe[pipeIndex].y = mod(pipe[mod(pipeIndex - 1, pipeMaxSize)].y - 1, 8);
			pipe[pipeIndex].x = pipe[mod(pipeIndex - 1, pipeMaxSize)].x;
			break;
		case 2: //left
			pipe[pipeIndex].x = mod(pipe[mod(pipeIndex - 1, pipeMaxSize)].x - 1, 6);
			pipe[pipeIndex].y = pipe[mod(pipeIndex - 1, pipeMaxSize)].y;
			break;
		case 3: //down
			pipe[pipeIndex].y = mod(pipe[mod(pipeIndex - 1, pipeMaxSize)].y + 1, 8);
			pipe[pipeIndex].x = pipe[mod(pipeIndex - 1, pipeMaxSize)].x;
			break;
		default: //right
			pipe[pipeIndex].x = mod(pipe[mod(pipeIndex - 1, pipeMaxSize)].x + 1, 6);
			pipe[pipeIndex].y = pipe[mod(pipeIndex - 1, pipeMaxSize)].y;
			break;
		}
		pipe[pipeIndex].color = colors[pipeColor];
		pipeIndex++;
		singlePipeSize++;
		if (pipeSize < pipeMaxSize) {
			pipeSize++;
		}

		if (change) {
			byte newPipeDirection = random(0, 4);
			if (newPipeDirection == pipeDirection || abs(newPipeDirection - pipeDirection) == 2) {
				pipeDirection = mod(newPipeDirection + 1, 4);
			}
			else {
				pipeDirection = newPipeDirection;
			}
		}

		for (uint16_t i = 0; i < strip.numPixels(); i++) {
			strip.setPixelColor(i, 0, 0, 0);
		}

		for (uint16_t i = mod(pipeIndex,pipeSize); i != mod(pipeIndex-1,pipeSize); i = mod(i + 1, pipeSize)) {
			byte startpixel = ConvertMatrixToStrip(pipe[i].y, pipe[i].x);
			strip.setPixelColor(startpixel, colorindex[pipe[i].color].r, colorindex[pipe[i].color].g, colorindex[pipe[i].color].b);
			strip.setPixelColor(startpixel + 1, colorindex[pipe[i].color].r, colorindex[pipe[i].color].g, colorindex[pipe[i].color].b);
			strip.setPixelColor(startpixel + 2, colorindex[pipe[i].color].r, colorindex[pipe[i].color].g, colorindex[pipe[i].color].b);
		}
		byte i = mod(pipeIndex-1, pipeSize);
		byte startpixel = ConvertMatrixToStrip(pipe[i].y, pipe[i].x);
		strip.setPixelColor(startpixel, colorindex[pipe[i].color].r, colorindex[pipe[i].color].g, colorindex[pipe[i].color].b);
		strip.setPixelColor(startpixel + 1, colorindex[pipe[i].color].r, colorindex[pipe[i].color].g, colorindex[pipe[i].color].b);
		strip.setPixelColor(startpixel + 2, colorindex[pipe[i].color].r, colorindex[pipe[i].color].g, colorindex[pipe[i].color].b);


		strip.show();
		delay(200);
		SCmd.readSerial();
	}
	SendMsg("ModiPipes ended.");
}

void Stripes() {
	SendMsg("ModiStripes started.");
	CancelModi = false;

	struct Point {
		int8_t x;
		int8_t y;
		byte color;
	} Point;

	int colors[4] = { 0, 240 ,120 ,60 };
	byte longStripesTimer = 0;
	byte directionChangeTimer = 0;
	int movingStep = 1;
	byte longStripesTimerLimit = 5;
	byte directionChangeTimerLimit;
	randomSeed(analogRead(0));
	directionChangeTimerLimit = random(5, 11) *10;
	shuffle(colors, 4);
	struct Point stripelong[8][3];
	struct Point stripeshort[6][3];

	//init long stripes
	for (size_t columns = 0; columns < 3; columns++)
	{
		byte color = colors[columns];
		for (size_t rows = 0; rows < 8; rows++)
		{
			stripelong[rows][columns].color = color;
			stripelong[rows][columns].y = rows;
			stripelong[rows][columns].x = 2 * columns;
		}
	}

	//init short stripes
	for (size_t columns = 0; columns < 3; columns++)
	{
		for (size_t rows = 0; rows < 6; rows++)
		{
			stripeshort[rows][columns].color = colors[3];
			stripeshort[rows][columns].y = 3 * columns;
			stripeshort[rows][columns].x = rows;
		}
	}

	while (!CancelModi) {
		//change direction
		if (directionChangeTimer == directionChangeTimerLimit) {
			movingStep = movingStep * -1;
			directionChangeTimer = 0;
			directionChangeTimerLimit = random(5, 11) *10;
			shuffle(colors, 4);
			//change long stripe color
			for (size_t columns = 0; columns < 3; columns++)
			{
				byte color = colors[columns];
				for (size_t rows = 0; rows < 8; rows++)
				{
					stripelong[rows][columns].color = color;
				}
			}

			//change short stripe color
			for (size_t columns = 0; columns < 3; columns++)
			{
				for (size_t rows = 0; rows < 6; rows++)
				{
					stripeshort[rows][columns].color = colors[3];
				}
			}
		}
		//move long stripes
		if (longStripesTimer == longStripesTimerLimit) {
			for (size_t columns = 0; columns < 3; columns++)
			{
				for (size_t rows = 0; rows < 8; rows++)
				{
					stripelong[rows][columns].x = mod(stripelong[rows][columns].x + movingStep, 6);
				}
			}
			longStripesTimer = 0;
		}

		//move short stripes
		for (size_t columns = 0; columns < 3; columns++)
		{
			for (size_t rows = 0; rows < 6; rows++)
			{
				stripeshort[rows][columns].y = mod(stripeshort[rows][columns].y + movingStep,9);
			}
		}
		directionChangeTimer++;
		longStripesTimer++;


		//draw background
		for (uint16_t i = 0; i < strip.numPixels(); i++) {
			strip.setPixelColor(i, 0, 0, 0);
		}

		//draw long stripes
		for (size_t columns = 0; columns < 3; columns++)
		{
			for (size_t rows = 0; rows < 8; rows++)
			{
				byte startpixel = ConvertMatrixToStrip(stripelong[rows][columns].y, stripelong[rows][columns].x);
				strip.setPixelColor(startpixel, colorindex[stripelong[rows][columns].color].r, colorindex[stripelong[rows][columns].color].g, colorindex[stripelong[rows][columns].color].b);
				strip.setPixelColor(startpixel + 1, colorindex[stripelong[rows][columns].color].r, colorindex[stripelong[rows][columns].color].g, colorindex[stripelong[rows][columns].color].b);
				strip.setPixelColor(startpixel + 2, colorindex[stripelong[rows][columns].color].r, colorindex[stripelong[rows][columns].color].g, colorindex[stripelong[rows][columns].color].b);
			}
		}

		//draw short stripes
		for (size_t columns = 0; columns < 3; columns++)
		{
			for (size_t rows = 0; rows < 6; rows++)
			{
				if (stripeshort[rows][columns].y == 8) {
					continue;
				}
				byte startpixel = ConvertMatrixToStrip(stripeshort[rows][columns].y, stripeshort[rows][columns].x);
				strip.setPixelColor(startpixel, colorindex[stripeshort[rows][columns].color].r, colorindex[stripeshort[rows][columns].color].g, colorindex[stripeshort[rows][columns].color].b);
				strip.setPixelColor(startpixel + 1, colorindex[stripeshort[rows][columns].color].r, colorindex[stripeshort[rows][columns].color].g, colorindex[stripeshort[rows][columns].color].b);
				strip.setPixelColor(startpixel + 2, colorindex[stripeshort[rows][columns].color].r, colorindex[stripeshort[rows][columns].color].g, colorindex[stripeshort[rows][columns].color].b);
			}
		}
		strip.show();
		delay(100);
		SCmd.readSerial();
	}
	SendMsg("ModiStripes ended.");
}

void Snake() {
	SendMsg("Snake started.");
	CancelModi = false;
	boolean ateFood = false;
	userInput = 0;
	byte snakeColor = 230;
	byte foodColor = 0;
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
		strip.setPixelColor(startpixel, colorindex[foodColor].r, colorindex[foodColor].g, colorindex[foodColor].b);
		strip.setPixelColor(startpixel+1, colorindex[foodColor].r, colorindex[foodColor].g, colorindex[foodColor].b);
		strip.setPixelColor(startpixel+2, colorindex[foodColor].r, colorindex[foodColor].g, colorindex[foodColor].b);

		//Zeichne Schlange
		for (byte i = 0; i < snakeSize; i++)
		{
			byte startpixel = ConvertMatrixToStrip(snake[i].y, snake[i].x);
			strip.setPixelColor(startpixel, colorindex[snakeColor].r, colorindex[snakeColor].g, colorindex[snakeColor].b);
			strip.setPixelColor(startpixel + 1, colorindex[snakeColor].r, colorindex[snakeColor].g, colorindex[snakeColor].b);
			strip.setPixelColor(startpixel + 2, colorindex[snakeColor].r, colorindex[snakeColor].g, colorindex[snakeColor].b);
		}

		strip.show();

		end = millis();
		delay(500 - (end-start));
	}
}

void Memory() {
	SendMsg("Memory started.");
	CancelModi = false;
	struct FieldPoint {
		int color;
		boolean opened;
	} FieldPoint;

	struct Point {
		int8_t x;
		int8_t y;
	} Point;

	struct FieldPoint field[8][6];
	int cards[12];

	struct Point selection;
	selection.x = 0;
	selection.y = 0;

	byte selectedCardSize = 0;

	struct Point selectedCards[2];

	int color[6] = { 0, 290, 240, 130, 60, 180 };
	for (byte i = 0; i < 6; i++)
	{
		cards[i*2] = color[i];
		cards[(i*2)+1] = color[i];
	}

	randomSeed(analogRead(0));
	shuffle(cards, 6);

	for (uint16_t i = 0; i<strip.numPixels(); i++) {
		strip.setPixelColor(i, 0, 0, 0);
	}
	byte startpixel = ConvertMatrixToStrip(selection.y, selection.x);
	strip.setPixelColor(startpixel, 255, 255, 255);
	strip.setPixelColor(startpixel + 1, 255, 255, 255);
	strip.setPixelColor(startpixel + 2, 255, 255, 255);
	startpixel = ConvertMatrixToStrip(selection.y + 1, selection.x);
	strip.setPixelColor(startpixel, 255, 255, 255);
	strip.setPixelColor(startpixel + 1, 255, 255, 255);
	strip.setPixelColor(startpixel + 2, 255, 255, 255);
	startpixel = ConvertMatrixToStrip(selection.y, selection.x + 1);
	strip.setPixelColor(startpixel, 255, 255, 255);
	strip.setPixelColor(startpixel + 1, 255, 255, 255);
	strip.setPixelColor(startpixel + 2, 255, 255, 255);
	startpixel = ConvertMatrixToStrip(selection.y + 1, selection.x + 1);
	strip.setPixelColor(startpixel, 255, 255, 255);
	strip.setPixelColor(startpixel + 1, 255, 255, 255);
	strip.setPixelColor(startpixel + 2, 255, 255, 255);

	strip.show();
	byte counter = 0;
	for (byte y = 0; y < 8; y = y+2)
	{
		for (byte x = 0; x < 6; x = x+2)
		{
			field[y][x].color = cards[counter];
			field[y][x].opened = false;
			counter++;
		}
	}
	while (!CancelModi) {
		while (!Serial.available()) {

		}
		delay(10);
		SCmd.readSerial();
		if (CancelModi) {
			SendMsg("Memory ended.");
			return;
		}

		// reset selected cards
		if (selectedCardSize == 2) {
			selectedCardSize = 0;
			if (field[selectedCards[0].y][selectedCards[0].x].color != field[selectedCards[1].y][selectedCards[1].x].color) {
				field[selectedCards[0].y][selectedCards[0].x].opened = false;
				field[selectedCards[1].y][selectedCards[1].x].opened = false;
			}
		}

		// move selection
		switch (userInput)
		{
			case 0: //right
				selection.x = mod(selection.x + 2, 6);
				break;
			case 1: //up
				selection.y = mod(selection.y - 2, 8);
				break;
			case 2: //left
				selection.x = mod(selection.x - 2, 6);
				break;
			case 3: //down
				selection.y = mod(selection.y + 2, 8);
				break;
			case 4: //open card
				if (!field[selection.y][selection.x].opened) {
					selectedCards[selectedCardSize].x = selection.x;
					selectedCards[selectedCardSize].y = selection.y;
					field[selection.y][selection.x].opened = true;
					selectedCardSize++;
				}
				break;
			default: //right
				selection.x = mod(selection.x + 2, 6);
				break;

		}

		// draw field
		for (byte y = 0; y < 8; y = y+2)
		{
			for (byte x = 0; x < 6; x= x+2)
			{
				
				if (field[y][x].opened) {
					startpixel = ConvertMatrixToStrip(y, x);
					strip.setPixelColor(startpixel, colorindex[field[y][x].color].r, colorindex[field[y][x].color].g, colorindex[field[y][x].color].b);
					strip.setPixelColor(startpixel + 1, colorindex[field[y][x].color].r, colorindex[field[y][x].color].g, colorindex[field[y][x].color].b);
					strip.setPixelColor(startpixel + 2, colorindex[field[y][x].color].r, colorindex[field[y][x].color].g, colorindex[field[y][x].color].b);
					startpixel = ConvertMatrixToStrip(y+1, x);
					strip.setPixelColor(startpixel, colorindex[field[y][x].color].r, colorindex[field[y][x].color].g, colorindex[field[y][x].color].b);
					strip.setPixelColor(startpixel + 1, colorindex[field[y][x].color].r, colorindex[field[y][x].color].g, colorindex[field[y][x].color].b);
					strip.setPixelColor(startpixel + 2, colorindex[field[y][x].color].r, colorindex[field[y][x].color].g, colorindex[field[y][x].color].b);
					startpixel = ConvertMatrixToStrip(y, x+1);
					strip.setPixelColor(startpixel, colorindex[field[y][x].color].r, colorindex[field[y][x].color].g, colorindex[field[y][x].color].b);
					strip.setPixelColor(startpixel + 1, colorindex[field[y][x].color].r, colorindex[field[y][x].color].g, colorindex[field[y][x].color].b);
					strip.setPixelColor(startpixel + 2, colorindex[field[y][x].color].r, colorindex[field[y][x].color].g, colorindex[field[y][x].color].b);
					startpixel = ConvertMatrixToStrip(y+1, x+1);
					strip.setPixelColor(startpixel, colorindex[field[y][x].color].r, colorindex[field[y][x].color].g, colorindex[field[y][x].color].b);
					strip.setPixelColor(startpixel + 1, colorindex[field[y][x].color].r, colorindex[field[y][x].color].g, colorindex[field[y][x].color].b);
					strip.setPixelColor(startpixel + 2, colorindex[field[y][x].color].r, colorindex[field[y][x].color].g, colorindex[field[y][x].color].b);
				}
				else {
					startpixel = ConvertMatrixToStrip(y, x);
					strip.setPixelColor(startpixel, 0, 0, 0);
					strip.setPixelColor(startpixel + 1, 0, 0, 0);
					strip.setPixelColor(startpixel + 2, 0, 0, 0);
					startpixel = ConvertMatrixToStrip(y+1, x);
					strip.setPixelColor(startpixel, 0, 0, 0);
					strip.setPixelColor(startpixel + 1, 0, 0, 0);
					strip.setPixelColor(startpixel + 2, 0, 0, 0);
					startpixel = ConvertMatrixToStrip(y, x+1);
					strip.setPixelColor(startpixel, 0, 0, 0);
					strip.setPixelColor(startpixel + 1, 0, 0, 0);
					strip.setPixelColor(startpixel + 2, 0, 0, 0);
					startpixel = ConvertMatrixToStrip(y+1, x+1);
					strip.setPixelColor(startpixel, 0, 0, 0);
					strip.setPixelColor(startpixel + 1, 0, 0, 0);
					strip.setPixelColor(startpixel + 2, 0, 0, 0);
				}
			}
		}

		// draw selection
		startpixel = ConvertMatrixToStrip(selection.y, selection.x);
		strip.setPixelColor(startpixel, 255, 255, 255);
		strip.setPixelColor(startpixel + 1, 255, 255, 255);
		strip.setPixelColor(startpixel + 2, 255, 255, 255);
		startpixel = ConvertMatrixToStrip(selection.y+1, selection.x);
		strip.setPixelColor(startpixel, 255, 255, 255);
		strip.setPixelColor(startpixel + 1, 255, 255, 255);
		strip.setPixelColor(startpixel + 2, 255, 255, 255);
		startpixel = ConvertMatrixToStrip(selection.y, selection.x+1);
		strip.setPixelColor(startpixel, 255, 255, 255);
		strip.setPixelColor(startpixel + 1, 255, 255, 255);
		strip.setPixelColor(startpixel + 2, 255, 255, 255);
		startpixel = ConvertMatrixToStrip(selection.y+1, selection.x+1);
		strip.setPixelColor(startpixel, 255, 255, 255);
		strip.setPixelColor(startpixel + 1, 255, 255, 255);
		strip.setPixelColor(startpixel + 2, 255, 255, 255);

		// draw selected cards
		for (size_t i = 0; i < selectedCardSize; i++)
		{
			startpixel = ConvertMatrixToStrip(selectedCards[i].y, selectedCards[i].x);
			strip.setPixelColor(startpixel, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].r, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].g, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].b);
			strip.setPixelColor(startpixel + 1, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].r, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].g, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].b);
			strip.setPixelColor(startpixel + 2, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].r, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].g, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].b);
			startpixel = ConvertMatrixToStrip(selectedCards[i].y+1, selectedCards[i].x);
			strip.setPixelColor(startpixel, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].r, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].g, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].b);
			strip.setPixelColor(startpixel + 1, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].r, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].g, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].b);
			strip.setPixelColor(startpixel + 2, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].r, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].g, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].b);
			startpixel = ConvertMatrixToStrip(selectedCards[i].y, selectedCards[i].x+1);
			strip.setPixelColor(startpixel, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].r, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].g, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].b);
			strip.setPixelColor(startpixel + 1, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].r, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].g, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].b);
			strip.setPixelColor(startpixel + 2, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].r, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].g, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].b);
			startpixel = ConvertMatrixToStrip(selectedCards[i].y+1, selectedCards[i].x+1);
			strip.setPixelColor(startpixel, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].r, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].g, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].b);
			strip.setPixelColor(startpixel + 1, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].r, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].g, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].b);
			strip.setPixelColor(startpixel + 2, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].r, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].g, colorindex[field[selectedCards[i].y][selectedCards[i].x].color].b);
		}

		strip.show();
	}
}

/* Arrange the N elements of ARRAY in random order.
Only effective if N is much smaller than RAND_MAX;
if this may not be the case, use a better random
number generator. */
void shuffle(int *array, size_t n)
{
	if (n > 1)
	{
		size_t i;
		for (i = 0; i < n - 1; i++)
		{
			size_t j = i + rand() / (RAND_MAX / (n - i) + 1);
			int t = array[j];
			array[j] = array[i];
			array[i] = t;
		}
	}
}

int8_t mod(int8_t a, int8_t b)
{
	int8_t r = a % b;
	return r < 0 ? r + b : r;
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

void PlayMusic()
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

uint32_t Wheel(byte WheelPos) {
	WheelPos = 255 - WheelPos;
	if (WheelPos < 85) {
		return strip.Color(255 - WheelPos * 3, 0, WheelPos * 3);
	}
	if (WheelPos < 170) {
		WheelPos -= 85;
		return strip.Color(0, WheelPos * 3, 255 - WheelPos * 3);
	}
	WheelPos -= 170;
	return strip.Color(WheelPos * 3, 255 - WheelPos * 3, 0);
}

int GetNextArgAsInteger() {
	char *arg;
	int aNumber;

	arg = SCmd.next();
	if (arg != NULL)
	{
		aNumber = atoi(arg);    // Converts a char string to an integer
		return aNumber;
	}
	else {
		return NULL;
	}
}

// This gets set as the default handler, and gets called when no other command matches. 
void unrecognized()
{
	SendMsg("Cant recognize command.");
}

void PrintFreeRam() {
	int aInt = freeRam();
	char freeram[15];
	char msg[80];
	
	sprintf(freeram, "%d", aInt);

	strcat(msg, "Free space between the heap and the stack is: ");
	strcat(msg, freeram);
	strcat(msg, " bytes");

	SendMsg(msg);
}

int freeRam()
{
	extern int __heap_start, *__brkval;
	int v;
	return (int)&v - (__brkval == 0 ? (int)&__heap_start : (int)__brkval);
}

/*******************************************************************************
* Function HSV2RGB
* Description: Converts an HSV color value into its equivalen in the RGB color space.
* Copyright 2010 by George Ruinelli
* The code I used as a source is from http://www.cs.rit.edu/~ncs/color/t_convert.html
* Parameters:
*   1. struct with HSV color (source)
*   2. pointer to struct RGB color (target)
* Notes:
*   - r, g, b values are from 0..255
*   - h = [0,360], s = [0,255], v = [0,255]
*   - NB: if s == 0, then h = 0 (undefined)
******************************************************************************/
void HSV2RGB(struct HSV_set HSV, struct RGB_set *RGB) {
	int i;
	float f, p, q, t, h, s, v;

	h = (float)HSV.h;
	s = (float)HSV.s;
	v = (float)HSV.v;

	s /= 255;

	if (s == 0) { // achromatic (grey)
		RGB->r = RGB->g = RGB->b = v;
		return;
	}

	h /= 60;            // sector 0 to 5
	i = floor(h);
	f = h - i;            // factorial part of h
	p = (unsigned char)(v * (1 - s));
	q = (unsigned char)(v * (1 - s * f));
	t = (unsigned char)(v * (1 - s * (1 - f)));

	switch (i) {
	case 0:
		RGB->r = v;
		RGB->g = t;
		RGB->b = p;
		break;
	case 1:
		RGB->r = q;
		RGB->g = v;
		RGB->b = p;
		break;
	case 2:
		RGB->r = p;
		RGB->g = v;
		RGB->b = t;
		break;
	case 3:
		RGB->r = p;
		RGB->g = q;
		RGB->b = v;
		break;
	case 4:
		RGB->r = t;
		RGB->g = p;
		RGB->b = v;
		break;
	default:        // case 5:
		RGB->r = v;
		RGB->g = p;
		RGB->b = q;
		break;
	}
}
