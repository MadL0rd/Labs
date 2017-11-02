// Lab5_A.c : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "windows.h"
#include <stdlib.h>
#include <io.h>
#include <Fcntl.h>
#include <locale.h>

#pragma warning(disable: 4996)

int _tmain(int argc, _TCHAR* argv[])
{
	int errCode;
	char *strToSend, answer[250];
	BOOL procBRuns;
	HANDLE hReadPipe, hWritePipe, hMapping, hDataSentEvent, hAnswerEvent;
	FILE *readPipeFile;
	STARTUPINFO startInfo = {sizeof(startInfo)};
	PROCESS_INFORMATION procInfo;
	SECURITY_ATTRIBUTES pipeAttributes = {sizeof(SECURITY_ATTRIBUTES), NULL, TRUE};

	SetWindowText(GetForegroundWindow(), _T("Процесс A"));

//	setlocale(LC_ALL,"Rus"); // - это не решает проблему ввода кириллицы!
	// Внимание! Для правильного ввода/вывода символов кириллицы
	// следует в свойствах окна консоли выбрать шрифт Lucida Console
    SetConsoleCP(1251);			// установка кодовой страницы win-cp 1251 в поток ввода
    SetConsoleOutputCP(1251);	// установка кодовой страницы win-cp 1251 в поток вывода

	// Создание объекта "отображение файла" и представления strToSend
	hMapping = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, 4096, _T("Lab5_Mapping"));
	strToSend = MapViewOfFile(hMapping, FILE_MAP_WRITE, 0, 0, 250);

	// Создание событий для синхронизации обмена с процессом B
	hDataSentEvent = CreateEvent(NULL, FALSE, FALSE, _T("Lab5_SentEvent"));
	hAnswerEvent = CreateEvent(NULL, FALSE, FALSE, _T("Lab5_AnswEvent"));

	// Создание безымянного канала
	CreatePipe(&hReadPipe, &hWritePipe, &pipeAttributes, 0);

	// Создание объекта языка C типа FILE для работы с каналом с помощью функцмй C
	readPipeFile = _fdopen((HFILE)_open_osfhandle((intptr_t)hReadPipe, _O_TEXT | _O_RDONLY), "rt");

	// Создание процесса-потомка B с перенаправленным выводом
	startInfo.lpTitle = _T("Процесс B");
	startInfo.dwFlags = STARTF_USESTDHANDLES;
	startInfo.hStdInput = GetStdHandle(STD_INPUT_HANDLE);
	startInfo.hStdOutput = hWritePipe;
	startInfo.hStdError = GetStdHandle(STD_ERROR_HANDLE);
	procBRuns = CreateProcess(_T("..\\Debug\\Lab5_B.exe"), NULL, NULL, NULL, TRUE, 
		CREATE_NEW_CONSOLE, NULL, NULL, &startInfo, &procInfo);
	// Закрытие ненужного хэндла вывода в канал
	CloseHandle(hWritePipe);

	// Обработка ошибки создания процесса B
	if (!procBRuns) {
		errCode = 1;
		printf("%s", "Не удалось запустить процесс B\n");
		getchar();
		return 1;
	}

	// Ожидание готовности канала к работе от процесса B и прием начального "мусора" из канала
	WaitForSingleObject(hAnswerEvent, INFINITE);
	fgets(answer, 200, readPipeFile);

	// Основной цикл посылки/приема данных
	do {
		printf("%s", "Введите строку: ");
		gets(strToSend);
		if (strcmp(strToSend, ""))
			strcat(strToSend, " : Process A");

		// Извещение для B - данные в представлении strToSend готовы
		SetEvent(hDataSentEvent);

		// Прием строки из канала
		fgets(answer, 200, readPipeFile);
		// Функция fgets не удаляет символ перевода строки. Удалим его сами.
		answer[strlen(answer) - 1] = '\0';
		printf("Получено от B: %s\n", answer);
		printf("===============================================\n\n");
	} while (strcmp(answer, ""));

	// Закрытие представления strToSend
	UnmapViewOfFile(strToSend);

	// Закрытие хэндла объекта
	CloseHandle(hMapping);
	return 0;
}
