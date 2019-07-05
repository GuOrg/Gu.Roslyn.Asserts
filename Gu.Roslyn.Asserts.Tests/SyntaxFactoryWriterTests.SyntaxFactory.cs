namespace Gu.Roslyn.Asserts.Tests
{
    using System;
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
                                                                           .WithImports("Microsoft.CodeAnalysis", "Microsoft.CodeAnalysis.CSharp", "Microsoft.CodeAnalysis.CSharp.Syntax")
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
        public static async Task Class()
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

        [Test]
        public static async Task ClassWithMethod()
        {
            var code = @"namespace A.B
{
    public class C
    {
        public static T Id<T>(T t) => t;
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
                    members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                        SyntaxFactory.MethodDeclaration(
                            attributeLists: default,
                            modifiers: SyntaxFactory.TokenList(
                                SyntaxFactory.Token(
                                    leading: SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(""        "")),
                                    kind: SyntaxKind.PublicKeyword,
                                    trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space)),
                                SyntaxFactory.Token(
                                    leading: default,
                                    kind: SyntaxKind.StaticKeyword,
                                    trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space))),
                            returnType: SyntaxFactory.IdentifierName(
                                identifier: SyntaxFactory.Identifier(
                                    leading: default,
                                    text: ""T"",
                                    trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space))),
                            explicitInterfaceSpecifier: default,
                            identifier: SyntaxFactory.Identifier(
                                leading: default,
                                text: ""Id"",
                                trailing: default),
                            typeParameterList: SyntaxFactory.TypeParameterList(
                                lessThanToken: SyntaxFactory.Token(SyntaxKind.LessThanToken),
                                parameters: SyntaxFactory.SingletonSeparatedList<TypeParameterSyntax>(
                                    SyntaxFactory.TypeParameter(
                                        attributeLists: default,
                                        varianceKeyword: default,
                                        identifier: SyntaxFactory.Identifier(
                                            leading: default,
                                            text: ""T"",
                                            trailing: default))),
                                greaterThanToken: SyntaxFactory.Token(SyntaxKind.GreaterThanToken)),
                            parameterList: SyntaxFactory.ParameterList(
                                openParenToken: SyntaxFactory.Token(SyntaxKind.OpenParenToken),
                                parameters: SyntaxFactory.SingletonSeparatedList<ParameterSyntax>(
                                    SyntaxFactory.Parameter(
                                        attributeLists: default,
                                        modifiers: default,
                                        type: SyntaxFactory.IdentifierName(
                                            identifier: SyntaxFactory.Identifier(
                                                leading: default,
                                                text: ""T"",
                                                trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space))),
                                        identifier: SyntaxFactory.Identifier(
                                            leading: default,
                                            text: ""t"",
                                            trailing: default),
                                        @default: default)),
                                closeParenToken: SyntaxFactory.Token(
                                    leading: default,
                                    kind: SyntaxKind.CloseParenToken,
                                    trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space))),
                            constraintClauses: default,
                            body: default,
                            expressionBody: SyntaxFactory.ArrowExpressionClause(
                                arrowToken: SyntaxFactory.Token(
                                    leading: default,
                                    kind: SyntaxKind.EqualsGreaterThanToken,
                                    trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space)),
                                expression: SyntaxFactory.IdentifierName(
                                    identifier: SyntaxFactory.Identifier(
                                        leading: default,
                                        text: ""t"",
                                        trailing: default))),
                            semicolonToken: SyntaxFactory.Token(
                                leading: default,
                                kind: SyntaxKind.SemicolonToken,
                                trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)))),
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

        [Test]
        public static async Task ClassWithFields()
        {
            var code = @"namespace A
{
    class C
    {
        private readonly int i = 1;
        private readonly string s1 = ""abc"";
        private readonly string s2 = ""abc\\r\\n"";
        private readonly string s3 = $""abc{1}"";
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
            name: SyntaxFactory.IdentifierName(
                identifier: SyntaxFactory.Identifier(
                    leading: default,
                    text: ""A"",
                    trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed))),
            openBraceToken: SyntaxFactory.Token(
                leading: default,
                kind: SyntaxKind.OpenBraceToken,
                trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),
            externs: default,
            usings: default,
            members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                SyntaxFactory.ClassDeclaration(
                    attributeLists: default,
                    modifiers: default,
                    keyword: SyntaxFactory.Token(
                        leading: SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(""    "")),
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
                    members: SyntaxFactory.List(
                        new MemberDeclarationSyntax[]
                        {
                            SyntaxFactory.FieldDeclaration(
                                attributeLists: default,
                                modifiers: SyntaxFactory.TokenList(
                                    SyntaxFactory.Token(
                                        leading: SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(""        "")),
                                        kind: SyntaxKind.PrivateKeyword,
                                        trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space)),
                                    SyntaxFactory.Token(
                                        leading: default,
                                        kind: SyntaxKind.ReadOnlyKeyword,
                                        trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space))),
                                declaration: SyntaxFactory.VariableDeclaration(
                                    type: SyntaxFactory.PredefinedType(
                                        keyword: SyntaxFactory.Token(
                                            leading: default,
                                            kind: SyntaxKind.IntKeyword,
                                            trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space))),
                                    variables: SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                                        SyntaxFactory.VariableDeclarator(
                                            identifier: SyntaxFactory.Identifier(
                                                leading: default,
                                                text: ""i"",
                                                trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space)),
                                            argumentList: default,
                                            initializer: SyntaxFactory.EqualsValueClause(
                                                equalsToken: SyntaxFactory.Token(
                                                    leading: default,
                                                    kind: SyntaxKind.EqualsToken,
                                                    trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space)),
                                                value: SyntaxFactory.LiteralExpression(
                                                    kind: SyntaxKind.NumericLiteralExpression,
                                                    token: SyntaxFactory.Literal(1)))))),
                                semicolonToken: SyntaxFactory.Token(
                                    leading: default,
                                    kind: SyntaxKind.SemicolonToken,
                                    trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed))),
                            SyntaxFactory.FieldDeclaration(
                                attributeLists: default,
                                modifiers: SyntaxFactory.TokenList(
                                    SyntaxFactory.Token(
                                        leading: SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(""        "")),
                                        kind: SyntaxKind.PrivateKeyword,
                                        trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space)),
                                    SyntaxFactory.Token(
                                        leading: default,
                                        kind: SyntaxKind.ReadOnlyKeyword,
                                        trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space))),
                                declaration: SyntaxFactory.VariableDeclaration(
                                    type: SyntaxFactory.PredefinedType(
                                        keyword: SyntaxFactory.Token(
                                            leading: default,
                                            kind: SyntaxKind.StringKeyword,
                                            trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space))),
                                    variables: SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                                        SyntaxFactory.VariableDeclarator(
                                            identifier: SyntaxFactory.Identifier(
                                                leading: default,
                                                text: ""s1"",
                                                trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space)),
                                            argumentList: default,
                                            initializer: SyntaxFactory.EqualsValueClause(
                                                equalsToken: SyntaxFactory.Token(
                                                    leading: default,
                                                    kind: SyntaxKind.EqualsToken,
                                                    trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space)),
                                                value: SyntaxFactory.LiteralExpression(
                                                    kind: SyntaxKind.StringLiteralExpression,
                                                    token: SyntaxFactory.Literal(
                                                        text: ""\""abc\"""",
                                                        value: ""abc"")))))),
                                semicolonToken: SyntaxFactory.Token(
                                    leading: default,
                                    kind: SyntaxKind.SemicolonToken,
                                    trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed))),
                            SyntaxFactory.FieldDeclaration(
                                attributeLists: default,
                                modifiers: SyntaxFactory.TokenList(
                                    SyntaxFactory.Token(
                                        leading: SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(""        "")),
                                        kind: SyntaxKind.PrivateKeyword,
                                        trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space)),
                                    SyntaxFactory.Token(
                                        leading: default,
                                        kind: SyntaxKind.ReadOnlyKeyword,
                                        trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space))),
                                declaration: SyntaxFactory.VariableDeclaration(
                                    type: SyntaxFactory.PredefinedType(
                                        keyword: SyntaxFactory.Token(
                                            leading: default,
                                            kind: SyntaxKind.StringKeyword,
                                            trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space))),
                                    variables: SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                                        SyntaxFactory.VariableDeclarator(
                                            identifier: SyntaxFactory.Identifier(
                                                leading: default,
                                                text: ""s2"",
                                                trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space)),
                                            argumentList: default,
                                            initializer: SyntaxFactory.EqualsValueClause(
                                                equalsToken: SyntaxFactory.Token(
                                                    leading: default,
                                                    kind: SyntaxKind.EqualsToken,
                                                    trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space)),
                                                value: SyntaxFactory.LiteralExpression(
                                                    kind: SyntaxKind.StringLiteralExpression,
                                                    token: SyntaxFactory.Literal(
                                                        text: ""\""abc\\\\r\\\\n\"""",
                                                        value: ""abc\\r\\n"")))))),
                                semicolonToken: SyntaxFactory.Token(
                                    leading: default,
                                    kind: SyntaxKind.SemicolonToken,
                                    trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed))),
                            SyntaxFactory.FieldDeclaration(
                                attributeLists: default,
                                modifiers: SyntaxFactory.TokenList(
                                    SyntaxFactory.Token(
                                        leading: SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(""        "")),
                                        kind: SyntaxKind.PrivateKeyword,
                                        trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space)),
                                    SyntaxFactory.Token(
                                        leading: default,
                                        kind: SyntaxKind.ReadOnlyKeyword,
                                        trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space))),
                                declaration: SyntaxFactory.VariableDeclaration(
                                    type: SyntaxFactory.PredefinedType(
                                        keyword: SyntaxFactory.Token(
                                            leading: default,
                                            kind: SyntaxKind.StringKeyword,
                                            trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space))),
                                    variables: SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                                        SyntaxFactory.VariableDeclarator(
                                            identifier: SyntaxFactory.Identifier(
                                                leading: default,
                                                text: ""s3"",
                                                trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space)),
                                            argumentList: default,
                                            initializer: SyntaxFactory.EqualsValueClause(
                                                equalsToken: SyntaxFactory.Token(
                                                    leading: default,
                                                    kind: SyntaxKind.EqualsToken,
                                                    trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space)),
                                                value: SyntaxFactory.InterpolatedStringExpression(
                                                    stringStartToken: SyntaxFactory.Token(SyntaxKind.InterpolatedStringStartToken),
                                                    contents: SyntaxFactory.List(
                                                        new InterpolatedStringContentSyntax[]
                                                        {
                                                            SyntaxFactory.InterpolatedStringText(
                                                                textToken: SyntaxFactory.Token(
                                                                    leading: default,
                                                                    kind: SyntaxKind.InterpolatedStringTextToken,
                                                                    text: ""abc"",
                                                                    valueText: ""abc"",
                                                                    trailing: default)),
                                                            SyntaxFactory.Interpolation(
                                                                openBraceToken: SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
                                                                expression: SyntaxFactory.LiteralExpression(
                                                                    kind: SyntaxKind.NumericLiteralExpression,
                                                                    token: SyntaxFactory.Literal(1)),
                                                                alignmentClause: default,
                                                                formatClause: default,
                                                                closeBraceToken: SyntaxFactory.Token(SyntaxKind.CloseBraceToken)),
                                                        }),
                                                    stringEndToken: SyntaxFactory.Token(SyntaxKind.InterpolatedStringEndToken)))))),
                                semicolonToken: SyntaxFactory.Token(
                                    leading: default,
                                    kind: SyntaxKind.SemicolonToken,
                                    trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed))),
                        }),
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

        [Test]
        public static async Task ClassWithDocs()
        {
            var code = @"// ReSharper disable InconsistentNaming
namespace A.B
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
                leading: SyntaxFactory.TriviaList(
                    SyntaxFactory.Comment(""// ReSharper disable InconsistentNaming""),
                    SyntaxFactory.LineFeed),
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
                                SyntaxFactory.LineFeed,
                                SyntaxFactory.Whitespace(""    ""),
                                SyntaxFactory.Comment(""// text""),
                                SyntaxFactory.LineFeed,
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

        [Test]
        public static async Task EnumSingleMember()
        {
            var code = @"namespace A
{
    public enum E
    {
        M
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
            name: SyntaxFactory.IdentifierName(
                identifier: SyntaxFactory.Identifier(
                    leading: default,
                    text: ""A"",
                    trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed))),
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
                    members: SyntaxFactory.SingletonSeparatedList<EnumMemberDeclarationSyntax>(
                        SyntaxFactory.EnumMemberDeclaration(
                            attributeLists: default,
                            identifier: SyntaxFactory.Identifier(
                                leading: SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(""        "")),
                                text: ""M"",
                                trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),
                            equalsValue: default)),
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

        [Test]
        public static async Task Enum()
        {
            var code = @"namespace A
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
            name: SyntaxFactory.IdentifierName(
                identifier: SyntaxFactory.Identifier(
                    leading: default,
                    text: ""A"",
                    trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed))),
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
                    members: SyntaxFactory.SeparatedList(
                        new EnumMemberDeclarationSyntax[]
                        {
                            SyntaxFactory.EnumMemberDeclaration(
                                attributeLists: default,
                                identifier: SyntaxFactory.Identifier(
                                    leading: SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(""        "")),
                                    text: ""M1"",
                                    trailing: default),
                                equalsValue: default),
                            SyntaxFactory.EnumMemberDeclaration(
                                attributeLists: default,
                                identifier: SyntaxFactory.Identifier(
                                    leading: SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(""        "")),
                                    text: ""M2"",
                                    trailing: default),
                                equalsValue: default),
                        },
                        new SyntaxToken[]
                        {
                            SyntaxFactory.Token(
                                leading: default,
                                kind: SyntaxKind.CommaToken,
                                trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),
                            SyntaxFactory.Token(
                                leading: default,
                                kind: SyntaxKind.CommaToken,
                                trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),
                        }),
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

        [Test]
        public static async Task EnumWithDocs()
        {
            var code = @"namespace A
{
    /// <summary>
    /// Summary
    /// </summary>
    public enum E
    {
        /// <summary>
        /// M1.
        /// </summary>
        M1,

        /// <summary>
        /// M2!
        /// </summary>
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
            name: SyntaxFactory.IdentifierName(
                identifier: SyntaxFactory.Identifier(
                    leading: default,
                    text: ""A"",
                    trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed))),
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
                                                                    text: "" Summary"",
                                                                    value: "" Summary"",
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
                        text: ""E"",
                        trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),
                    baseList: default,
                    openBraceToken: SyntaxFactory.Token(
                        leading: SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(""    "")),
                        kind: SyntaxKind.OpenBraceToken,
                        trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),
                    members: SyntaxFactory.SeparatedList(
                        new EnumMemberDeclarationSyntax[]
                        {
                            SyntaxFactory.EnumMemberDeclaration(
                                attributeLists: default,
                                identifier: SyntaxFactory.Identifier(
                                    leading: SyntaxFactory.TriviaList(
                                        SyntaxFactory.Whitespace(""        ""),
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
                                                                                SyntaxFactory.DocumentationCommentExterior(""        ///"")),
                                                                            text: "" M1."",
                                                                            value: "" M1."",
                                                                            trailing: default),
                                                                        SyntaxFactory.XmlTextNewLine(
                                                                            leading: default,
                                                                            text: ""\r\n"",
                                                                            value: ""\r\n"",
                                                                            trailing: default),
                                                                        SyntaxFactory.XmlEntity(
                                                                            leading: SyntaxFactory.TriviaList(
                                                                                SyntaxFactory.DocumentationCommentExterior(""        ///"")),
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
                                        SyntaxFactory.Whitespace(""        "")),
                                    text: ""M1"",
                                    trailing: default),
                                equalsValue: default),
                            SyntaxFactory.EnumMemberDeclaration(
                                attributeLists: default,
                                identifier: SyntaxFactory.Identifier(
                                    leading: SyntaxFactory.TriviaList(
                                        SyntaxFactory.LineFeed,
                                        SyntaxFactory.Whitespace(""        ""),
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
                                                                                SyntaxFactory.DocumentationCommentExterior(""        ///"")),
                                                                            text: "" M2!"",
                                                                            value: "" M2!"",
                                                                            trailing: default),
                                                                        SyntaxFactory.XmlTextNewLine(
                                                                            leading: default,
                                                                            text: ""\r\n"",
                                                                            value: ""\r\n"",
                                                                            trailing: default),
                                                                        SyntaxFactory.XmlEntity(
                                                                            leading: SyntaxFactory.TriviaList(
                                                                                SyntaxFactory.DocumentationCommentExterior(""        ///"")),
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
                                        SyntaxFactory.Whitespace(""        "")),
                                    text: ""M2"",
                                    trailing: default),
                                equalsValue: default),
                        },
                        new SyntaxToken[]
                        {
                            SyntaxFactory.Token(
                                leading: default,
                                kind: SyntaxKind.CommaToken,
                                trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),
                            SyntaxFactory.Token(
                                leading: default,
                                kind: SyntaxKind.CommaToken,
                                trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),
                        }),
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

        [Test]
        public static async Task DeepNesting()
        {
            var code = @"namespace A
{
    class C
    {
        static string M() => typeof(C).ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString()
                                      .ToString();
    }
}";
            //// Checking so that we don't get StackOverflowException here.
            Assert.DoesNotThrow(() => SyntaxFactoryWriter.Serialize(code));
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
        [TestCase("object x = null ")]
        [TestCase("var x = true")]
        [TestCase("var x = true ")]
        [TestCase("var x = false")]
        [TestCase("var x = 1")]
        [TestCase("var x = 1 ")]
        [TestCase("var @default = 1")]
        [TestCase("var x = 1.2")]
        [TestCase("var x = 1.2 ")]
        [TestCase("string x = \"a\"")]
        [TestCase("string x = \"a\" ")]
        [TestCase("string x = $\"a{1}\"")]
        [TestCase("string x = $\"a{1}\" ")]
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
            try
            {
                var result = await CSharpScript.EvaluateAsync<SyntaxNode>(call, ScriptOptions);
                CodeAssert.AreEqual(code, result.ToFullString());
            }
            catch (CompilationErrorException e)
            {
                Console.Write(call);
                throw;
            }
        }
    }
}
