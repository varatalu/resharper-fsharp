﻿module Kek

open JetBrains.Annotations
open System.Diagnostics.CodeAnalysis

type A() =
    [<RegexPattern>]
    member x.R = "[123]"

    member x.K = "[123]"

let f ([<RegexPattern>] x: string) = ()
f ("[123]")

let f1 =
    fun _ ->
        fun ([<RegexPattern>] x: string) -> ()
f1 "[123]" "[123]"

let g (x: string) = ()
g ("[123]")

[<RegexPattern>]
let x = "[123]"

[<StringSyntax("regex")>]
let y = "[123]"

let _ =
    ()
    let _ = "[123]" //language=regex
    ()
