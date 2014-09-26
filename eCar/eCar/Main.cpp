// Main.cpp : Defines the entry point for the console application.
//


#include "stdafx.h"
#include "arduino.h"
#include "Command.h"
#include "Communicator.h"
#include "Controller.h"
#include "nesmapping.h"


const int OUT_UP = 10;
const int OUT_RIGHT = 12;
const int OUT_DOWN = 11;
const int OUT_LEFT = 13;

Controller * controller;
bool * dpadValues;
bool * buttonValues;
bool useNetworkCommands;

Communicator * comm;


int _tmain(int argc, _TCHAR* argv[])
{
	useNetworkCommands = false;
    return RunArduinoSketch();
}

void setup()
{   
	//set motor pins to output mode
	pinMode(OUT_UP, OUTPUT);     
	pinMode(OUT_RIGHT, OUTPUT);
	pinMode(OUT_DOWN, OUTPUT);
	pinMode(OUT_LEFT, OUTPUT);

	//set pin outputs to low
	digitalWrite(OUT_UP, LOW);
	digitalWrite(OUT_RIGHT, LOW);
	digitalWrite(OUT_DOWN, LOW);
	digitalWrite(OUT_LEFT, LOW);

	//setup controller
	controller = new Controller();
	controller->registerController(0);
	dpadValues = (bool *)malloc(sizeof(int)* 4);
	buttonValues = (bool *)malloc(sizeof(int)* 4);

	//setup network communications
	comm = new Communicator();
	comm->startWindowsConnection();
	comm->openUDPSocket();
	comm->setupServerAndBind();
}


// the loop routine runs over and over again forever:
void loop()
{
	if (useNetworkCommands)
	{
		switch (comm->receiveCommand())
		{
		case FORWARD:
			digitalWrite(OUT_UP, HIGH);
			digitalWrite(OUT_DOWN, LOW);
			break;
		case RIGHT:
			digitalWrite(OUT_RIGHT, HIGH);
			digitalWrite(OUT_LEFT, LOW);
			break;
		case BACKWARD:
			digitalWrite(OUT_DOWN, HIGH);
			digitalWrite(OUT_UP, LOW);
			break;
		case LEFT:
			digitalWrite(OUT_LEFT, HIGH);
			digitalWrite(OUT_RIGHT, LOW);
			break;
		case STOP_FORWARD:
			digitalWrite(OUT_UP, LOW);
			break;
		case STOP_RIGHT:
			digitalWrite(OUT_RIGHT, LOW);
			break;
		case STOP_BACKWARD:
			digitalWrite(OUT_DOWN, LOW);
			break;
		case STOP_LEFT:
			digitalWrite(OUT_LEFT, LOW);
			break;
		case SWITCH_COMMAND_MODE:
			useNetworkCommands = !useNetworkCommands;
			break;
		default:
			digitalWrite(OUT_UP, LOW);
			digitalWrite(OUT_RIGHT, LOW);
			digitalWrite(OUT_DOWN, LOW);
			digitalWrite(OUT_LEFT, LOW);
		}
	}else
	{
		controller->getDPadInput(dpadValues);
		controller->getButtonInput(buttonValues);
		if (dpadValues[DPAD_UP_INDEX]){
			digitalWrite(OUT_DOWN, LOW);
			digitalWrite(OUT_UP, HIGH);
		}
		else if (dpadValues[DPAD_DOWN_INDEX]){
			digitalWrite(OUT_UP, LOW);
			digitalWrite(OUT_DOWN, HIGH);
		}
		else{
			digitalWrite(OUT_UP, LOW);
			digitalWrite(OUT_DOWN, LOW);
		}

		if (dpadValues[DPAD_LEFT_INDEX]){
			digitalWrite(OUT_RIGHT, LOW);
			digitalWrite(OUT_LEFT, HIGH);	
		}
		else if (dpadValues[DPAD_RIGHT_INDEX]){
			digitalWrite(OUT_LEFT, LOW);
			digitalWrite(OUT_RIGHT, HIGH);
		}
		else{
			digitalWrite(OUT_LEFT, LOW);
			digitalWrite(OUT_RIGHT, LOW);
		}

		if (buttonValues[BUTTON_SELECT_INDEX]){
			useNetworkCommands = true;
		}
	}
	
	
}