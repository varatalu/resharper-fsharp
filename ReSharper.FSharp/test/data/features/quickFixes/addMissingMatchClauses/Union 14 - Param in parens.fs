module Say

type E =
    | A
    | B
    | C
    | D

type U =
    | A of E

match A E.A{caret} with
| A E.A -> ()
| A (E.B) -> ()
| A ((E.C)) -> ()
