// Lab4.c : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "windows.h"
#include <stdlib.h>
#include <locale.h>

int _tmain(int argc, _TCHAR* argv[])
{
	char choice = '\0';
	HANDLE hKillBEvent, hPrintEvent, hProcessB;
	int waitResult, errCode = 0;
	BOOL procBRuns = 0;
	STARTUPINFO startInfo = {sizeof(startInfo)};
	PROCESS_INFORMATION procInfo;

	setlocale(LC_ALL,"Rus");
	startInfo.lpTitle = _T("������� B");
	hKillBEvent = CreateEvent(NULL, FALSE, FALSE, _T("LAB2_KillBEvent"));
	if (!hKillBEvent) {
		printf("%s", "�� ������� ������� ������� KillBEvent.\n");
		getchar();
		return 1;
	}
	hPrintEvent = CreateEvent(NULL, TRUE, FALSE, _T("LAB2_PrintEvent"));
	if (!hPrintEvent) {
		printf("%s", "�� ������� ������� ������� PrintEvent.\n");
		getchar();
		CloseHandle(hKillBEvent);
		return 2;
	}

	do {
		system("cls");
		printf("%s", "� � � � � � �  A\n");
		printf("%s", "================\n");
		printf("%s", "������� �������:\n");
		printf("%s", "   1 - ��������� ������� B\n");
		printf("%s", "   2 - ��������� ������\n");
		printf("%s", "   3 - ���������� ������\n");
		printf("%s", "   4 - ��������� ������� B\n");
		printf("%s", "   0 - ��������� ��\n");
		putchar('>');
		choice = getchar();
		getchar();

		switch (choice) {
			case '1':
				procBRuns = CreateProcess(_T("Lab4_2.exe"), NULL, NULL, NULL, FALSE, CREATE_NEW_CONSOLE, 
					NULL, NULL, &startInfo, &procInfo);
				if (!procBRuns) {
					errCode = 3;
					printf("%s", "�� ������� ��������� ������� B.\n");
					getchar();
				}
				hProcessB = procInfo.hProcess;
				break;
			case '2':
				SetEvent(hPrintEvent);
				break;
			case '3':
				ResetEvent(hPrintEvent);
				break;
			case '4':
			case '0':
				if (procBRuns) {
					SetEvent(hKillBEvent);
					waitResult = WaitForSingleObject(hProcessB, 20000);
					if (waitResult == WAIT_TIMEOUT) {
						printf("%s", "�� ������� ��������� ������� B.\n");
						getchar();
					}
					CloseHandle(procInfo.hProcess);
					CloseHandle(procInfo.hThread);
					ResetEvent(hPrintEvent);
					procBRuns = FALSE;
				}
						//getchar();
				break;
			default:
				break;
		}
	} while (choice != '0' && !errCode);

	CloseHandle(hKillBEvent);
	CloseHandle(hPrintEvent);
	return errCode;
}

