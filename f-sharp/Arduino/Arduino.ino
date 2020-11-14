/*
 Name:		Arduino.ino
 Created:	01.01.2016 13:04:31
 Author:	frieh
*/

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

SerialCommand SCmd;

Adafruit_NeoPixel strip = Adafruit_NeoPixel(144, PIN, NEO_GRB + NEO_KHZ800);

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

struct RGB_set colorindex[400];

void setup()
{
	Serial.begin(115200);

	// Setup callbacks for SerialCommand commands
	SCmd.addCommand("1", Connected);
	SCmd.addCommand("2", SetTile);     
	SCmd.addCommand("3", SetAllTiles);  
	SCmd.addCommand("4", PlayMusic);
	SCmd.addCommand("5", StopMusic);
	SCmd.addCommand("6", PauseMusic);
	SCmd.addCommand("7", PrintFreeRam);
	SCmd.addDefaultHandler(unrecognized);

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

	//black
	colorindex[370].r = 0;
	colorindex[370].g = 0;
	colorindex[370].b = 0;

	//white
	colorindex[371].r = 255;
	colorindex[371].g = 255;
	colorindex[371].b = 255;

	audioInitDone = false;

	for (int i = 0; i< 48; i++)
	{
		int rnd = random(0, 360);
		int led = i * 3;
		int r = colorindex[rnd].r;
		int g = colorindex[rnd].g;
		int b = colorindex[rnd].b;
		strip.setPixelColor(led, r, g, b);
		strip.setPixelColor(led + 1, r, g, b);
		strip.setPixelColor(led + 2, r, g, b);
	}
	strip.show();
}

void loop()
{
	SCmd.readSerial();     // We don't do much, just process serial commands
}

void Connected() {
	SendMsg("Connection established.");

	if (!audioInitDone) {
		
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
	char *arg;
	arg = SCmd.next();

	String argString = String(arg);
	int led = argString.substring(0, 3).toInt();
	int color = argString.substring(3, 6).toInt();
	int r = colorindex[color].r;
	int g = colorindex[color].g;
	int b = colorindex[color].b;

	strip.setPixelColor(led, r, g, b);
	strip.setPixelColor(led + 1, r, g, b);
	strip.setPixelColor(led + 2, r, g, b);
	strip.show();
}

void SetAllTiles()
{
	char *arg;
	arg = SCmd.next();

	String argString = String(arg);
	if (arg != NULL && argString.length() == 144) {
		for (int i = 0; i< 48; i++) {
			int color = argString.substring(i * 3, (i * 3) + 3).toInt();
			int r = colorindex[color].r;
			int g = colorindex[color].g;
			int b = colorindex[color].b;

			strip.setPixelColor(i * 3, r, g, b);
			strip.setPixelColor((i * 3) + 1, r, g, b);
			strip.setPixelColor((i * 3) + 2, r, g, b);
		}
		strip.show();
	}
}

void SendMsg(String myStr) {
	Serial.write(myStr.length());
	Serial.write(myStr.c_str());
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

// This gets set as the default handler, and gets called when no other command matches. 
void unrecognized()
{
	SendMsg("Cant recognize command.");
}

void PrintFreeRam() {
	int space = freeRam();
	SendMsg("Free space between the heap and the stack is: " + String(space) + " bytes");
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
