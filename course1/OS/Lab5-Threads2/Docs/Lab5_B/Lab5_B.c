// Lab5_B.c : Defines the entry point for the console application.
//

#include "stdafx.h"
#include "windows.h"
#include <stdlib.h>
#include <locale.h>

#pragma warning(disable: 4996)

int _tmain(int argc, _TCHAR* argv[])
{
	char *strReceived;
	HANDLE hMapping, hReceivedEvent, hAnswerEvent;

	// �������� ������� "����������� �����" � �������� ������������� strReceived
	hMapping = OpenFileMapping(FILE_MAP_WRITE, FALSE, _T("Lab5_Mapping"));
	strReceived = MapViewOfFile(hMapping, FILE_MAP_WRITE, 0, 0, 250);

	// �������� ������� ��� ������������� ������ � ��������� A
	hReceivedEvent = OpenEvent(EVENT_ALL_ACCESS, FALSE, _T("Lab5_SentEvent"));
	hAnswerEvent = OpenEvent(EVENT_ALL_ACCESS, FALSE, _T("Lab5_AnswEvent"));

	// ���������� ����� ���������
	setlocale(LC_ALL,"Rus");

	// ��������� ������� "������" � ������ � ����� ������
	puts("");
	fflush(stdout);
	// ��������� ��� �������� A: ����� �����
	SetEvent(hAnswerEvent);

	// �������� ���� ������/������� ������
	do {
		// �������� ���������� ������ � ������������� strReceived
		WaitForSingleObject(hReceivedEvent, INFINITE);

		// ����� �� stderr, ������ ��� stdout ������������� � �����
		fprintf(stderr, "�������� �� A: %s\n", strReceived);
		if (strcmp(strReceived, "")) {
			strcat(strReceived, " : Process B");
		}
		// ����� � ����� � ����� ������
		puts(strReceived);
		fflush(stdout);
	} while (strcmp(strReceived, ""));

	// �������� ������������� strReceived
	UnmapViewOfFile(strReceived);

	// �������� ������ �������
	CloseHandle(hMapping);
	return 0;
}

