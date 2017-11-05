#include "windows.h"
#include <stdlib.h>
#include <locale.h>
#include <iostream>
#include <string>
#include <tchar.h>
#include <ctime>
#include <vector>
#include <map>

// Имя файла второго процесса
const char EXE_NAME[] = "Lab4-Threads1-child.exe";

const int
	NUM_PROCESSES = 5,	 // Кол-во процессов
	MIN_TIMEOUT = 500,	 // Минимальное время ожидания процессов
	MAX_TIMEOUT = 6000;  // Максимальное время ожидания процессов

// Необходимая информация о процессе
struct Proc {
	int status;	// Код ошибки при создании (0 - успешно)
	int id; 
	HANDLE handle; // хэндл ивента для закрытия процесса
	PROCESS_INFORMATION info; // информация о процессе
	bool opened; // Открыт ли процесс
};

// Имена ивентов
// Сишные функции не принимают сппшные строки
const char* hKillRandomName = "lab4_killRandom";
const char* hCloseName = "lab4_closed";
std::string hKillNameDefault = "lab4_kill";

// Создает ивент и возвращает хэндл, либо завершает программу при неудаче
HANDLE createEvent(const char eventName[], int errCode) {
	HANDLE handle = CreateEvent(NULL, TRUE, FALSE, eventName);
	if (!handle) {
		std::cout << "Failed to open event " << eventName << std::endl;
		CloseHandle(handle);
		system("pause");
		exit(errCode);
	}
	return handle;
}

int getRandomBetween(int min, int max){
	int n = max - min + 1;
	int remainder = RAND_MAX % n;
	int x;
	do{
		x = rand();
	} while (x >= RAND_MAX - remainder);
	return min + x % n;
}

// Создает процесс и возвращает информацию о нем
Proc createProcess(int i) {

	// id процесса
	std::string pid = std::to_string(i);
	int id = i + 1;

	// Открываем ивент закрытия для процесса
	std::string hKillNameStr = hKillNameDefault;
	char* hKillName = const_cast<char*>(hKillNameStr.append(pid).c_str());
	HANDLE hKill = CreateEvent(NULL, TRUE, FALSE, hKillName);
	if (!hKill) {
		std::cout << "Failed to create event " << hKillName << std::endl;
		return { 3, id, hKill, NULL, false};
	}

	// Заголовок процесса
	STARTUPINFO startInfo = {};
	std::string title = "Process ";
	title.append(std::to_string(id));
	startInfo.lpTitle = const_cast<char*>(title.c_str());

	// Задержка перед закрытием процесса
	int delay = getRandomBetween(MIN_TIMEOUT, MAX_TIMEOUT)*10;

	// Параметры, передаваемые процессу (id, delay, hKill, hKillRandom, hClose) 
	std::string paramString = pid;
	paramString += ' ';
	paramString.append(std::to_string(delay));
	paramString += ' ';
	paramString.append(hKillName);
	paramString += ' ';
	paramString.append(hKillRandomName);
	paramString += ' ';
	paramString.append(hCloseName);
	char* params = const_cast<char*>(paramString.c_str());

	// Информация о процессе
	PROCESS_INFORMATION procInfo;

	// Создаем процесс, обрабатываем ошибку при создании
	if (
		!CreateProcess(
			EXE_NAME, params,
			NULL, NULL, FALSE, CREATE_NEW_CONSOLE, NULL, NULL,
			&startInfo, &procInfo
		)
	) {
		std::cout << "Failed to create process " << id << std::endl;
		return { 4, id, hKill, procInfo, false};
	}

	std::cout << title << std::endl;
	std::cout << params << std::endl;

	// Возвращаем всю необходимую информацию о процессе
	return { 0, id, hKill, procInfo, true};
}

// Возвращает индекс случайного процесса с opened == true
int getRandomAliveProcessIndex(const std::vector<Proc> processes) {
	std::vector<int> processesAlive;

	// Находим все открытые процессы
	for (size_t i = 0; i < processes.size(); i++) {
		if (processes[i].opened) {
			processesAlive.push_back(i);
		}
	}

	// Возвращаем случайный или первый, если все закрыты
	if (processesAlive.size() > 0) {
		return processesAlive[getRandomBetween(0, processesAlive.size() - 1)];
	}
	else {
		return 0;
	}
}

int main(int argc, wchar_t* argv[])
{
	int errorCode = 0; // Код ошибки

	// Здесь будет информация о процессах
	std::vector<Proc> processes;

	// Ивенты и хэндлы уничтожения и закрытия процессов
	const int numMainHandles = 2;
	HANDLE hKillRandom = createEvent(hKillRandomName, 1);
	HANDLE hClose = createEvent(hCloseName, 2);
	HANDLE mainHandles[numMainHandles] = { hKillRandom, hClose };

	int processesOpened = 0; // Кол-во открытых процессов

	// Запускаем процессы
	for (int i = 0; i < NUM_PROCESSES; i++) {

		Proc process = createProcess(i);
		
		// Проверяем на ошибки при запуске
		errorCode = process.status;
		if (errorCode != 0) {
			break;
		}

		// Добавляем информацию о процессе в массив и считаем его открытым
		processes.push_back(process);
		processesOpened++;
	}

	// Ждем событий от процессов, пока хотя бы один процесс открыт
	while (processesOpened > 0) {
		int waitStatus = WaitForMultipleObjects(2, mainHandles, false, INFINITE);

		// Обрабатываем события
		if (waitStatus < 0 || waitStatus >= numMainHandles) {
			std::cout << "Error when waiting for response " << waitStatus << std::endl;
			continue;
		}

		// Процесс хочет закрыть другой процесс
		if (mainHandles[waitStatus] == hKillRandom) {

			// Находим случайный открытый процесс
			Proc& process = processes[getRandomAliveProcessIndex(processes)];
			if (process.opened) {

				// Закрываем его
				// Если он уже закрыл другой процесс, то он пришлет сообщение о своем закрытии
				// В противном случае он это сделает после закрытия другого процесса
				process.opened = false;
				SetEvent(process.handle);

				std::cout << "Closing process " << process.id << std::endl;
			}
			else {
				std::cout << "All processes already closing or closed" << std::endl;
			}

			// Ресетим ивент закрытия случайного процесса
			ResetEvent(hKillRandom);
		}
		// Процесс сообщил, что закрылся, с этого момента он нам не важен
		else if(mainHandles[waitStatus] == hClose){

			// Уменьшаем кол-во открытых процессов
			processesOpened--;

			// Ресетим ивент закрытия процесса
			ResetEvent(hClose);

			std::cout << "A process closed" << std::endl;
		}		
	}

	// Закрываем все основные хэндлы
	for (size_t i = 0; i < numMainHandles; i++) {
		CloseHandle(mainHandles[i]);
	}

	// Закрываем все хэндлы процессов
	for (size_t i = 0; i < processes.size(); i++) {
		CloseHandle(processes[i].handle);
		if (processes[i].status == 0) {
			CloseHandle(processes[i].info.hProcess);
			CloseHandle(processes[i].info.hThread);
		}
	}

	system("pause");
	return errorCode;
}

