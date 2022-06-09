package typeProviders

import com.intellij.openapi.actionSystem.ActionPlaces
import com.intellij.openapi.actionSystem.IdeActions
import com.jetbrains.rdclient.testFramework.executeWithGold
import com.jetbrains.rdclient.testFramework.waitForDaemon
import com.jetbrains.rdclient.testFramework.waitForNextDaemon
import com.jetbrains.rider.daemon.util.hasErrors
import com.jetbrains.rider.test.annotations.TestEnvironment
import com.jetbrains.rider.test.asserts.shouldBeFalse
import com.jetbrains.rider.test.base.EditorTestBase
import com.jetbrains.rider.test.enums.CoreVersion
import com.jetbrains.rider.test.enums.ToolsetVersion
import com.jetbrains.rider.test.scriptingApi.*
import org.testng.annotations.Test
import java.io.File

@Test
@TestEnvironment(
    toolset = ToolsetVersion.TOOLSET_17_CORE,
    coreVersion = CoreVersion.DOT_NET_6
)
class TypeProvidersFeaturesTest : EditorTestBase() {
    override fun getSolutionDirectoryName() = "SwaggerProviderCSharp"
    override val restoreNuGetPackages = true

    //TODO: with signature files, wrong abbreviation


    @Test
    fun `provided member navigation`() = doNavigationTest()

    @Test
    fun `provided abbreviation navigation`() = doNavigationTest()

    @Test
    fun `provided nested type navigation`() = doNavigationTest()

    @Test
    fun `provided member rename disabled`() = doRenameUnavailableTest()

    @Test
    fun `provided nested type rename disabled`() = doRenameUnavailableTest()

    @Test
    fun `provided abbreviation rename`() {
        withOpenedEditor("CSharpLibrary/CSharpLibrary.cs", "CSharpLibrary.cs") {
            waitForDaemon()
            defaultRefactoringRename("Renamed")
            waitForNextDaemon()
            markupAdapter.hasErrors.shouldBeFalse()
            executeWithGold(File(testGoldFile.path + " - csharp")) {
                dumpOpenedDocument(it, project!!)
            }
        }

        withOpenedEditor("SwaggerProviderLibrary/SwaggerProvider.fs") {
            waitForDaemon()
            markupAdapter.hasErrors.shouldBeFalse()
            executeWithGold(File(testGoldFile.path + " - fsharp")) {
                dumpOpenedDocument(it, project!!)
            }
        }
    }

    private fun doNavigationTest() {
        withOpenedEditor("CSharpLibrary/CSharpLibrary.cs", "CSharpLibrary.cs") {
            waitForDaemon()
            gotoDeclaration {
                waitForEditorSwitch("SwaggerProvider.fs")
                waitForDaemon()
                executeWithGold(testGoldFile) {
                    dumpOpenedDocument(it, project!!, true)
                }
            }
        }
    }

    private fun doRenameUnavailableTest() {
        withOpenedEditor("CSharpLibrary/CSharpLibrary.cs", "CSharpLibrary.cs") {
            waitForDaemon()
            assertActionDisabled(project!!, dataContext, ActionPlaces.UNKNOWN, IdeActions.ACTION_RENAME)
        }
    }
}
