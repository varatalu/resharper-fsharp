module Module

let (|Id|) x = x

type U =
    | A of int

match A 1 with
| Id _{caret} -> ()
