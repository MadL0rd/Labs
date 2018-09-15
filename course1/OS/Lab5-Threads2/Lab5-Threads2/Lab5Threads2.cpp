/*
* Сделано на основе https://msdn.microsoft.com/en-us/library/ms682499.aspx
*/

#include <windows.h> 
#include <tchar.h>
#include <stdio.h> 
#include <strsafe.h>
#include <ctime>

const int BUFSIZE = 1;

const int START_INT = 1;
const int END_INT = 100;

const char EXE_NAME[] = "Lab5-Threads2-child.exe";

HANDLE childStdInReadHandle = NULL;
HANDLE childStdInWriteHandle = NULL;

void createChildProcess(PROCESS_INFORMATION&, STARTUPINFO&);
void writeToPipe(int);
void errorExit(const char* text);

int main(int argc, char *argv[]){

	srand((int)time(NULL));

	// Свойства канала
	SECURITY_ATTRIBUTES saAttr;
	saAttr.nLength = sizeof(SECURITY_ATTRIBUTES);
	saAttr.bInheritHandle = TRUE;
	saAttr.lpSecurityDescriptor = NULL;

	// Создаем безымянный канал, который будет заменять stdin у созданных процессов
	if (!CreatePipe(&childStdInReadHandle, &childStdInWriteHandle, &saAttr, 0)) {
		errorExit("Failed to create pipe");
	}

	// Создаваемые процессы будут наследовать только канал для чтения
	if (!SetHandleInformation(childStdInWriteHandle, HANDLE_FLAG_INHERIT, 0)) {
		errorExit("Failed to set handle information");
	}

	// Информация о процессах
	PROCESS_INFORMATION procInfo1 = {}, procInfo2 = {};

	// Настройки процессов
	STARTUPINFO startInfo1 = {}, startInfo2 = {};
	startInfo1.lpTitle = "Child Process 1";
	startInfo2.lpTitle = "Child Process 2";

	// Создаем процессы
	createChildProcess(procInfo1, startInfo1);
	createChildProcess(procInfo2, startInfo2);

	// Пишем числа в канал
	for (int i = START_INT; i <= END_INT; i++) {
		printf("Writing %d\n", i);
		writeToPipe(i);
		Sleep(1000);
	}

	// Сообщаем процессам, что нужно завершить работу
	writeToPipe(-1);
	writeToPipe(-1);

	// Закрываем хэндлы ввода\вывода
	CloseHandle(childStdInReadHandle);
	CloseHandle(childStdInWriteHandle);

	// Закрываем хэндлы процессов
	CloseHandle(procInfo1.hProcess);
	CloseHandle(procInfo1.hThread);
	CloseHandle(procInfo2.hProcess);
	CloseHandle(procInfo2.hThread);

	printf("Finished writing\n");
	system("pause");

	return 0;
}

// Создает процесс
void createChildProcess(PROCESS_INFORMATION& procInfo, STARTUPINFO& startInfo){
	
	startInfo.cb = sizeof(STARTUPINFO);

	// Включаем перенаправление каналов
	startInfo.dwFlags |= STARTF_USESTDHANDLES;

	// Перенаправляем стандартный канал вывода процесса в созданный нами канал
	startInfo.hStdInput = childStdInReadHandle;

	// Передаем случаное id процессом для рандомизации времени ожидания
	char params[32];
	sprintf(params, "%d", rand());

	// Создаем процесс
	if (!CreateProcess(
		EXE_NAME, // имя файла
		params,   // коммандная строка
		NULL,     // security attributes процесса
		NULL,     // security attributes треда
		TRUE,     // включаем наследование хэндлов
		CREATE_NEW_CONSOLE, // создаем новое окно
		NULL,     // процесс оперирует в среде родительского процесса
		NULL,     // процесс оперирует в директории родительского процесса
		&startInfo,
		&procInfo
	)) {
		errorExit("Failed to create process");
	}

	printf("Created %s\n", startInfo.lpTitle);
}

// Отправляет i дочерним процессам
void writeToPipe(int i){
	unsigned long writeLen;

	// Создаем буффер для записи
	char chBuf[BUFSIZE] = { char(i) };

	// Мы будем писать в stdin созданного канала
	if (!WriteFile(childStdInWriteHandle, chBuf, BUFSIZE, &writeLen, NULL)) {
		errorExit("Failed to write to stdout");
	}
}

// Выводит ошибку и завершает процесс
void errorExit(const char* text){
	printf(text);
	system("pause");
	ExitProcess(1);
}
