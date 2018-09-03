/*
* Сделано на основе https://msdn.microsoft.com/en-us/library/ms682499.aspx
*/
#pragma warning(disable : 4996)  

#include <windows.h>
#include <stdio.h>
#include <ctime>

const int BUFSIZE_IN = 1;
const int BUFSIZE_OUT = 5;

const int MIN_DELAY = 500;
const int MAX_DELAY = 1500;

// Возвращает случайное число между min и max
int getRandomBetween(int min, int max) {
	int n = max - min + 1;
	int remainder = RAND_MAX % n;
	int x;
	do {
		x = rand();
	} while (x >= RAND_MAX - remainder);
	return min + x % n;
}

int main(int argc, char *argv[]){

	// Создаем сид для rand() из id процесса и текущего времени
	char *endChar;
	int pid = strtol(argv[0], &endChar, 10);
	srand((int)time(NULL) + pid);

	char inBuf[BUFSIZE_IN];   // Буффер для входящих данных
	char outBuf[BUFSIZE_OUT]; // Буфер для исходящих данных
	unsigned long inputLen, writeLen;  // Длина считанных и записанных данных
	HANDLE stdinHandle, stdoutHandle;  // Хэндлы для считывания и записи данных
	bool success; // Были ли данные успешно считаны

	// Получаем хэндлы для считывания и записи
	stdoutHandle = GetStdHandle(STD_OUTPUT_HANDLE);
	stdinHandle = GetStdHandle(STD_INPUT_HANDLE);
	if (stdoutHandle == INVALID_HANDLE_VALUE || stdinHandle == INVALID_HANDLE_VALUE) {
		ExitProcess(1);
	}

	// Читаем из перенаправленного стандартного ввода
	while (true) {
		printf("Waiting for input...\n");
		success = ReadFile(stdinHandle, inBuf, BUFSIZE_IN, &inputLen, NULL);

		// Мы что-то прочитали
		if (success && inputLen != 0 && inBuf[0] != -1) {

			// Преобразовываем прочтенное число в строку
			itoa(inBuf[0], outBuf, 10);
			int len = strlen(outBuf);
			outBuf[len] = '\n';
			outBuf[len + 1] = '\0';

			// Выводим строку в окно этого процесса
			WriteFile(stdoutHandle, outBuf, len + 1, &writeLen, NULL);
		}
		// Сигнал того, что нужно перестать читать
		else if (inBuf[0] == -1) {
			printf("Closing\n");
			break;
		}

		// Спим случайное время
		int delay = getRandomBetween(MIN_DELAY, MAX_DELAY);
		printf("Sleeping for %dms\n", delay);
		Sleep(delay);
	}

	CloseHandle(stdoutHandle);
	CloseHandle(stdinHandle);

	return 0;
}
