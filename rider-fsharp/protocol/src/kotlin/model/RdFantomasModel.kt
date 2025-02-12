package model

import com.jetbrains.rider.model.nova.ide.SolutionModel
import com.jetbrains.rd.generator.nova.*
import com.jetbrains.rd.generator.nova.PredefinedType.*
import com.jetbrains.rd.generator.nova.csharp.CSharp50Generator
import java.io.File

@Suppress("unused")
object RdFantomasModel : Root() {

  private val rdFcsParsingOptions = structdef {
    field("lastSourceFile", string)
    field("lightSyntax", bool.nullable)
    field("conditionalCompilationDefines", array(string))
    field("isExe", bool)
    field("langVersion", string)
  }

  private val rdFcsRange = structdef {
    field("fileName", string)
    field("startLine", int)
    field("startCol", int)
    field("endLine", int)
    field("endCol", int)
  }

  private val rdFantomasFormatArgs = basestruct {
    field("fileName", string)
    field("source", string)
    field("formatConfig", array(string))
    field("parsingOptions", rdFcsParsingOptions)
    field("newLineText", string)
  }

  init {
    call("getFormatConfigFields", void, array(string))
    call("formatDocument", structdef("rdFantomasFormatDocumentArgs") extends rdFantomasFormatArgs {}, string)
    call("formatSelection", structdef("rdFantomasFormatSelectionArgs") extends rdFantomasFormatArgs {
      field("range", rdFcsRange)
    }, string)

    call("getVersion", void, string)
    signal("exit", void)
  }
}
