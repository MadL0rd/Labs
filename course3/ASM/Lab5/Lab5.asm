.model small
.stack 200h

.data
    buff db 512*15 dup (?)                          ; буфер для информации о каталоге
    endl db 0ah, 0dh, '$'                           ; символы конца строки
    numfiles db 0                                   ; счетчик кол-ва файлов в каталоге
    msg_nofiles db 'No files found', 0ah, 0dh, '$'  ; сообщение о том, что нет файлов
    filename db 12 dup (?)                          ; имя наибольшего файла
    filesizeh dw 0                                  ; верхние 2 байта размера наиб. файла
    filesizel dw 0                                  ; нижние 2 байта размера наиб. файла

.code

; выводит 16бит число в десятичном виде
print_dec proc c
arg input:word

    push ax
    push bx
    push cx
    push dx

    ; Используемые регистры:
    ; AX = еще не выведенное число
    ; BX = степень десяти для вытаскивания текущей цифры
    ; DX = нужно для div
    ; CX = остаток от div
    mov ax, input
    mov bx, 10000d
    xor dx, dx

    _pd_convert:     
        div bx      ; DX = число без верхней цифры, AX = верхняя цифра
        mov cx, dx  ; еще невыведенное число

        ; если цифра не ноль, то выводим ее
        cmp ax, 0
        jnz _pd_print

        ; если все число равно нулю, то выводим ноль
        cmp bx, 1
        jne _pd_continue
    jmp _pd_print

    _pd_print:
        ; выводим цифру в al
        mov dl, al
        add dl, '0'
        mov ah, 02h
        int 21h
    jmp _pd_continue

    _pd_continue:
        ; bx = bx/10
        ; dx = 0
        mov ax, bx
        xor dx, dx
        mov bx, 10d
        div bx
        mov bx, ax

        ; кладем, что осталось от числа в ax и повторяем...
        mov ax, cx

        ; ...пока делитель не 0
        test bx, bx
    jnz _pd_convert

    pop dx
    pop cx
    pop bx
    pop ax

    ret
print_dec endp

; выводит размер в кб
; input - верхние 2 байта 32бит размера в байтах
print_kb proc c
arg input:word

    push ax
    push bx

    mov ax, input
    mov bx, input

    ; сдвигаем, чтобы 0000 0001 == 1кб
    shr ax, 2  
    ; округляем вверх, если в бите, значащем 512б, стоит 1
    and bx, 10b 
    cmp bx, 0
    je _pkb_print
    add ax, 1

    ; выводим xxxkb 
    _pkb_print:
        push ax
        call print_dec
        pop ax

        mov ah, 02h
        mov dl, 'k'
        int 21h
        mov dl, 'b'
        int 21h
        mov dl, ' '
        int 21h

    pop bx
    pop ax

    ret
print_kb endp

main:
    ; ставим сегмент данных
    mov ax, @data
    mov ds, ax

    ; считываем корневой каталог в буфер
    mov dx, 19
    mov cx, 15
    mov bx, offset buff
    mov al, 0
    int 25h
    pop bx
    jc check_count

    ; настраиваем si на начало буфера
    mov si, offset buff

jmp next_file

; читаем первый байт очередного элемента каталога
next_file:
    mov al, [si]

    ; файлов больше нет?
    cmp al, 0      
    je check_count  

    ; удаленный файл?
    cmp al, 0e5h
    jne check_file_size

    ; след. файл
    add si, 32

jmp next_file

; сравниваем размер файла
check_file_size:
    ; наращиваем счетчик
    inc numfiles

    ; запоминаем индекс 
    push si
    ;сдвигаем его к позиции с размером файла
    add si, 29

    ; сравниваем верхние два байта размера
    mov ax, [si]
    cmp ax, filesizeh
    jg  new_max_size
    jl  shift_index

    ; сравниваем нижние два байта размера
    mov ax, [si+2]
    cmp ax, filesizel
    jg  new_max_size

jmp shift_index

; новый макс. размер найден
new_max_size:
    ; запоминаем его
    mov ax, [si]
    mov filesizeh, ax
    mov ax, [si+2]
    mov filesizel, ax

    ; возвращаемся к началу элемента каталога
    pop si
    ; снова запоминаем индекс
    push si

    ; запоминаем имя файла
    mov bx, 0
    mov cx, 11      
    read_char:
        mov al, [si]
        mov [filename + bx], al

        inc si      
        inc bx
    loop read_char  
    mov [filename + bx], '$'

jmp shift_index

; возвращаемся к началу элемента каталога и переходим к след. элементу
shift_index:
    pop si
    add si, 32

jmp next_file

; проверяем, что хотя бы один файл был найден
check_count:
    cmp numfiles, 0
    jne print_file_info

    ; выводим сообщение, что файлов не было
    mov ah, 9
    mov dx, offset msg_nofiles
    int 21h

jmp exit

; выводим название и размер наибольшего файла
print_file_info:
    ; название и расширение
    mov ah, 9
    mov dx, offset filename
    int 21h
    ; пробел
    mov ah, 02h
    mov dl, ' '
    int 21h
    ; размер
    push filesizeh
    call print_kb
    pop filesizeh
    ; переводим курсор в начало следующей строки экрана
    mov ah, 9
    mov dx, offset endl
    int 21h

jmp exit

exit:
    mov ah, 4ch
    mov al, 0
    int 21h

end main
