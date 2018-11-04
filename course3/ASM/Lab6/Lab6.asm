code segment 
assume cs:code, ds:code
org 100h

; Переход на загрузочную часть программы
main: jmp load

; Область  данных  резидентной  части.
    key dw 5555h  ; ключ расположен в ячейке со смещением 103h (100h байт PSP и 3 байта jmp load)
    oldvect dd 0  ; здесь запоминаем адрес системного обработчика
    ; сообщение и его длина
    msg db 'NOBODY IS GOING TO HELP YOU NOW'
    msg_len dw 31
    ; позиция сообщения
    x db 22
    y db 24
    ; атрибут сообщения
    attr db ?
    cursor_position dw ?

; Позиционирует курсор в DX
set_cursor proc c
    push bx
    push ax

    mov bh, 0  ; в bh № видеостраницы
    mov ah, 2  ; функция
    int 10h

    pop ax
    pop bx
    ret
set_cursor endp

; Запоминает позицию курсора в cursor_position
save_cursor proc c
    push ax
    push bx
    push dx

    mov ah, 3
    mov bh, 0
    int 10h
    mov cs:cursor_position, dx

    pop dx
    pop bx
    pop ax
    ret
save_cursor endp

; Восстанавливает курсор в cursor_position
reset_cursor proc c
    push dx
    push bx
    push ax

    mov dx, cs:cursor_position
    mov ah, 2
    mov bh, 0
    int 10h 

    pop ax
    pop bx
    pop dx
    ret
reset_cursor endp

; Очищает весь экран используя глобавльный атрибут attr
cls proc c
    push dx
    push cx
    push bx
    push ax

    mov bh, cs:attr
    mov cl, 0
    mov ch, 0
    mov dl, 79
    mov dh, 24
    mov al, 0
    mov ah, 6
    int 10h

    pop ax
    pop bx
    pop cx
    pop dx
    ret
cls endp

; Сдвигает весь экран на одну строку вверх
scrollup proc c
    push bx
    push ax
    push cx

    mov bh, cs:attr
    mov cl, 0
    mov dl, 79
    mov al, 1
    mov ah, 6
    int 10h
    
    pop cx
    pop ax
    pop bx
    ret
scrollup endp

; Выводит глобальное сообщение msg глобальной длиной msg_len
print_msg proc c
    push cx
    push bx
    push ax

    ;  Выводим сообщение, начиная с текущей позиции курсора, что Alt/t  нажата. 
    mov cx, cs:msg_len
    mov bx, offset msg
    _pm_loop:  
        mov ah, 0eh
        mov al, cs:[bx]
        int 10h
        inc bx
    loop _pm_loop

    pop ax
    pop bx
    pop cx
    ret
print_msg endp

; Устанавливает таймаут в одну секунду
timeout proc c
    push ax
    push cx
    push dx
    push bx

    mov ah, 0
    int 1ah
    mov bx, dx
    add bx, 1
    _t_loop:
        mov ah, 0
        int 1ah
        cmp bx, dx
    ja _t_loop

    pop bx
    pop dx
    pop cx
    pop ax
    ret
timeout endp


; <РЕЗИДЕНТНАЯ ЧАСТЬ>  
newvect:
    ; сохраняем в стеке все регистры, которые можем испортить
    push ax
    push es
    push bx
    push cx
    push dx

    ; выясняем, нажата ли F1
    in al, 60h
    cmp al, 3bh
    jne exit

    ; запоминаем позицию курсора
    call save_cursor

    ; устанавливаем курсор по центру в низу
    mov dl, cs:x
    mov dh, cs:y
    call set_cursor
    call scrollup

    ; запускаем цикл скролла
    mov cx, 50
    scroll:
        mov cs:attr, 49h ; ставим синий текст на красном фоне  
        mov dl, cs:x     ; возвращаем курсор в 
        call set_cursor  ; центр текущей строки
        call print_msg   ; выводим сообщение
        call timeout     ; ждем
        call scrollup    ; скроллим экран вверх на один
    loop scroll

    ; очищаем экран черным цветом и воостанавливаем курсор в первичное положение
    mov cs:attr, 07h
    call cls
    call reset_cursor

exit:
    ; восстанавливаем в регистрах информацию фоновой программы.
    pop dx
    pop cx
    pop bx
    pop es
    pop ax
    ; вызываем правильный системный обработчик. Так как ячейка oldvect
    ; описана как двойное слово,  команде  jmp  автоматически  будет присвоен тип far.
    jmp cs:oldvect

; </РЕЗИДЕНТНАЯ ЧАСТЬ>  


; <ЗАГРУЗОЧНАЯ ЧАСТЬ>
load:
    ; Получаем вектор правильного (системного) обработчика (в ES:BX).
    mov ah, 35h
    mov al, 9
    int 21h
    ; Производим проверку (сравнивая с ключом) на повторную установку программы.
    cmp word ptr es:[103h], 5555h
    jz already_loaded
    ; Запоминаем вектор правильного (системного) обработчика.
    mov word ptr oldvect, bx
    mov word ptr oldvect+2, es
    ; Устанавливаем вектор своего обработчика.
    mov dx, offset newvect
    mov ah, 25h
    mov al, 9
    int 21h 
    ; Завершаем программу, оставляя резидентной в памяти ее часть,
    ; от начала PSP до метки load.
    mov dx, offset load
    int 27h

; Если программа уже установлена в памяти, выдаем сообщение об
; этом и завершаем программу, ничего не оставляя в памяти.
already_loaded:
    mov ah, 9
    mov dx, offset msg2
    int 21h
    mov ah,4ch
    int 21h

; Область данных  загрузочной части.
    msg2 db 'Already loaded$'

; </ЗАГРУЗОЧНАЯ ЧАСТЬ>


code ends
end main
