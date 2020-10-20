﻿namespace JetBrains.ReSharper.Plugins.FSharp.Psi.Features.Daemon.QuickFixes

open System.Collections.Generic
open FSharp.Compiler.SourceCodeServices
open JetBrains.ReSharper.Plugins.FSharp.Psi
open JetBrains.ReSharper.Plugins.FSharp.Psi.Features.Daemon.Highlightings
open JetBrains.ReSharper.Plugins.FSharp.Psi.Features.Daemon.QuickFixes
open JetBrains.ReSharper.Plugins.FSharp.Psi.Features.Generate
open JetBrains.ReSharper.Plugins.FSharp.Psi.Impl.Tree
open JetBrains.ReSharper.Plugins.FSharp.Psi.Parsing
open JetBrains.ReSharper.Plugins.FSharp.Psi.Tree
open JetBrains.ReSharper.Plugins.FSharp.Psi.Util
open JetBrains.ReSharper.Plugins.FSharp.Util
open JetBrains.ReSharper.Psi
open JetBrains.ReSharper.Psi.ExtensionsAPI
open JetBrains.ReSharper.Psi.ExtensionsAPI.Tree
open JetBrains.ReSharper.Resources.Shell

type FSharpGeneratorMfvElement(mfv, substitution, addTypes) =
    new (mfvInstance: FcsMfvInstance, addTypes) =
        FSharpGeneratorMfvElement(mfvInstance.Mfv, mfvInstance.Substitution, addTypes)

    interface IFSharpGeneratorElement with
        member x.Mfv = mfv
        member x.Substitution = substitution
        member x.AddTypes = addTypes
        member x.IsOverride = false

type GenerateInterfaceMembersFix(error: NoImplementationGivenInterfaceError) =
    inherit FSharpQuickFixBase()

    let impl = error.Impl

    let getInterfaces (fcsType: FSharpType) =
        fcsType.AllInterfaces
        |> Seq.filter (fun t -> t.HasTypeDefinition)
        |> Seq.map FcsEntityInstance.create
        |> Seq.toList

    override x.Text = "Generate missing members"

    override x.IsAvailable _ =
        let fcsEntity = impl.FcsEntity
        isNotNull fcsEntity && fcsEntity.IsInterface 

    override x.ExecutePsiTransaction _ =
        use writeCookie = WriteLockCookie.Create(impl.IsPhysical())
        use disableFormatter = new DisableCodeFormatter()

        let interfaceType =
            let typeDeclaration =
                match FSharpTypeDeclarationNavigator.GetByTypeMember(impl) with
                | null ->
                    let repr = ObjectModelTypeRepresentationNavigator.GetByTypeMember(impl)
                    FSharpTypeDeclarationNavigator.GetByTypeRepresentation(repr)
                | decl -> decl

            let fcsEntity = typeDeclaration.GetFSharpSymbol() :?> FSharpEntity
            fcsEntity.DeclaredInterfaces |> Seq.find (fun e ->
                e.HasTypeDefinition && e.TypeDefinition.IsEffectivelySameAs(impl.FcsEntity))

        let displayContext = impl.TypeName.Reference.GetSymbolUse().DisplayContext

        let existingMemberDecls = impl.TypeMembers

        let implementedMembers =
            existingMemberDecls
            |> Seq.map (fun m ->
                m.DeclaredElement.As<IOverridableMember>().ExplicitImplementations
                |> Seq.choose (fun i -> i.Resolve() |> Option.ofObj |> Option.map (fun i -> i.Element.XMLDocId)))
            |> Seq.concat
            |> HashSet

        let allInterfaceMembers = 
            getInterfaces interfaceType |> List.collect (fun fcsEntityInstance ->
                fcsEntityInstance.Entity.MembersFunctionsAndValues
                |> Seq.map (fun mfv -> FcsMfvInstance.create mfv fcsEntityInstance.Substitution)
                |> Seq.toList)

        let needsTypesAnnotations =
            GenerateOverrides.getMembersNeedingTypeAnnotations allInterfaceMembers

        let membersToGenerate = 
            allInterfaceMembers
            |> List.filter (fun mfvInstance ->
                let mfv = mfvInstance.Mfv
                // todo: other accessors
                not (mfv.IsPropertyGetterMethod || mfv.IsPropertySetterMethod) &&

                let xmlDocId = FSharpElementsUtil.GetXmlDocId(mfv)
                isNotNull xmlDocId && not (implementedMembers.Contains(xmlDocId)))
            |> List.sortBy (fun mfvInstance -> mfvInstance.Mfv.LogicalName) // todo: better sorting?
            |> List.map (fun mfvInstance -> mfvInstance, needsTypesAnnotations.Contains(mfvInstance.Mfv))
            |> List.map FSharpGeneratorMfvElement

        let indent =
            if existingMemberDecls.IsEmpty then
                impl.Indent + impl.GetIndentSize()
            else
                existingMemberDecls.Last().Indent

        let generatedMembers =
            membersToGenerate
            |> List.map (GenerateOverrides.generateMember impl displayContext)
            |> List.collect (withNewLineAndIndentBefore indent)

        if isNull impl.WithKeyword then
            addNodesAfter impl.LastChild [
                Whitespace()
                FSharpTokenType.WITH.CreateLeafElement()
            ] |> ignore

        let anchor = GenerateOverrides.addEmptyLineIfNeeded impl.LastChild
        addNodesAfter anchor generatedMembers |> ignore
