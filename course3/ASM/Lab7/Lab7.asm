.model small
.stack 200h

.data

; НОТЫ
; октава 1
ef1 equ 3091
gs1 equ 2454
af1 equ 2316
bf1 equ 2063
; октава 2
cs2 equ 1838
ef2 equ 1546
gs2 equ 1227
bf2 equ 1032
; октава 3
cf3 equ 974
cs3 equ 919
ef3 equ 773
fs3 equ 689
gs3 equ 613
af3 equ 579
bf3 equ 516
; октава 4
cs4 equ 459
ef4 equ 386

; ПЕСНЯ
; мелодия
seq dw ef1, 3, bf1, 3, ef2, 3, af3, 6, ef2, 3, gs3, 3, ef2, 3
    dw gs1, 3, gs2, 3, cf3, 3, gs3, 6, gs2, 3, fs3, 3, gs2, 3
    dw cs2, 3, cs3, 3, ef3, 3, cs4, 6, cs3, 3, bf3, 3, cs3, 3
    dw af1, 3, cs3, 3, ef3, 3, bf3, 6, af1, 3, af3, 3, af1, 3
    dw ef2, 3, bf2, 3, ef3, 3, af3, 3, gs3, 3, ef3, 3, bf2, 3, ef2, 3
    dw gs1, 3, gs2, 3, cf3, 3, gs3, 6, gs2, 3, fs3, 3, gs2, 3
    dw cs2, 3, cs3, 3, ef3, 3, cs4, 6, cs3, 3, bf3, 3, cs3, 3
    dw af1, 3, cs3, 3, ef3, 3, bf3, 6, af1, 3, af3, 3, af1, 6
    dw ef4, 6, 0, 6, ef1, 18
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
