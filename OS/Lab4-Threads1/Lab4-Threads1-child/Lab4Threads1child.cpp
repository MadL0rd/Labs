#include "windows.h"
#include <stdlib.h>
#include <locale.h>
#include <iostream>
#include <string>
#include <tchar.h>
#include <ctime>

// ������� � ���������� ����� ��� ������
HANDLE openEvent(const char eventName[], int errCode) {
	HANDLE handle = OpenEvent(EVENT_ALL_ACCESS, FALSE, eventName);
	if (!handle) {
		std::cout << "Failed to open event " << eventName << std::endl;
		system("pause");
		exit(errCode);
	}
	return handle;
}

int main(int argc, const char* argv[])
{
	// id ��������
	int pid = atoi(argv[0]);

	// ����� ���
	int delay = atoi(argv[1]);

	// �����, ���������� ����� ��������, ��� ����� �����������
	HANDLE killSelfHandler = openEvent(argv[2], 1);  
	// �����, ���������� ��������� ��������, ��� ����� ������� ��������� �������
	HANDLE killOtherNotify = openEvent(argv[3], 2); 
	// �����, ���������� ��������� ��������, ��� ���� ������� ��������
	HANDLE closedNotify = openEvent(argv[4], 3);

	// ���� ��������� �����
	std::cout << "Sleeping " << (double(delay) / 1000) << "sec" << std::endl;
	Sleep(delay);

	// ������� ������ �������
	// ���� ����� ���� ���� �������� ������� �������� ���������� ��������
	std::cout << "Killing random process" << std::endl;
	while (true) {
		if (WaitForSingleObject(killOtherNotify, 0) != WAIT_OBJECT_0) {
			SetEvent(killOtherNotify);
			break;
		}
	}	

	// ����, ���� ���� ������� �����
	std::cout << "Waiting to be killed" << std::endl;
	while (true) {
		if (WaitForSingleObject(killSelfHandler, INFINITE) == WAIT_OBJECT_0) {
			std::cout << "Killed" << std::endl;
			break;
		}
	}

	// ��������� ������ �������
	CloseHandle(killSelfHandler);
	CloseHandle(killOtherNotify);

	// ���� ������� ����� �������
	system("pause");

	// �������� ��������� ��������, ��� �� �����������
	while (true) {
		if (WaitForSingleObject(closedNotify, 0) != WAIT_OBJECT_0) {
			SetEvent(closedNotify);
			break;
		}
	}

	// ��������� ����� ������ ��������
	CloseHandle(closedNotify);

	// �����������
	return 0;
}

