/**
 * Communicator is a wrapper for setup and management of UDP communication
 **/

#ifndef COMMUNICATOR_H
#define COMMUNICATOR_H

#include "Command.h"
class Communicator
{
	const unsigned short PORT_NUMBER = 10001;
	const char * host_name = "HOSTNAME";
public:
	Communicator();
	~Communicator();
	/*
	Starts the required Windows conection
	*/
	int startWindowsConnection();

	/*
	Close the windows connection
	*/
	int closeWindowsConnection();

	/*
	Opens a UDP Socket
	*/
	int openUDPSocket();

	/*
	Setup server details and bind
	*/
	int setupServerAndBind();

	/*
	Receives udp bytes and returns the command code
	*/
	Command receiveCommand();
};


#endif