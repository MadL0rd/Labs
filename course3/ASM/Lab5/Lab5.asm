.model small
.stack 200h

.data
    buf db 512*15 dup (?)
    mess1 db 0ah, 0dh, '$' 
    nomber db 0
    mess2 db 'No files starting with t', 0ah, 0dh, '$'
    mess3 db 'Press whatever$'

.code

print_dec proc c
arg input:word

    push ax
    push bx
    push cx
    push dx

    ;Set up registers:
    ;AX = Number left to print
    ;BX = Power of ten to extract the current digit
    ;DX = Scratch/Needed for DIV
    ;CX = Scratch

    mov ax, input
    mov bx, 10000d
    xor dx, dx

    _pd_convert:     
        div bx                           ;DX = Number without highmost digit, AX = Highmost digit
        mov cx, dx                       ;Number left to print

        ;If digit is non zero or param for leading zeros is non zero
        ;print the digit
        cmp ax, 0
        jnz _pd_print

        ;If both are zeros, make sure to show at least one digit so that 0 prints as "0"
        cmp bx, 1
        jne _pd_continue

    _pd_print:

        ;Print digit in AL

        mov dl, al
        add dl, '0'
        mov ah, 02h
        int 21h

    _pd_continue:
        ;BX = BX/10
        ;DX = 0

        mov ax, bx
        xor dx, dx
        mov bx, 10d
        div bx
        mov bx, ax

        ;Put what's left of the number in AX again and repeat...
        mov ax, cx

        ;...Until the divisor is zero
        test bx, bx
        jnz _pd_convert

    pop dx
    pop cx
    pop bx
    pop ax

    ret
print_dec endp

print_kb proc c
arg input:word

    push ax
    push bx

    mov ax, input
    mov bx, input
    shr ax, 2
    and bx, 10b
    cmp bx, 0
    je _pkb_print
    add ax, 1

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



start:
mov ax, @data
mov ds, ax

begin:
; считываем корневой каталог в буфер
mov dx, 19
mov cx ,15
mov bx, offset buf
mov al, 0
int 25h
pop bx
jc exit1

; настраиваем si на начало буфера
mov si, offset buf

m2:
mov al, [si]   ; читаем первый байт очередного элемента каталога
cmp al, 0       ; файлов больше нет?
je exit1          ; да, выходим

cmp al, 0e5h  ; удаленный файл?
jne m3            ; нет, проверяем дальше
add si, 32
jmp m2          ; раз удален, переходим к следующему элементу каталога


m3:
; наращиваем счетчик
inc nomber

; выводим название файла на экран, начиная с текущей позиции курсора
mov cx, 11      ; число повторений цикла (8 – название, 3 – расширение)
mov ah, 0eh    ; функция

m4:
mov al, [si]    ; в al  заносим ASCII – код очередного выводимого символа
int 10h            ; выводим символ, курсор сам сдвигается на позицию вправо

inc si               ; теперь si адресует следующий выводимый символ
loop m4  

mov ah, 02h
mov dl, ' '
int 21h


mov ah, 0eh
add si, 18        ; 11 мы уже прибавили к si в цикле при выводе названия файла
mov ax, [si]
push ax
call print_kb
pop ax
add si, 3

; переводим курсор в начало следующей строки экрана
mov ah, 9
mov dx, offset mess1
int 21h

jmp m2           ; переходим к исследованию следующего элемента каталога

exit1:
cmp nomber, 0
jne exit                ; были файлы, начинающиеся с Т

; выводим сообщение, что искомых файлов не было
mov ah, 9
mov dx, offset mess2
int 21h

exit:
; выводим сообщение, с просьбой нажать любую клавишу      
mov ah, 9
mov dx, offset mess3
int 21h

mov ah, 7
int 21h                ; ждем, когда клавишу нажмут

mov ah, 4ch
int 21h

end start
