//${OCCURRENCE0:Bind 'int * string' computation with let!}
//${OCCURRENCE1:Deconstruct tuple}

async {
    {selstart}async { return 1, "" }{selend}{caret}
    return 1
}
