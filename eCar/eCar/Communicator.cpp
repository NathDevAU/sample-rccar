// Communicator.cpp : a wrapper for setup and management of UDP communication

#include "Communicator.h"

#include <winsock.h>

SOCKET partner_socket;
WSADATA wsdata;
sockaddr_in server;

Communicator::Communicator()
{

}

Communicator::~Communicator()
{
}

/*
	Starts the required Windows conection

	Returns 1 for _success, 0 for failure
*/
int Communicator::startWindowsConnection()
{
	WORD wVersionRequested = MAKEWORD(2, 2);
	if (WSAStartup(wVersionRequested, &wsdata) != 0)
	{
		return 0;
	}
	return 1; //1 for _success
}

int Communicator::closeWindowsConnection()
{
	closesocket(partner_socket);
	return WSACleanup();
}

/*
Opens a UDP Socket

-1 for failure, else returns the sd
*/
int Communicator::openUDPSocket()
{
	partner_socket = socket(AF_INET, SOCK_DGRAM, 0);
	if (partner_socket == INVALID_SOCKET)
	{
		//cleanup windows connection and return 0
		WSACleanup();

		return -1;
	}
	return 1; //1 for _success
}

/*
Setup server details and bind
*/
int Communicator::setupServerAndBind()
{
	hostent * host;

	host = gethostbyname(host_name);

	if (host == NULL)
	{
		//failed hostname lookup
		WSACleanup();
		return 0;
	}

	/* Set family and port */
	server.sin_family = AF_INET;
	server.sin_port = htons(PORT_NUMBER);
	server.sin_addr.S_un.S_un_b.s_b1 = host->h_addr_list[0][0];
	server.sin_addr.S_un.S_un_b.s_b2 = host->h_addr_list[0][1];
	server.sin_addr.S_un.S_un_b.s_b3 = host->h_addr_list[0][2];
	server.sin_addr.S_un.S_un_b.s_b4 = host->h_addr_list[0][3];
	
	if (bind(partner_socket, (struct sockaddr *)&server, sizeof(server)))
	{
		int se;
		se = WSAGetLastError();

		WSACleanup();
		closesocket(partner_socket);
		return 0;
	}
	
	
	return 1;
};

/*
Receives udp bytes and returns the command code
*/
Command Communicator::receiveCommand()
{
	
	char * bytes = new char[1];
	int bytecount = -1;
	bytecount = recv(partner_socket, bytes, 1, 0);
	
	if (bytecount == -1)
	{
		return STOP;
	}
	
	switch (bytes[0])
	{
	case '1' :
		return FORWARD;
		break;
	case '2' :
		return RIGHT;
		break;
	case '3' :
		return BACKWARD;
		break;
	case '4':
		return LEFT;
		break;
	case '5':
		return STOP_FORWARD;
		break;
	case '6':
		return STOP_RIGHT;
		break;
	case '7':
		return STOP_BACKWARD;
		break;
	case '8':
		return STOP_LEFT;
		break;
	case '9':
		return SWITCH_COMMAND_MODE;
		break;
	default:
		return STOP;

	}
	return STOP;
}
