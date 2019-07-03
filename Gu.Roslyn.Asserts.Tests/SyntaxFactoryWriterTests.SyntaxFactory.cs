namespace Gu.Roslyn.Asserts.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Scripting;
    using Microsoft.CodeAnalysis.Scripting;
    using NUnit.Framework;

    public static partial class SyntaxFactoryWriterTests
    {
        private static readonly ScriptOptions ScriptOptions = ScriptOptions.Default
                                                                           .WithReferences(Gu.Roslyn.Asserts.MetadataReferences.Transitive(typeof(SyntaxFactory)))
                                                                           .WithImports("Microsoft.CodeAnalysis.CSharp", "Microsoft.CodeAnalysis.CSharp.Syntax")
                                                                           .WithEmitDebugInformation(emitDebugInformation: true);

        private static readonly IReadOnlyList<FileInfo> CSharpFiles = SolutionFile.Find("Gu.Roslyn.Asserts.sln")
                                                                                  .Directory.EnumerateFiles("*.cs", SearchOption.AllDirectories)
                                                                                  .ToArray();

        [Explicit("Fix later.")]
        [TestCaseSource(nameof(CSharpFiles))]
        public static async Task Roundtrip(FileInfo file)
        {
            var code = File.ReadAllText(file.FullName);
            await AssertRoundtrip(code).ConfigureAwait(false);
        }

        [Test]
        public static async Task ClassInNamespace()
        {
            var code = @"namespace A.B
{
    public class C
    {
    }
}";
            var call = SyntaxFactoryWriter.Serialize(code);
            var expected = @"SyntaxFactory.CompilationUnit(
    externs: default,
    usings: default,
    attributeLists: default,
    members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
        SyntaxFactory.NamespaceDeclaration(
            namespaceKeyword: SyntaxFactory.Token(
                leading: default,
                kind: SyntaxKind.NamespaceKeyword,
                trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space)),
            name: SyntaxFactory.QualifiedName(
                left: SyntaxFactory.IdentifierName(
                    identifier: SyntaxFactory.Identifier(
                        leading: default,
                        text: ""A"",
                        trailing: default)),
                dotToken: SyntaxFactory.Token(SyntaxKind.DotToken),
                right: SyntaxFactory.IdentifierName(
                    identifier: SyntaxFactory.Identifier(
                        leading: default,
                        text: ""B"",
                        trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)))),
            openBraceToken: SyntaxFactory.Token(
                leading: default,
                kind: SyntaxKind.OpenBraceToken,
                trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),
            externs: default,
            usings: default,
            members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                SyntaxFactory.ClassDeclaration(
                    attributeLists: default,
                    modifiers: SyntaxFactory.TokenList(
                        SyntaxFactory.Token(
                            leading: SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(""    "")),
                            kind: SyntaxKind.PublicKeyword,
                            trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space))),
                    keyword: SyntaxFactory.Token(
                        leading: default,
                        kind: SyntaxKind.ClassKeyword,
                        trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space)),
                    identifier: SyntaxFactory.Identifier(
                        leading: default,
                        text: ""C"",
                        trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),
                    typeParameterList: default,
                    baseList: default,
                    constraintClauses: default,
                    openBraceToken: SyntaxFactory.Token(
                        leading: SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(""    "")),
                        kind: SyntaxKind.OpenBraceToken,
                        trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),
                    members: default,
                    closeBraceToken: SyntaxFactory.Token(
                        leading: SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(""    "")),
                        kind: SyntaxKind.CloseBraceToken,
                        trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),
                    semicolonToken: default)),
            closeBraceToken: SyntaxFactory.Token(SyntaxKind.CloseBraceToken),
            semicolonToken: default)),
    endOfFileToken: SyntaxFactory.Token(SyntaxKind.EndOfFileToken))";
            CodeAssert.AreEqual(expected, call);
            await AssertRoundtrip(code).ConfigureAwait(false);
        }

        [Explicit("Fix later.")]
        [Test]
        public static async Task EnumInNamespace()
        {
            var code = @"namespace A.B
{
    public enum E
    {
        M1,
        M2,
    }
}";
            var call = SyntaxFactoryWriter.Serialize(code);
            var expected = @"SyntaxFactory.CompilationUnit(
    externs: default,
    usings: default,
    attributeLists: default,
    members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
        SyntaxFactory.NamespaceDeclaration(
            namespaceKeyword: SyntaxFactory.Token(
                leading: default,
                kind: SyntaxKind.NamespaceKeyword,
                trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space)),
            name: SyntaxFactory.QualifiedName(
                left: SyntaxFactory.IdentifierName(
                    identifier: SyntaxFactory.Identifier(
                        leading: default,
                        text: ""A"",
                        trailing: default)),
                dotToken: SyntaxFactory.Token(SyntaxKind.DotToken),
                right: SyntaxFactory.IdentifierName(
                    identifier: SyntaxFactory.Identifier(
                        leading: default,
                        text: ""B"",
                        trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)))),
            openBraceToken: SyntaxFactory.Token(
                leading: default,
                kind: SyntaxKind.OpenBraceToken,
                trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),
            externs: default,
            usings: default,
            members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                SyntaxFactory.EnumDeclaration(
                    attributeLists: default,
                    modifiers: SyntaxFactory.TokenList(
                        SyntaxFactory.Token(
                            leading: SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(""    "")),
                            kind: SyntaxKind.PublicKeyword,
                            trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space))),
                    enumKeyword: SyntaxFactory.Token(
                        leading: default,
                        kind: SyntaxKind.EnumKeyword,
                        trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space)),
                    identifier: SyntaxFactory.Identifier(
                        leading: default,
                        text: ""E"",
                        trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),
                    baseList: default,
                    openBraceToken: SyntaxFactory.Token(
                        leading: SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(""    "")),
                        kind: SyntaxKind.OpenBraceToken,
                        trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),
                    members: default,
                    closeBraceToken: SyntaxFactory.Token(
                        leading: SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(""    "")),
                        kind: SyntaxKind.CloseBraceToken,
                        trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),
                    semicolonToken: default)),
            closeBraceToken: SyntaxFactory.Token(SyntaxKind.CloseBraceToken),
            semicolonToken: default)),
    endOfFileToken: SyntaxFactory.Token(SyntaxKind.EndOfFileToken))";
            CodeAssert.AreEqual(expected, call);
            await AssertRoundtrip(code).ConfigureAwait(false);
        }

        [Explicit("Fix later.")]
        [Test]
        public static async Task EnumWithDocsInNamespace()
        {
            var code = @"namespace Gu.Roslyn.Asserts
{
    /// <summary>
    /// Specifies if a code fix is allowed to introduce compiler errors.
    /// </summary>
    public enum AllowCompilationErrors
    {
        /// <summary>
        /// Nope.
        /// </summary>
        No,

        /// <summary>
        /// Yes!
        /// </summary>
        Yes,
    }
}";
            var call = SyntaxFactoryWriter.Serialize(code);
            var expected = @"SyntaxFactory.CompilationUnit(
    externs: default,
    usings: default,
    attributeLists: default,
    members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
        SyntaxFactory.NamespaceDeclaration(
            namespaceKeyword: SyntaxFactory.Token(
                leading: default,
                kind: SyntaxKind.NamespaceKeyword,
                trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space)),
            name: SyntaxFactory.QualifiedName(
                left: SyntaxFactory.QualifiedName(
                    left: SyntaxFactory.IdentifierName(
                        identifier: SyntaxFactory.Identifier(
                            leading: default,
                            text: ""Gu"",
                            trailing: default)),
                    dotToken: SyntaxFactory.Token(SyntaxKind.DotToken),
                    right: SyntaxFactory.IdentifierName(
                        identifier: SyntaxFactory.Identifier(
                            leading: default,
                            text: ""Roslyn"",
                            trailing: default))),
                dotToken: SyntaxFactory.Token(SyntaxKind.DotToken),
                right: SyntaxFactory.IdentifierName(
                    identifier: SyntaxFactory.Identifier(
                        leading: default,
                        text: ""Asserts"",
                        trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)))),
            openBraceToken: SyntaxFactory.Token(
                leading: default,
                kind: SyntaxKind.OpenBraceToken,
                trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),
            externs: default,
            usings: default,
            members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                SyntaxFactory.EnumDeclaration(
                    attributeLists: default,
                    modifiers: SyntaxFactory.TokenList(
                        SyntaxFactory.Token(
                            leading: SyntaxFactory.TriviaList(
                                SyntaxFactory.Whitespace(""    ""),
                                SyntaxFactory.Trivia(
                                    SyntaxFactory.DocumentationCommentTrivia(
                                        kind: SyntaxKind.SingleLineDocumentationCommentTrivia,
                                        content: SyntaxFactory.List(
                                            new XmlNodeSyntax[]
                                            {
                                                SyntaxFactory.XmlText(
                                                    textTokens: SyntaxFactory.TokenList(
                                                        SyntaxFactory.XmlEntity(
                                                            leading: SyntaxFactory.TriviaList(
                                                                SyntaxFactory.DocumentationCommentExterior(""///"")),
                                                            text: "" "",
                                                            value: "" "",
                                                            trailing: default))),
                                                SyntaxFactory.XmlElement(
                                                    startTag: SyntaxFactory.XmlElementStartTag(
                                                        lessThanToken: SyntaxFactory.Token(SyntaxKind.LessThanToken),
                                                        name: SyntaxFactory.XmlName(
                                                            prefix: default,
                                                            localName: SyntaxFactory.Identifier(
                                                                leading: default,
                                                                text: ""summary"",
                                                                trailing: default)),
                                                        attributes: default,
                                                        greaterThanToken: SyntaxFactory.Token(SyntaxKind.GreaterThanToken)),
                                                    content: SyntaxFactory.SingletonList<XmlNodeSyntax>(
                                                        SyntaxFactory.XmlText(
                                                            textTokens: SyntaxFactory.TokenList(
                                                                SyntaxFactory.XmlTextNewLine(
                                                                    leading: default,
                                                                    text: ""\r\n"",
                                                                    value: ""\r\n"",
                                                                    trailing: default),
                                                                SyntaxFactory.XmlEntity(
                                                                    leading: SyntaxFactory.TriviaList(
                                                                        SyntaxFactory.DocumentationCommentExterior(""    ///"")),
                                                                    text: "" Specifies if a code fix is allowed to introduce compiler errors."",
                                                                    value: "" Specifies if a code fix is allowed to introduce compiler errors."",
                                                                    trailing: default),
                                                                SyntaxFactory.XmlTextNewLine(
                                                                    leading: default,
                                                                    text: ""\r\n"",
                                                                    value: ""\r\n"",
                                                                    trailing: default),
                                                                SyntaxFactory.XmlEntity(
                                                                    leading: SyntaxFactory.TriviaList(
                                                                        SyntaxFactory.DocumentationCommentExterior(""    ///"")),
                                                                    text: "" "",
                                                                    value: "" "",
                                                                    trailing: default)))),
                                                    endTag: SyntaxFactory.XmlElementEndTag(
                                                        lessThanSlashToken: SyntaxFactory.Token(SyntaxKind.LessThanSlashToken),
                                                        name: SyntaxFactory.XmlName(
                                                            prefix: default,
                                                            localName: SyntaxFactory.Identifier(
                                                                leading: default,
                                                                text: ""summary"",
                                                                trailing: default)),
                                                        greaterThanToken: SyntaxFactory.Token(SyntaxKind.GreaterThanToken))),
                                                SyntaxFactory.XmlText(
                                                    textTokens: SyntaxFactory.TokenList(
                                                        SyntaxFactory.XmlTextNewLine(
                                                            leading: default,
                                                            text: ""\r\n"",
                                                            value: ""\r\n"",
                                                            trailing: default))),
                                            }),
                                        endOfComment: SyntaxFactory.Token(SyntaxKind.EndOfDocumentationCommentToken))),
                                SyntaxFactory.Whitespace(""    "")),
                            kind: SyntaxKind.PublicKeyword,
                            trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space))),
                    enumKeyword: SyntaxFactory.Token(
                        leading: default,
                        kind: SyntaxKind.EnumKeyword,
                        trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space)),
                    identifier: SyntaxFactory.Identifier(
                        leading: default,
                        text: ""AllowCompilationErrors"",
                        trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),
                    baseList: default,
                    openBraceToken: SyntaxFactory.Token(
                        leading: SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(""    "")),
                        kind: SyntaxKind.OpenBraceToken,
                        trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),
                    members: SyntaxFactory.SeparatedList<EnumMemberDeclarationSyntax>(
                        SyntaxFactory.EnumMemberDeclaration(
                            attributeLists: default,
                            identifier: SyntaxFactory.Identifier(
                                leading: SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(""        "")),
                                text: ""No"",
                                trailing: default),
                            equalsValue: default),
                        SyntaxFactory.EnumMemberDeclaration(
                            attributeLists: default,
                            identifier: SyntaxFactory.Identifier(
                                leading: SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(""        "")),
                                text: ""Yes"",
                                trailing: default),
                            equalsValue: default)),
                    closeBraceToken: SyntaxFactory.Token(
                        leading: SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(""    "")),
                        kind: SyntaxKind.CloseBraceToken,
                        trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),
                    semicolonToken: default)),
            closeBraceToken: SyntaxFactory.Token(
                leading: default,
                kind: SyntaxKind.CloseBraceToken,
                trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),
            semicolonToken: default)),
    endOfFileToken: SyntaxFactory.Token(SyntaxKind.EndOfFileToken))";
            CodeAssert.AreEqual(expected, call);
            await AssertRoundtrip(code).ConfigureAwait(false);
        }

        [Test]
        public static async Task ClassWithCommentInNamespace()
        {
            var code = @"namespace A.B
{
    /// <summary>
    /// Extension methods for <see cref=""Gu.Inject.Container{T}"" />.
    /// This file is generated by Gu.Inject.Analyzers.
    /// </summary>
    // <auto-generated/>
    // text
    public class C
    {
    }
}";
            var call = SyntaxFactoryWriter.Serialize(code);
            var expected = @"SyntaxFactory.CompilationUnit(
    externs: default,
    usings: default,
    attributeLists: default,
    members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
        SyntaxFactory.NamespaceDeclaration(
            namespaceKeyword: SyntaxFactory.Token(
                leading: default,
                kind: SyntaxKind.NamespaceKeyword,
                trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space)),
            name: SyntaxFactory.QualifiedName(
                left: SyntaxFactory.IdentifierName(
                    identifier: SyntaxFactory.Identifier(
                        leading: default,
                        text: ""A"",
                        trailing: default)),
                dotToken: SyntaxFactory.Token(SyntaxKind.DotToken),
                right: SyntaxFactory.IdentifierName(
                    identifier: SyntaxFactory.Identifier(
                        leading: default,
                        text: ""B"",
                        trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)))),
            openBraceToken: SyntaxFactory.Token(
                leading: default,
                kind: SyntaxKind.OpenBraceToken,
                trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),
            externs: default,
            usings: default,
            members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                SyntaxFactory.ClassDeclaration(
                    attributeLists: default,
                    modifiers: SyntaxFactory.TokenList(
                        SyntaxFactory.Token(
                            leading: SyntaxFactory.TriviaList(
                                SyntaxFactory.Whitespace(""    ""),
                                SyntaxFactory.Trivia(
                                    SyntaxFactory.DocumentationCommentTrivia(
                                        kind: SyntaxKind.SingleLineDocumentationCommentTrivia,
                                        content: SyntaxFactory.List(
                                            new XmlNodeSyntax[]
                                            {
                                                SyntaxFactory.XmlText(
                                                    textTokens: SyntaxFactory.TokenList(
                                                        SyntaxFactory.XmlEntity(
                                                            leading: SyntaxFactory.TriviaList(
                                                                SyntaxFactory.DocumentationCommentExterior(""///"")),
                                                            text: "" "",
                                                            value: "" "",
                                                            trailing: default))),
                                                SyntaxFactory.XmlElement(
                                                    startTag: SyntaxFactory.XmlElementStartTag(
                                                        lessThanToken: SyntaxFactory.Token(SyntaxKind.LessThanToken),
                                                        name: SyntaxFactory.XmlName(
                                                            prefix: default,
                                                            localName: SyntaxFactory.Identifier(
                                                                leading: default,
                                                                text: ""summary"",
                                                                trailing: default)),
                                                        attributes: default,
                                                        greaterThanToken: SyntaxFactory.Token(SyntaxKind.GreaterThanToken)),
                                                    content: SyntaxFactory.List(
                                                        new XmlNodeSyntax[]
                                                        {
                                                            SyntaxFactory.XmlText(
                                                                textTokens: SyntaxFactory.TokenList(
                                                                    SyntaxFactory.XmlTextNewLine(
                                                                        leading: default,
                                                                        text: ""\r\n"",
                                                                        value: ""\r\n"",
                                                                        trailing: default),
                                                                    SyntaxFactory.XmlEntity(
                                                                        leading: SyntaxFactory.TriviaList(
                                                                            SyntaxFactory.DocumentationCommentExterior(""    ///"")),
                                                                        text: "" Extension methods for "",
                                                                        value: "" Extension methods for "",
                                                                        trailing: default))),
                                                            SyntaxFactory.XmlEmptyElement(
                                                                lessThanToken: SyntaxFactory.Token(SyntaxKind.LessThanToken),
                                                                name: SyntaxFactory.XmlName(
                                                                    prefix: default,
                                                                    localName: SyntaxFactory.Identifier(
                                                                        leading: default,
                                                                        text: ""see"",
                                                                        trailing: default)),
                                                                attributes: SyntaxFactory.SingletonList<XmlAttributeSyntax>(
                                                                    SyntaxFactory.XmlCrefAttribute(
                                                                        name: SyntaxFactory.XmlName(
                                                                            prefix: default,
                                                                            localName: SyntaxFactory.Identifier(
                                                                                leading: SyntaxFactory.TriviaList(SyntaxFactory.Space),
                                                                                text: ""cref"",
                                                                                trailing: default)),
                                                                        equalsToken: SyntaxFactory.Token(SyntaxKind.EqualsToken),
                                                                        startQuoteToken: SyntaxFactory.Token(SyntaxKind.DoubleQuoteToken),
                                                                        cref: SyntaxFactory.QualifiedCref(
                                                                            container: SyntaxFactory.QualifiedName(
                                                                                left: SyntaxFactory.IdentifierName(
                                                                                    identifier: SyntaxFactory.Identifier(
                                                                                        leading: default,
                                                                                        text: ""Gu"",
                                                                                        trailing: default)),
                                                                                dotToken: SyntaxFactory.Token(SyntaxKind.DotToken),
                                                                                right: SyntaxFactory.IdentifierName(
                                                                                    identifier: SyntaxFactory.Identifier(
                                                                                        leading: default,
                                                                                        text: ""Inject"",
                                                                                        trailing: default))),
                                                                            dotToken: SyntaxFactory.Token(SyntaxKind.DotToken),
                                                                            member: SyntaxFactory.NameMemberCref(
                                                                                name: SyntaxFactory.GenericName(
                                                                                    identifier: SyntaxFactory.Identifier(
                                                                                        leading: default,
                                                                                        text: ""Container"",
                                                                                        trailing: default),
                                                                                    typeArgumentList: SyntaxFactory.TypeArgumentList(
                                                                                        lessThanToken: SyntaxFactory.Token(
                                                                                            leading: default,
                                                                                            kind: SyntaxKind.LessThanToken,
                                                                                            text: ""{"",
                                                                                            valueText: ""<"",
                                                                                            trailing: default),
                                                                                        arguments: SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                                                            SyntaxFactory.IdentifierName(
                                                                                                identifier: SyntaxFactory.Identifier(
                                                                                                    leading: default,
                                                                                                    text: ""T"",
                                                                                                    trailing: default))),
                                                                                        greaterThanToken: SyntaxFactory.Token(
                                                                                            leading: default,
                                                                                            kind: SyntaxKind.GreaterThanToken,
                                                                                            text: ""}"",
                                                                                            valueText: "">"",
                                                                                            trailing: default))),
                                                                                parameters: default)),
                                                                        endQuoteToken: SyntaxFactory.Token(SyntaxKind.DoubleQuoteToken))),
                                                                slashGreaterThanToken: SyntaxFactory.Token(
                                                                    leading: SyntaxFactory.TriviaList(SyntaxFactory.Space),
                                                                    kind: SyntaxKind.SlashGreaterThanToken,
                                                                    trailing: default)),
                                                            SyntaxFactory.XmlText(
                                                                textTokens: SyntaxFactory.TokenList(
                                                                    SyntaxFactory.XmlTextLiteral("".""),
                                                                    SyntaxFactory.XmlTextNewLine(
                                                                        leading: default,
                                                                        text: ""\r\n"",
                                                                        value: ""\r\n"",
                                                                        trailing: default),
                                                                    SyntaxFactory.XmlEntity(
                                                                        leading: SyntaxFactory.TriviaList(
                                                                            SyntaxFactory.DocumentationCommentExterior(""    ///"")),
                                                                        text: "" This file is generated by Gu.Inject.Analyzers."",
                                                                        value: "" This file is generated by Gu.Inject.Analyzers."",
                                                                        trailing: default),
                                                                    SyntaxFactory.XmlTextNewLine(
                                                                        leading: default,
                                                                        text: ""\r\n"",
                                                                        value: ""\r\n"",
                                                                        trailing: default),
                                                                    SyntaxFactory.XmlEntity(
                                                                        leading: SyntaxFactory.TriviaList(
                                                                            SyntaxFactory.DocumentationCommentExterior(""    ///"")),
                                                                        text: "" "",
                                                                        value: "" "",
                                                                        trailing: default))),
                                                        }),
                                                    endTag: SyntaxFactory.XmlElementEndTag(
                                                        lessThanSlashToken: SyntaxFactory.Token(SyntaxKind.LessThanSlashToken),
                                                        name: SyntaxFactory.XmlName(
                                                            prefix: default,
                                                            localName: SyntaxFactory.Identifier(
                                                                leading: default,
                                                                text: ""summary"",
                                                                trailing: default)),
                                                        greaterThanToken: SyntaxFactory.Token(SyntaxKind.GreaterThanToken))),
                                                SyntaxFactory.XmlText(
                                                    textTokens: SyntaxFactory.TokenList(
                                                        SyntaxFactory.XmlTextNewLine(
                                                            leading: default,
                                                            text: ""\r\n"",
                                                            value: ""\r\n"",
                                                            trailing: default))),
                                            }),
                                        endOfComment: SyntaxFactory.Token(SyntaxKind.EndOfDocumentationCommentToken))),
                                SyntaxFactory.Whitespace(""    ""),
                                SyntaxFactory.Comment(""// <auto-generated/>""),
                                SyntaxFactory.CarriageReturnLineFeed,
                                SyntaxFactory.Whitespace(""    ""),
                                SyntaxFactory.Comment(""// text""),
                                SyntaxFactory.CarriageReturnLineFeed,
                                SyntaxFactory.Whitespace(""    "")),
                            kind: SyntaxKind.PublicKeyword,
                            trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space))),
                    keyword: SyntaxFactory.Token(
                        leading: default,
                        kind: SyntaxKind.ClassKeyword,
                        trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space)),
                    identifier: SyntaxFactory.Identifier(
                        leading: default,
                        text: ""C"",
                        trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),
                    typeParameterList: default,
                    baseList: default,
                    constraintClauses: default,
                    openBraceToken: SyntaxFactory.Token(
                        leading: SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(""    "")),
                        kind: SyntaxKind.OpenBraceToken,
                        trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),
                    members: default,
                    closeBraceToken: SyntaxFactory.Token(
                        leading: SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(""    "")),
                        kind: SyntaxKind.CloseBraceToken,
                        trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),
                    semicolonToken: default)),
            closeBraceToken: SyntaxFactory.Token(SyntaxKind.CloseBraceToken),
            semicolonToken: default)),
    endOfFileToken: SyntaxFactory.Token(SyntaxKind.EndOfFileToken))";
            CodeAssert.AreEqual(expected, call);
            await AssertRoundtrip(code).ConfigureAwait(false);
        }

        [TestCase("int x = 1")]
        [TestCase("int x = 1 ")]
        [TestCase("long x = 1")]
        [TestCase("long x = 1 ")]
        [TestCase("double x = 1")]
        [TestCase("double x = 1 ")]
        [TestCase("double x = 1.2")]
        [TestCase("double x = 1.2 ")]
        [TestCase("object x = null")]
        [TestCase("var x = true")]
        [TestCase("var x = false")]
        [TestCase("var x = 1")]
        [TestCase("var x = 1.2")]
        [TestCase("string x = \"a\"")]
        [TestCase("string x = \"a\" ")]
        [TestCase("string x = \"\\\"\"")]
        [TestCase("string x = \"\\\"\" ")]
        [TestCase("string x = @\"a\"")]
        [TestCase("string x = @\"a\" ")]
        [TestCase("char x = 'a'")]
        [TestCase("char x = 'a' ")]
        [TestCase("char x = '\\\\'")]
        [TestCase("char x = '\\\\' ")]
        public static async Task Token(string expression)
        {
            var code = @"namespace A
{
    class C
    {
        public C()
        {
            int x = 1;
        }
    }
}".AssertReplace("int x = 1", expression);
            await AssertRoundtrip(code).ConfigureAwait(false);
        }

        private static async Task AssertRoundtrip(string code)
        {
            var call = SyntaxFactoryWriter.Serialize(code);
            var result = await CSharpScript.EvaluateAsync<SyntaxNode>(call, ScriptOptions);
            CodeAssert.AreEqual(code, result.ToString());
        }
    }
}
