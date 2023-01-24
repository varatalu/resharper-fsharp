package com.jetbrains.rider.plugins.fsharp.actions

import com.jetbrains.rdclient.editorActions.FrontendDummyLineIndentProvider
import com.jetbrains.rider.ideaInterop.fileTypes.fsharp.FSharpLanguage
import com.jetbrains.rider.ideaInterop.fileTypes.fsharp.FSharpScriptLanguage

object FSharpDummyLineIndentProvider : FrontendDummyLineIndentProvider(FSharpLanguage)
object FSharpScriptDummyLineIndentProvider : FrontendDummyLineIndentProvider(FSharpScriptLanguage)
