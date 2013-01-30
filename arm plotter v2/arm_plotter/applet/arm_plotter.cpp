#include <Servo.h>

//#define callibration  //uncoment this to make callibration

//values from excel
#define aS1 90  
#define bS1 380

#define aS2 91
#define bS2 1091
//////////////////////////////////////////

#define led0 13
#define Servo1Pin 17
#define Servo2Pin 18
#define Servo3Pin 19

#include "WProgram.h"
void setup();
void loop();
void attachServos();
Servo servo1;
Servo servo2;
Servo servo3;

byte s1=45;
byte s2=45;
byte s3=90;

byte DELAY=10;
boolean inProgress=false;

int t;

//#include <LiquidCrystal.h>
//LiquidCrystal lcd(12, 8, 7, 6, 5, 4, 2);

void setup()
{
  // lcd.begin(16,2);
  Serial.begin(115200);
  pinMode(led0,OUTPUT);
  attachServos();
}

void loop()
{

  digitalWrite(led0,inProgress);

  if(Serial.available())
  {

    if (Serial.read()==0xAA)
    {
      do{
      }
      while(Serial.available()<=4);
      //serwa
#ifdef callibration
      s1=Serial.read();
      s2=Serial.read();
#else
      s1=(Serial.read()*100+bS1)/aS1;
      s2=(Serial.read()*100+bS2)/aS2;
#endif
      s3=Serial.read();
      DELAY=Serial.read();
      inProgress=true;
    }
  }


  if (servo1.read()>s1) servo1.write(servo1.read()-1);
  if (servo1.read()<s1) servo1.write(servo1.read()+1);

  if (servo2.read()>s2) servo2.write(servo2.read()-1);
  if (servo2.read()<s2) servo2.write(servo2.read()+1);

  if (servo3.read()>s3) servo3.write(servo3.read()-1);
  if (servo3.read()<s3) servo3.write(servo3.read()+1);


  if (inProgress==true && servo1.read()==s1 && servo2.read()==s2 && servo3.read()==s3)
  {
    Serial.print(1,BYTE);
    inProgress=false;
    t=0;
  }

  delay(DELAY);

  t++;
  if (t==200) 
  {
    Serial.print(1,BYTE);
    inProgress=false;
    t=0;
  }

  /*
        lcd.clear();
   lcd.print(servo1.read(),DEC);
   lcd.print("-");
   lcd.print(s1,DEC);
   lcd.print(" ");
   lcd.print(servo2.read(),DEC);
   lcd.print("-");
   lcd.print(s2,DEC);
   lcd.setCursor(0,1);
   lcd.print(servo3.read(),DEC);
   lcd.print("-");
   lcd.print(s3,DEC);
   lcd.print(" ");
   lcd.print(DELAY,DEC);
   lcd.print(" ");
   */

}





/////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
void attachServos()
{

  servo1.attach(Servo1Pin);
  servo2.attach(Servo2Pin);
  servo3.attach(Servo3Pin);
}








int main(void)
{
	init();

	setup();
    
	for (;;)
		loop();
        
	return 0;
}

