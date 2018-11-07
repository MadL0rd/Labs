.model small
.stack 200h

.data

; НОТЫ
; октава 1
en1 equ 3091
gs1 equ 2454
an1 equ 2316
bn1 equ 2063
; октава 2
cs2 equ 1838
en2 equ 1546
gs2 equ 1227
bn2 equ 1032
; октава 3
cn3 equ 974
cs3 equ 919
en3 equ 773
fs3 equ 689
gs3 equ 613
an3 equ 579
bn3 equ 516
; октава 4
cs4 equ 459
en4 equ 386

; ПЕСНЯ
; мелодия
seq dw en1, 3, bn1, 3, en2, 3, an3, 6, en2, 3, gs3, 3, en2, 3
    dw gs1, 3, gs2, 3, cn3, 3, gs3, 6, gs2, 3, fs3, 3, gs2, 3
    dw cs2, 3, cs3, 3, en3, 3, cs4, 6, cs3, 3, bn3, 3, cs3, 3
    dw an1, 3, cs3, 3, en3, 3, bn3, 6, an1, 3, an3, 3, an1, 3
    dw en2, 3, bn2, 3, en3, 3, an3, 3, gs3, 3, en3, 3, bn2, 3, en2, 3
    dw gs1, 3, gs2, 3, cn3, 3, gs3, 6, gs2, 3, fs3, 3, gs2, 3
    dw cs2, 3, cs3, 3, en3, 3, cs4, 6, cs3, 3, bn3, 3, cs3, 3
    dw an1, 3, cs3, 3, en3, 3, bn3, 6, an1, 3, an3, 3, an1, 6
    dw en4, 6, 0, 6, en1, 18
    dw 0, 0
; название песни
song_title db 'Now Playing: Bo Burnham - Art Is Dead$'

.code

; включает звук
turn_sound_on proc
    mov al, 00000011b
    out 61h, al
    ret
turn_sound_on endp

; выключает звук
turn_sound_off proc
    mov al, 00000000b
    out 61h, al
    ret
turn_sound_off endp

; загружает ноту
play_note proc
    out 42h, al
    mov al, ah
    out 42h, al
    ret
play_note endp

; задержка
timeout proc
    mov ah, 86h
    mov dx, 0
    int 15h
    ret
timeout endp

main:
    ; адрес сегмента данных
    mov ax, @data
    mov ds, ax

    ; выводим название
    mov dx, offset song_title
    mov ah, 9
    int 21h

    ; настраиваем канал
    mov al, 0b6h
    out 43h, al 

    ; адрес массива с нотами и задержками
    mov bx, offset seq

    ; включаем звук
    call turn_sound_on

; играем текущую ноту
play:
    mov ax, [bx]     ; частота ноты
    mov cx, [bx + 2] ; длительность задержки
    
    ; если частота 0, то не играем ничего
    cmp ax, 0
    je quite
    
    ; загружаем ноту и ставим задержку
    call play_note
    call timeout

    jmp shift

; не играем ничего
quite:
    ; если и задержка 0, то это конец мелодии
    cmp cx, 0
    je exit

    ; выключаем звук на заданное в cx время
    call turn_sound_off
    call timeout
    call turn_sound_on

    jmp shift

; сдвигаем bx на следующую ноту
shift:
    add bx, 4
    jmp play

exit:
    ; выключаем звук
    call turn_sound_off

    ; выход
    mov ah, 4ch
    mov al, 0
    int 21h

end main
