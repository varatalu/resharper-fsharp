namespace Ns1

type A() = class end

namespace Ns2

open Ns1

type A with
    static member P = 1

module Module =
    let a = A()
    a.P{caret}
