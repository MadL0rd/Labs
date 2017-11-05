#include "windows.h"
#include <stdlib.h>
#include <locale.h>
#include <iostream>
#include <string>
#include <tchar.h>
#include <ctime>

// Создает и возвращает хэндл для ивента
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
	// id процесса
	int pid = atoi(argv[0]);

	// Время сна
	int delay = atoi(argv[1]);

	// Ивент, сообщающий этому процессу, что нужно закрываться
	HANDLE killSelfHandler = openEvent(argv[2], 1);  
	// Ивент, сообщающий основному процессу, что нужно закрыть случайный процесс
	HANDLE killOtherNotify = openEvent(argv[3], 2); 
	// Ивент, сообщающий основному процессу, что этот процесс закрылся
	HANDLE closedNotify = openEvent(argv[4], 3);

	// Спим указанное время
	std::cout << "Sleeping " << (double(delay) / 1000) << "sec" << std::endl;
	Sleep(delay);

	// Убиваем другой процесс
	// Если нужно ждем пока основной процесс закончит предыдущее убийство
	std::cout << "Killing random process" << std::endl;
	while (true) {
		if (WaitForSingleObject(killOtherNotify, 0) != WAIT_OBJECT_0) {
			SetEvent(killOtherNotify);
			break;
		}
	}	

	// Ждем, пока этот процесс убьют
	std::cout << "Waiting to be killed" << std::endl;
	while (true) {
		if (WaitForSingleObject(killSelfHandler, INFINITE) == WAIT_OBJECT_0) {
			std::cout << "Killed" << std::endl;
			break;
		}
	}

	// Закрываем хэндлы убийств
	CloseHandle(killSelfHandler);
	CloseHandle(killOtherNotify);

	// Ждем нажатия любой клавиши
	system("pause");

	// Сообщаем основному процессу, что мы закрываемся
	while (true) {
		if (WaitForSingleObject(closedNotify, 0) != WAIT_OBJECT_0) {
			SetEvent(closedNotify);
			break;
		}
	}

	// Закрываем хэндл ивента закрытия
	CloseHandle(closedNotify);

	// Закрываемся
	return 0;
}

