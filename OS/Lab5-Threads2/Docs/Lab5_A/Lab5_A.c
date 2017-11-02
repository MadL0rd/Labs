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

	SetWindowText(GetForegroundWindow(), _T("������� A"));

//	setlocale(LC_ALL,"Rus"); // - ��� �� ������ �������� ����� ���������!
	// ��������! ��� ����������� �����/������ �������� ���������
	// ������� � ��������� ���� ������� ������� ����� Lucida Console
    SetConsoleCP(1251);			// ��������� ������� �������� win-cp 1251 � ����� �����
    SetConsoleOutputCP(1251);	// ��������� ������� �������� win-cp 1251 � ����� ������

	// �������� ������� "����������� �����" � ������������� strToSend
	hMapping = CreateFileMapping(INVALID_HANDLE_VALUE, NULL, PAGE_READWRITE, 0, 4096, _T("Lab5_Mapping"));
	strToSend = MapViewOfFile(hMapping, FILE_MAP_WRITE, 0, 0, 250);

	// �������� ������� ��� ������������� ������ � ��������� B
	hDataSentEvent = CreateEvent(NULL, FALSE, FALSE, _T("Lab5_SentEvent"));
	hAnswerEvent = CreateEvent(NULL, FALSE, FALSE, _T("Lab5_AnswEvent"));

	// �������� ����������� ������
	CreatePipe(&hReadPipe, &hWritePipe, &pipeAttributes, 0);

	// �������� ������� ����� C ���� FILE ��� ������ � ������� � ������� ������� C
	readPipeFile = _fdopen((HFILE)_open_osfhandle((intptr_t)hReadPipe, _O_TEXT | _O_RDONLY), "rt");

	// �������� ��������-������� B � ���������������� �������
	startInfo.lpTitle = _T("������� B");
	startInfo.dwFlags = STARTF_USESTDHANDLES;
	startInfo.hStdInput = GetStdHandle(STD_INPUT_HANDLE);
	startInfo.hStdOutput = hWritePipe;
	startInfo.hStdError = GetStdHandle(STD_ERROR_HANDLE);
	procBRuns = CreateProcess(_T("..\\Debug\\Lab5_B.exe"), NULL, NULL, NULL, TRUE, 
		CREATE_NEW_CONSOLE, NULL, NULL, &startInfo, &procInfo);
	// �������� ��������� ������ ������ � �����
	CloseHandle(hWritePipe);

	// ��������� ������ �������� �������� B
	if (!procBRuns) {
		errCode = 1;
		printf("%s", "�� ������� ��������� ������� B\n");
		getchar();
		return 1;
	}

	// �������� ���������� ������ � ������ �� �������� B � ����� ���������� "������" �� ������
	WaitForSingleObject(hAnswerEvent, INFINITE);
	fgets(answer, 200, readPipeFile);

	// �������� ���� �������/������ ������
	do {
		printf("%s", "������� ������: ");
		gets(strToSend);
		if (strcmp(strToSend, ""))
			strcat(strToSend, " : Process A");

		// ��������� ��� B - ������ � ������������� strToSend ������
		SetEvent(hDataSentEvent);

		// ����� ������ �� ������
		fgets(answer, 200, readPipeFile);
		// ������� fgets �� ������� ������ �������� ������. ������ ��� ����.
		answer[strlen(answer) - 1] = '\0';
		printf("�������� �� B: %s\n", answer);
		printf("===============================================\n\n");
	} while (strcmp(answer, ""));

	// �������� ������������� strToSend
	UnmapViewOfFile(strToSend);

	// �������� ������ �������
	CloseHandle(hMapping);
	return 0;
}
