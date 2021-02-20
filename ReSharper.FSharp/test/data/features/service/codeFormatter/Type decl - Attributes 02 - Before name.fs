type [<A>] U =
    | A
    | B of int * int

type [<A>] E =
    | A = 1
    | B = 2

type [<A>] R =
    { F: int }

type [<A>] D =
    delegate of int -> int

type [<A>] T() =
    class end

type [<A>] Abbr = int
