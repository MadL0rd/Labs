@echo off

:: Перемещает файлы, созданные в указанный год, в подпапку, названную этим годом
:: Работает только с форматом dd-MMM-yy
:: Тестировалось только на английской локализации windows 8.1, должно работать на русской

:: Проверяем корректность формата даты
if "%date:~2,1%" neq "-" (
	echo Date must be in dd-MMM-yy format
	echo Go to Control Panel ^> Region to change it
	goto :end
)

:: Введенный год
set _year=%1

:: Проверяем является ли ввод годом в правильном формате ಠ_ಠ
:start
set _correct=1
if "%_year:~0,1%" == "" set _correct=0
if "%_year:~1,1%" == "" set _correct=0
if "%_year:~0,1%" lss "0" set _correct=0
if "%_year:~0,1%" gtr "9" set _correct=0
if "%_year:~1,1%" lss "0" set _correct=0
if "%_year:~1,1%" gtr "9" set _correct=0
if "%_year:~2,1%" neq "" set _correct=0
if %_correct% equ 0 (
	set /p _year=Please input year ^([0-9][0-9]^):
	goto :start
)

:: Флаг, указывающий, был ли перемещен хотя бы один файл
set _copied=0

:: Построчно проходим по выводу dir без дерикторий, запоминая:
::	 дату (в формате dd-MMM-yy) (%%~a),
::	 время (%%~b) создания файла, 
::	 размер (%%~c) и 
::	 имя файла (%%~d - до конца строки)
for /f "skip=5 tokens=1,2,3* delims= " %%a in ('dir /a:-d /t:c') do (

	rem dir выводит статистику, которую мы пытаемся пропустить
	rem также мы пропускаем текущий bat файл
	if "%%~b" neq "File(s)" if "%%~b" neq "Dir(s)" if "%%~b" neq "файлов" if "%%~b" neq "папок" if "%%~d" neq "%0.bat" if "%%~d" neq "%0" (

		:: Вытаскиваем год из даты и сравниваем с вводом
		for /f "tokens=3 delims=-" %%e in ("%%~a") do (

			if "%%~e" == "%_year%" (

				:: Создаем папку, если ее нет
				if not exist %_year%\NUL mkdir %_year%

				:: Перемещаем файл
				echo Moving "%%~d"
				move "%%~d" "%_year%"

				:: Файл был перемещен
				if %_copied% equ 0 set _copied=1
			) 
		)
	)
)

:: Выводим сообщение, что ни один файл не был перемещен
if %_copied% equ 0 echo No files moved

:end