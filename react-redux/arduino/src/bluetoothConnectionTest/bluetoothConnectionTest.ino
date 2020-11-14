#include <SoftwareSerial.h>

SoftwareSerial SerialBt(2, 3);
void setup()
{
    Serial.begin(115200);
    SerialBt.begin(115200);
}

void loop()
{
    if (SerialBt.available()) {
        Serial.write(SerialBt.read());
    }

    if (Serial.available()) {
        SerialBt.write(Serial.read());
    }
}
