#ifndef CONTROLLER_H
#define CONTROLLER_H

#include <windows.h>
#include <Hidsdi.h>

#define CHECK_NULL(ptr, label) \
if (ptr == NULL) \
{ \
	goto label; \
}\

#define CHECK_HID(value, label)\
if (value != HIDP_STATUS_SUCCESS)\
{\
	goto label; \
}\

#define CHECK_FAILURE(value, label)\
if (!value)\
{\
	goto label; \
}\

class Controller
{
public:
	//constructor for the controller object
	Controller();
	
	//destructor for the controller object
	~Controller();
	
	//registers a particular HID Controller to be read given the device ID
	bool registerController(int pDeviceId);
	
	//gets the DPad Input, storing it as an array of 4 booleans (up, down, left, right)
	int getDPadInput(bool * pValues);
	
	//gets the button usages input, storing it as an array of 4 booleans (A, B, Start, Select)
	int getButtonInput(bool * pValues);

	//clears hid structures
	void clearHidStructures();
private:
	//handle to the controller
	HANDLE _controllerHandle;

	//data structure representing the hid device capabilities
	PHIDP_CAPS _pCaps;

	//data structure representing the hid device button capabilities
	PHIDP_BUTTON_CAPS _pButtonCaps;

	//data structure representing the device value capabilities
	PHIDP_VALUE_CAPS _pValueCaps;

	//pointer to os-structured data structure from HID device
	PHIDP_PREPARSED_DATA _pPreparsedData;

	//data structure holding button usages of device
	PUSAGE _pButtonUsages;

	//data structure holding an input report from the hid device
	PCHAR _pInputReport;

	//flag to hold function success or failure flags
	NTSTATUS _succ;
};


#endif