.model  small
.stack  200h
.386

.data
song_title db 'Now Playing: Bo Burnham - Art Is Dead$'

.code

; включение спикера
on proc
    mov al, 00000011b
    out 61h, al
    ret
on endp

; выключение спикера
off proc
    mov al, 00000000b
    out 61h, al
    ret
off endp

; вывод ноты
ln proc
    out 42h, al
    mov al, ah
    out 42h, al
    ret
ln endp


; ОКТАВЫ

; октава 1
ef1 proc
    mov ax, 3091
    call ln
    ret
ef1 endp

gs1 proc
    mov ax, 2454
    call ln
    ret
gs1 endp

af1 proc
    mov ax, 2316
    call ln
    ret
af1 endp

bf1 proc
    mov ax, 2063
    call ln
    ret
bf1 endp

; октава 2
cs2 proc
    mov ax, 1838
    call ln
    ret
cs2 endp

ef2 proc
    mov ax, 1546
    call ln
    ret
ef2 endp

gs2 proc
    mov ax, 1227
    call ln
    ret
gs2 endp

bf2 proc
    mov ax, 1032
    call ln
    ret
bf2 endp

; октава 3
cf3 proc
    mov ax, 974
    call ln
    ret
cf3 endp

cs3 proc
    mov ax, 919
    call ln
    ret
cs3 endp

ef3 proc
    mov ax, 773
    call ln
    ret
ef3 endp

fs3 proc
    mov ax, 689
    call ln
    ret
fs3 endp

gs3 proc
    mov ax, 613
    call ln
    ret
gs3 endp

af3 proc
    mov ax, 579
    call ln
    ret
af3 endp

bf3 proc
    mov ax, 516
    call ln
    ret
bf3 endp

; октава 4
cs4 proc
    mov ax, 459
    call ln
    ret
cs4 endp

ef4 proc
    mov ax, 386
    call ln
    ret
ef4 endp


; ПАУЗЫ
n2 proc
    mov cx, 2
    call n
    ret
n2 endp

n3 proc
    mov cx, 3
    call n
    ret
n3 endp

n4 proc
    mov cx, 4
    call n
    ret
n4 endp

n6 proc
    mov cx, 6
    call n
    ret
n6 endp

n18 proc
    mov cx, 18
    call n
    ret
n18 endp

n proc
    mov ah, 86h
    mov dx, 0
    int 15h
    ret
n endp

; ТАКТЫ

; такт 1
t1 proc
    call ef1
    call n3
    call bf1
    call n3
    call ef2
    call n3
    call af3
    call n6
    call ef2
    call n3
    call gs3
    call n3
    call ef2
    call n3
    ret
t1 endp

; такт 1 вариация
t1v proc
    call ef2
    call n3
    call bf2
    call n3
    call ef3
    call n3
    call af3
    call n3
    call gs3
    call n3
    call ef3
    call n3
    call bf2
    call n3
    call ef2
    call n3
    ret
t1v endp

; такт 2
t2 proc
    call gs1
    call n3
    call gs2
    call n3
    call cf3
    call n3
    call gs3
    call n6
    call gs2
    call n3
    call fs3
    call n3
    call gs2
    call n3
    ret
t2 endp

; такт 3
t3 proc
    call cs2
    call n3
    call cs3
    call n3
    call ef3
    call n3
    call cs4
    call n6
    call cs3
    call n3
    call bf3
    call n3
    call cs3
    call n3
    ret
t3 endp

; такт 4
t4 proc
    call af1
    call n3
    call cs3
    call n3
    call ef3
    call n3
    call bf3
    call n6
    call af1
    call n3
    call af3
    call n3
    call af1
    call n3
    ret
t4 endp

; окончание
fin proc
    call af1
    call n3
    call ef4
    call n6
    call off
    call n6
    call on
    call ef1
    call n18
    ret
fin endp

main:

    ; адрес сегмента данных
    mov ax, @data
    mov ds, ax

    ; выводим название
    mov dx, offset song_title
    mov ah, 9
    int 21h

    ; настройка канала
    mov al, 0b6h
    out 43h, al 

    call on

    call t1
    call t2
    call t3
    call t4

    call t1v
    call t2
    call t3
    call t4
    
    call fin

    call off

    ; выход
    mov ah, 4ch
    mov al, 0
    int 21h

end main
