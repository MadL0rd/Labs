program pascal;

const n1 : longint = 5;
const n2 : longint = 6;

type matrix = array[1..6,1..6] of longint;

var amount : longint;
var m1 : matrix;
var m2 : matrix;
var a, d : real;
var i, k, c1, c2 : longint;
var i1, i2, i3, i4, i5, i6 : longint;

function pow (number, exponent : real) : real;
begin
	if number = 0 then begin
		pow := 0;
		exit;
	end;
	pow := Exp(exponent*Ln(number));
end;

function factorial(a : longint) : longint;
var res : longint;
begin
	if a <= 1 then begin
		factorial := 1;
		exit;
	end;

	res := a;
	dec(a);

	while a > 0 do begin
		res := res * a;
		dec(a);
	end;

	factorial := res;
end;

function powWithNeg(n : real; p : real) : real;
begin
	if n < 0 then begin
		powWithNeg := pow(n * -1, p) * -1;
	end	else begin
		powWithNeg := pow(n, p);
	end;
end;

procedure solveKindOfCubic(a : real; d : real);
var g, p, s, u, x1, x2r, x2i, hsqrt : real;
begin
	writeln('Solving cubic equiation for a = ', a:4:1, '; b = 0.0; c = 0.0; d = ', d:4:1, ';');

	g := d / a;

	hsqrt := sqrt(g * g / 4);

	p := 1.0 / 3.0;

	s := -(g / 2) + hsqrt;
	s := powWithNeg(s, p);

	u := -(g / 2) - hsqrt;
	u := powWithNeg(u, p);

	x1 := s + u;

	x2r := -1 * (s + u) / 2;
	x2i := ((s - u) / 2) * pow(3, 0.5);

	writeln('x1 = ', x1:10:8, ';');
	writeln('x2 = ', x2r:10:8, ' + i*', x2i:10:8, ';');
	writeln('x3 = ', x2r:10:8, ' - i*', x2i:10:8, ';');
end;

begin

	readln(amount);
	writeln(amount);

	while amount > 0 do begin

		dec(amount);

		c1 := 0;
		c2 := 0;

		writeln('T(5, 5)');
		for i := 1 to n1 do begin
			readln(i1, i2, i3, i4, i5);
			writeln(i1, ' ', i2, ' ', i3, ' ', i4, ' ', i5);
			if i2 > 0 then c1 := c1 + 1;
			if i4 > 0 then c1 := c1 + 1;
		end;

		writeln('D(6, 6)');
		for i := 1 to n2 do begin
			readln(i1, i2, i3, i4, i5, i6);
			writeln(i1, ' ', i2, ' ', i3, ' ', i4, ' ', i5, ' ', i6);
			if i2 > 0 then c2 := c2 + 1;
			if i4 > 0 then c2 := c2 + 1;
			if i6 > 0 then c2 := c2 + 1;
		end;

		writeln;

		writeln('a = ', c1, ';');
		writeln('b = ', c2, ';');

		a := 3 * factorial(c1 + c2);
		d := 2 * factorial(c1);

		solveKindOfCubic(a, d);
		writeln;
		writeln;
	end;
end.

