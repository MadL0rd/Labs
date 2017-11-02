// Lab4_2.c : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <stdlib.h>
#include "windows.h"
#include <locale.h>

int _tmain(int argc, _TCHAR* argv[])
{
	HANDLE hKillBEvent, hPrintEvent;
	HANDLE Handles[2];
	int waitResult;
	int count = 0;

	setlocale(LC_ALL,"Rus");
	hKillBEvent = OpenEvent(EVENT_ALL_ACCESS, FALSE, _T("LAB2_KillBEvent"));
	if (!hKillBEvent) {
		printf("%s", "Не удалось открыть событие KillBEvent.\n");
		getchar();
		return 1;
	}
	hPrintEvent = OpenEvent(EVENT_ALL_ACCESS, FALSE, _T("LAB2_PrintEvent"));
	if (!hPrintEvent) {
		printf("%s", "Не удалось открыть событие PrintEvent.\n");
		//errCode = 2;
		getchar();
		return 2;
	}

	Handles[0] = hKillBEvent;
	Handles[1] = hPrintEvent;
	while (TRUE) {
		waitResult = WaitForMultipleObjects(2, Handles, FALSE, INFINITE);
		if (waitResult == 0) {
			break;
		}
		printf("%d\n", ++count);
		Sleep(1000);
	}

	CloseHandle(hKillBEvent);
	CloseHandle(hPrintEvent);
	return 0;
}

