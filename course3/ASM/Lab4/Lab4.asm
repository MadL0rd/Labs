.model  small
.stack  200h
.386

.data
TRUE equ 255
FALSE equ 0

sphere  db '0$'
sphere_attr db 79h

xplosion db '* BOOM!$'
xplosion_attr db 74h

minx db 0
miny db 0
maxx db 79
maxy db 24

delayh dw 1
delayl dw 0
longdelay dw 10

x1 db ?
y1 db 8
dirx1 db 0
diry1 db 0

x2 db 35
y2 db ?
dirx2 db 255
diry2 db 0

.code

; уменьшает/увеличивает al в зависимости от направления в ah
; меняет направление в ah в соответствии с limits
move_axis proc c
arg min:byte:1, max:byte:1

    ; проверяем направление движения
    check_dir:
        cmp ah, FALSE
        jne forward

    ; движемся назад
    backward: 
        cmp al, min
        je reverse
        dec al
        jmp done
        
    ; движемся вперед
    forward:
        cmp al, max
        je reverse
        inc al
        jmp done

    reverse:
        xor ah, TRUE

    done:
        ret

move_axis endp

;позиционируем курсор
set_cursor proc
    mov bh, 0  ; в bh № видеостраницы
    mov ah, 2  ; функция
    int 10h
    ret
set_cursor endp

; выводим шар
draw_sphere proc
    mov dl, '0'
    mov ah, 02h
    int 21h
    ret  
draw_sphere endp

; возращает типа рандомное число от 0 до bx
rand proc
    mov ah, 00h  ; получаем системное время
    int 1ah      ; помещаем время в миллисекундах в CX:DX
    mov ax, dx
    xor dx, dx
    mov cx, bx    
    div cx       ; получаем остаток от деления на bx
    ret  
rand endp

; очищает установленную позицию
cls proc
    mov al, 0
    mov ah, 6
    int 10h
    ret
cls endp


; начало программы
start:

    ; адрес сегмента данных
    mov ax, @data
    mov ds, ax

    ; рандомим x1
    xor bx, bx
    mov bl, maxx
    call rand
    mov x1, dl

    ; рандомим y2
    xor bx, bx
    mov bl, maxy
    call rand
    mov y2, dl

    ; прячем курсор
    mov ah, 1
    mov ch, 20
    mov cl, 0
    int 10h

    ; очищаем экран
    mov bh, sphere_attr ; текущие атрибуты 
    mov cl, minx        ; координата Х левого верхнего угла экрана (№ столбца)
    mov ch, miny        ; координата Y левого верхнего угла экрана (№ строки)
    mov dl, maxx        ; координата Х правого нижнего угла экрана  (№ столбца) 
    mov dh, maxy        ; координата Y правого нижнего угла экрана  (№ строки)   
    call cls

; обновление экрана
update:

    ; выодим круг 1
    mov dl, x1
    mov dh, y1
    call set_cursor
    call draw_sphere

    ; выводим круг 2
    mov dl, x2 
    mov dh, y2
    call set_cursor
    call draw_sphere

    ; задержка
    mov ah, 86h
    mov cx, delayh
    mov dx, delayl
    int 15h

    ; проверяем выход из программы и паузу
    mov dl, 0ffh
    mov ah, 6
    int 21h
    ; никакая клавиша не нажата (zf=1)
    jz  clear        
    ; пробел
    cmp al, 20h
    je hold       
    ; Esc
    cmp al, 1bh
    je  exit       
    jmp clear

; ждем любой клавиши
hold:
    mov ah, 0
    int 16h  
    xor dirx1, TRUE
    xor dirx2, TRUE

; стираем предыдущую позицию шаров
clear:
    mov bh, sphere_attr

    mov cl, x1
    mov ch, y1
    mov dl, x1
    mov dh, y1
    call cls

    mov cl, x2
    mov ch, y2
    mov dl, x2
    mov dh, y2
    call cls

; обновляем позиции кругов
move:
    ; ось x
    mov dl, minx
    mov dh, maxx
    push dx

    mov al, x1
    mov ah, dirx1
    call move_axis
    mov x1, al
    mov dirx1, ah

    mov al, x2
    mov ah, dirx2
    call move_axis
    mov x2, al
    mov dirx2, ah
    pop dx

    ; ось y
    mov dl, miny
    mov dh, maxy
    push dx

    mov al, y1
    mov ah, diry1
    call move_axis
    mov y1, al
    mov diry1, ah

    mov al, y2
    mov ah, diry2
    call move_axis
    mov y2, al
    mov diry2, ah
    pop dx

; проверка столкновения кругов
check_collision:
    mov ah, x1
    cmp ah, x2
    jne update
    mov ah, y1
    cmp ah, y2
    jne update

; эффект при столкновении
collide:
    mov bh, xplosion_attr
    mov cl, x1
    mov ch, y1
    mov dl, x1
    mov dh, y1
    call cls

    mov dl, x1 
    mov dh, y1
    call set_cursor
    mov dx, offset xplosion
    mov ah, 9
    int 21h    ; выводим все до доллара

    ; задержка при столкновении
    mov ah, 86h
    mov cx, longdelay
    mov dx, 0
    int 15h

exit:
    ; ресет курсора
    mov ah, 1
    mov ch, 6
    mov cl, 7
    int 10h

    ; выход
    mov ah, 4ch
    mov al, 0
    int 21h
end start
