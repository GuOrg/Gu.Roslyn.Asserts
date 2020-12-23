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

    public static class DefaultTrivia
    {
        private static readonly ScriptOptions ScriptOptions = ScriptOptions.Default
                                                                           .WithReferences(Gu.Roslyn.Asserts.MetadataReferences.Transitive(typeof(SyntaxFactory)))
                                                                           .WithImports("Microsoft.CodeAnalysis", "Microsoft.CodeAnalysis.CSharp", "Microsoft.CodeAnalysis.CSharp.Syntax")
                                                                           .WithEmitDebugInformation(emitDebugInformation: true);

        private static readonly IReadOnlyList<FileInfo> CSharpFiles = SolutionFile.Find("Gu.Roslyn.Asserts.sln")
                                                                                  .Directory.EnumerateFiles("*.cs", SearchOption.AllDirectories)
                                                                                  .ToArray();

        private static readonly SyntaxFactoryWriterSettings Settings = new SyntaxFactoryWriterSettings(defaultTrivia: true);

        [Explicit("Fails on AppVeyor?")]
        [TestCaseSource(nameof(CSharpFiles))]
        public static async Task Roundtrip(FileInfo file)
        {
            var code = File.ReadAllText(file.FullName);
            await AssertRoundtrip(code).ConfigureAwait(false);
        }

        [Test]
        public static async Task Class()
        {
            var code = @"
namespace A.B
{
    public class C
    {
    }
}";

            var call = SyntaxFactoryWriter.Serialize(code, Settings);
            var expected = @"SyntaxFactory.CompilationUnit(
    externs: default,
    usings: default,
    attributeLists: default,
    members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
        SyntaxFactory.NamespaceDeclaration(
            attributeLists: default,
            modifiers: default,
            namespaceKeyword: SyntaxFactory.Token(SyntaxKind.NamespaceKeyword),
            name: SyntaxFactory.QualifiedName(
                left: SyntaxFactory.IdentifierName(
                    identifier: SyntaxFactory.Identifier(""A"")),
                dotToken: SyntaxFactory.Token(SyntaxKind.DotToken),
                right: SyntaxFactory.IdentifierName(
                    identifier: SyntaxFactory.Identifier(""B""))),
            openBraceToken: SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
            externs: default,
            usings: default,
            members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                SyntaxFactory.ClassDeclaration(
                    attributeLists: default,
                    modifiers: SyntaxFactory.TokenList(
                        SyntaxFactory.Token(SyntaxKind.PublicKeyword)),
                    keyword: SyntaxFactory.Token(SyntaxKind.ClassKeyword),
                    identifier: SyntaxFactory.Identifier(""C""),
                    typeParameterList: default,
                    baseList: default,
                    constraintClauses: default,
                    openBraceToken: SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
                    members: default,
                    closeBraceToken: SyntaxFactory.Token(SyntaxKind.CloseBraceToken),
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
            var call = SyntaxFactoryWriter.Serialize(code, Settings);
            var expected = @"SyntaxFactory.CompilationUnit(
    externs: default,
    usings: default,
    attributeLists: default,
    members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
        SyntaxFactory.NamespaceDeclaration(
            attributeLists: default,
            modifiers: default,
            namespaceKeyword: SyntaxFactory.Token(SyntaxKind.NamespaceKeyword),
            name: SyntaxFactory.QualifiedName(
                left: SyntaxFactory.IdentifierName(
                    identifier: SyntaxFactory.Identifier(""A"")),
                dotToken: SyntaxFactory.Token(SyntaxKind.DotToken),
                right: SyntaxFactory.IdentifierName(
                    identifier: SyntaxFactory.Identifier(""B""))),
            openBraceToken: SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
            externs: default,
            usings: default,
            members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                SyntaxFactory.ClassDeclaration(
                    attributeLists: default,
                    modifiers: SyntaxFactory.TokenList(
                        SyntaxFactory.Token(SyntaxKind.PublicKeyword)),
                    keyword: SyntaxFactory.Token(SyntaxKind.ClassKeyword),
                    identifier: SyntaxFactory.Identifier(""C""),
                    typeParameterList: default,
                    baseList: default,
                    constraintClauses: default,
                    openBraceToken: SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
                    members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                        SyntaxFactory.MethodDeclaration(
                            attributeLists: default,
                            modifiers: SyntaxFactory.TokenList(
                                SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                                SyntaxFactory.Token(SyntaxKind.StaticKeyword)),
                            returnType: SyntaxFactory.IdentifierName(
                                identifier: SyntaxFactory.Identifier(""T"")),
                            explicitInterfaceSpecifier: default,
                            identifier: SyntaxFactory.Identifier(""Id""),
                            typeParameterList: SyntaxFactory.TypeParameterList(
                                lessThanToken: SyntaxFactory.Token(SyntaxKind.LessThanToken),
                                parameters: SyntaxFactory.SingletonSeparatedList<TypeParameterSyntax>(
                                    SyntaxFactory.TypeParameter(
                                        attributeLists: default,
                                        varianceKeyword: default,
                                        identifier: SyntaxFactory.Identifier(""T""))),
                                greaterThanToken: SyntaxFactory.Token(SyntaxKind.GreaterThanToken)),
                            parameterList: SyntaxFactory.ParameterList(
                                openParenToken: SyntaxFactory.Token(SyntaxKind.OpenParenToken),
                                parameters: SyntaxFactory.SingletonSeparatedList<ParameterSyntax>(
                                    SyntaxFactory.Parameter(
                                        attributeLists: default,
                                        modifiers: default,
                                        type: SyntaxFactory.IdentifierName(
                                            identifier: SyntaxFactory.Identifier(""T"")),
                                        identifier: SyntaxFactory.Identifier(""t""),
                                        @default: default)),
                                closeParenToken: SyntaxFactory.Token(SyntaxKind.CloseParenToken)),
                            constraintClauses: default,
                            body: default,
                            expressionBody: SyntaxFactory.ArrowExpressionClause(
                                arrowToken: SyntaxFactory.Token(SyntaxKind.EqualsGreaterThanToken),
                                expression: SyntaxFactory.IdentifierName(
                                    identifier: SyntaxFactory.Identifier(""t""))),
                            semicolonToken: SyntaxFactory.Token(SyntaxKind.SemicolonToken))),
                    closeBraceToken: SyntaxFactory.Token(SyntaxKind.CloseBraceToken),
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
            var call = SyntaxFactoryWriter.Serialize(code, Settings);
            var expected = @"SyntaxFactory.CompilationUnit(
    externs: default,
    usings: default,
    attributeLists: default,
    members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
        SyntaxFactory.NamespaceDeclaration(
            attributeLists: default,
            modifiers: default,
            namespaceKeyword: SyntaxFactory.Token(SyntaxKind.NamespaceKeyword),
            name: SyntaxFactory.IdentifierName(
                identifier: SyntaxFactory.Identifier(""A"")),
            openBraceToken: SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
            externs: default,
            usings: default,
            members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                SyntaxFactory.ClassDeclaration(
                    attributeLists: default,
                    modifiers: default,
                    keyword: SyntaxFactory.Token(SyntaxKind.ClassKeyword),
                    identifier: SyntaxFactory.Identifier(""C""),
                    typeParameterList: default,
                    baseList: default,
                    constraintClauses: default,
                    openBraceToken: SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
                    members: SyntaxFactory.List(
                        new MemberDeclarationSyntax[]
                        {
                            SyntaxFactory.FieldDeclaration(
                                attributeLists: default,
                                modifiers: SyntaxFactory.TokenList(
                                    SyntaxFactory.Token(SyntaxKind.PrivateKeyword),
                                    SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)),
                                declaration: SyntaxFactory.VariableDeclaration(
                                    type: SyntaxFactory.PredefinedType(
                                        keyword: SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                                    variables: SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                                        SyntaxFactory.VariableDeclarator(
                                            identifier: SyntaxFactory.Identifier(""i""),
                                            argumentList: default,
                                            initializer: SyntaxFactory.EqualsValueClause(
                                                equalsToken: SyntaxFactory.Token(SyntaxKind.EqualsToken),
                                                value: SyntaxFactory.LiteralExpression(
                                                    kind: SyntaxKind.NumericLiteralExpression,
                                                    token: SyntaxFactory.Literal(
                                                        text: ""1"",
                                                        value: 1)))))),
                                semicolonToken: SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                            SyntaxFactory.FieldDeclaration(
                                attributeLists: default,
                                modifiers: SyntaxFactory.TokenList(
                                    SyntaxFactory.Token(SyntaxKind.PrivateKeyword),
                                    SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)),
                                declaration: SyntaxFactory.VariableDeclaration(
                                    type: SyntaxFactory.PredefinedType(
                                        keyword: SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                                    variables: SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                                        SyntaxFactory.VariableDeclarator(
                                            identifier: SyntaxFactory.Identifier(""s1""),
                                            argumentList: default,
                                            initializer: SyntaxFactory.EqualsValueClause(
                                                equalsToken: SyntaxFactory.Token(SyntaxKind.EqualsToken),
                                                value: SyntaxFactory.LiteralExpression(
                                                    kind: SyntaxKind.StringLiteralExpression,
                                                    token: SyntaxFactory.Literal(
                                                        text: ""\""abc\"""",
                                                        value: ""abc"")))))),
                                semicolonToken: SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                            SyntaxFactory.FieldDeclaration(
                                attributeLists: default,
                                modifiers: SyntaxFactory.TokenList(
                                    SyntaxFactory.Token(SyntaxKind.PrivateKeyword),
                                    SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)),
                                declaration: SyntaxFactory.VariableDeclaration(
                                    type: SyntaxFactory.PredefinedType(
                                        keyword: SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                                    variables: SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                                        SyntaxFactory.VariableDeclarator(
                                            identifier: SyntaxFactory.Identifier(""s2""),
                                            argumentList: default,
                                            initializer: SyntaxFactory.EqualsValueClause(
                                                equalsToken: SyntaxFactory.Token(SyntaxKind.EqualsToken),
                                                value: SyntaxFactory.LiteralExpression(
                                                    kind: SyntaxKind.StringLiteralExpression,
                                                    token: SyntaxFactory.Literal(
                                                        text: ""\""abc\\\\r\\\\n\"""",
                                                        value: ""abc\\r\\n"")))))),
                                semicolonToken: SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                            SyntaxFactory.FieldDeclaration(
                                attributeLists: default,
                                modifiers: SyntaxFactory.TokenList(
                                    SyntaxFactory.Token(SyntaxKind.PrivateKeyword),
                                    SyntaxFactory.Token(SyntaxKind.ReadOnlyKeyword)),
                                declaration: SyntaxFactory.VariableDeclaration(
                                    type: SyntaxFactory.PredefinedType(
                                        keyword: SyntaxFactory.Token(SyntaxKind.StringKeyword)),
                                    variables: SyntaxFactory.SingletonSeparatedList<VariableDeclaratorSyntax>(
                                        SyntaxFactory.VariableDeclarator(
                                            identifier: SyntaxFactory.Identifier(""s3""),
                                            argumentList: default,
                                            initializer: SyntaxFactory.EqualsValueClause(
                                                equalsToken: SyntaxFactory.Token(SyntaxKind.EqualsToken),
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
                                                                    token: SyntaxFactory.Literal(
                                                                        text: ""1"",
                                                                        value: 1)),
                                                                alignmentClause: default,
                                                                formatClause: default,
                                                                closeBraceToken: SyntaxFactory.Token(SyntaxKind.CloseBraceToken)),
                                                        }),
                                                    stringEndToken: SyntaxFactory.Token(SyntaxKind.InterpolatedStringEndToken)))))),
                                semicolonToken: SyntaxFactory.Token(SyntaxKind.SemicolonToken)),
                        }),
                    closeBraceToken: SyntaxFactory.Token(SyntaxKind.CloseBraceToken),
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
            var call = SyntaxFactoryWriter.Serialize(code, Settings);
            var expected = @"SyntaxFactory.CompilationUnit(
    externs: default,
    usings: default,
    attributeLists: default,
    members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
        SyntaxFactory.NamespaceDeclaration(
            attributeLists: default,
            modifiers: default,
            namespaceKeyword: SyntaxFactory.Token(
                leading: SyntaxFactory.TriviaList(
                    SyntaxFactory.Comment(""// ReSharper disable InconsistentNaming""),
                    SyntaxFactory.LineFeed),
                kind: SyntaxKind.NamespaceKeyword,
                trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space)),
            name: SyntaxFactory.QualifiedName(
                left: SyntaxFactory.IdentifierName(
                    identifier: SyntaxFactory.Identifier(""A"")),
                dotToken: SyntaxFactory.Token(SyntaxKind.DotToken),
                right: SyntaxFactory.IdentifierName(
                    identifier: SyntaxFactory.Identifier(""B""))),
            openBraceToken: SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
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
                                                        SyntaxFactory.XmlTextLiteral(
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
                                                            localName: SyntaxFactory.Identifier(""summary"")),
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
                                                                    SyntaxFactory.XmlTextLiteral(
                                                                        leading: SyntaxFactory.TriviaList(
                                                                            SyntaxFactory.DocumentationCommentExterior(""    ///"")),
                                                                        text: "" Extension methods for "",
                                                                        value: "" Extension methods for "",
                                                                        trailing: default))),
                                                            SyntaxFactory.XmlEmptyElement(
                                                                lessThanToken: SyntaxFactory.Token(SyntaxKind.LessThanToken),
                                                                name: SyntaxFactory.XmlName(
                                                                    prefix: default,
                                                                    localName: SyntaxFactory.Identifier(""see"")),
                                                                attributes: SyntaxFactory.SingletonList<XmlAttributeSyntax>(
                                                                    SyntaxFactory.XmlCrefAttribute(
                                                                        name: SyntaxFactory.XmlName(
                                                                            prefix: default,
                                                                            localName: SyntaxFactory.Identifier(""cref"")),
                                                                        equalsToken: SyntaxFactory.Token(SyntaxKind.EqualsToken),
                                                                        startQuoteToken: SyntaxFactory.Token(SyntaxKind.DoubleQuoteToken),
                                                                        cref: SyntaxFactory.QualifiedCref(
                                                                            container: SyntaxFactory.QualifiedName(
                                                                                left: SyntaxFactory.IdentifierName(
                                                                                    identifier: SyntaxFactory.Identifier(""Gu"")),
                                                                                dotToken: SyntaxFactory.Token(SyntaxKind.DotToken),
                                                                                right: SyntaxFactory.IdentifierName(
                                                                                    identifier: SyntaxFactory.Identifier(""Inject""))),
                                                                            dotToken: SyntaxFactory.Token(SyntaxKind.DotToken),
                                                                            member: SyntaxFactory.NameMemberCref(
                                                                                name: SyntaxFactory.GenericName(
                                                                                    identifier: SyntaxFactory.Identifier(""Container""),
                                                                                    typeArgumentList: SyntaxFactory.TypeArgumentList(
                                                                                        lessThanToken: SyntaxFactory.Token(
                                                                                            leading: default,
                                                                                            kind: SyntaxKind.LessThanToken,
                                                                                            text: ""{"",
                                                                                            valueText: ""<"",
                                                                                            trailing: default),
                                                                                        arguments: SyntaxFactory.SingletonSeparatedList<TypeSyntax>(
                                                                                            SyntaxFactory.IdentifierName(
                                                                                                identifier: SyntaxFactory.Identifier(""T""))),
                                                                                        greaterThanToken: SyntaxFactory.Token(
                                                                                            leading: default,
                                                                                            kind: SyntaxKind.GreaterThanToken,
                                                                                            text: ""}"",
                                                                                            valueText: "">"",
                                                                                            trailing: default))),
                                                                                parameters: default)),
                                                                        endQuoteToken: SyntaxFactory.Token(SyntaxKind.DoubleQuoteToken))),
                                                                slashGreaterThanToken: SyntaxFactory.Token(SyntaxKind.SlashGreaterThanToken)),
                                                            SyntaxFactory.XmlText(
                                                                textTokens: SyntaxFactory.TokenList(
                                                                    SyntaxFactory.XmlTextLiteral("".""),
                                                                    SyntaxFactory.XmlTextNewLine(
                                                                        leading: default,
                                                                        text: ""\r\n"",
                                                                        value: ""\r\n"",
                                                                        trailing: default),
                                                                    SyntaxFactory.XmlTextLiteral(
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
                                                                    SyntaxFactory.XmlTextLiteral(
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
                                                            localName: SyntaxFactory.Identifier(""summary"")),
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
                    keyword: SyntaxFactory.Token(SyntaxKind.ClassKeyword),
                    identifier: SyntaxFactory.Identifier(""C""),
                    typeParameterList: default,
                    baseList: default,
                    constraintClauses: default,
                    openBraceToken: SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
                    members: default,
                    closeBraceToken: SyntaxFactory.Token(SyntaxKind.CloseBraceToken),
                    semicolonToken: default)),
            closeBraceToken: SyntaxFactory.Token(SyntaxKind.CloseBraceToken),
            semicolonToken: default)),
    endOfFileToken: SyntaxFactory.Token(SyntaxKind.EndOfFileToken))";
            CodeAssert.AreEqual(expected, call);
            await AssertRoundtrip(code).ConfigureAwait(false);
        }

        [Test]
        public static async Task ClassWithXmlInDocs()
        {
            var code = @"namespace A
{
    /// <summary>
    /// Gets the action registered for <see cref=""Context""/> by BenchmarkAnalysisContext.RegisterSyntaxNodeAction(action, syntaxKinds)""/>.
    /// </summary>
    public class C
    {
    }
}";
            var call = SyntaxFactoryWriter.Serialize(code, Settings);
            var expected = @"SyntaxFactory.CompilationUnit(
    externs: default,
    usings: default,
    attributeLists: default,
    members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
        SyntaxFactory.NamespaceDeclaration(
            attributeLists: default,
            modifiers: default,
            namespaceKeyword: SyntaxFactory.Token(SyntaxKind.NamespaceKeyword),
            name: SyntaxFactory.IdentifierName(
                identifier: SyntaxFactory.Identifier(""A"")),
            openBraceToken: SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
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
                                                        SyntaxFactory.XmlTextLiteral(
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
                                                            localName: SyntaxFactory.Identifier(""summary"")),
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
                                                                    SyntaxFactory.XmlTextLiteral(
                                                                        leading: SyntaxFactory.TriviaList(
                                                                            SyntaxFactory.DocumentationCommentExterior(""    ///"")),
                                                                        text: "" Gets the action registered for "",
                                                                        value: "" Gets the action registered for "",
                                                                        trailing: default))),
                                                            SyntaxFactory.XmlEmptyElement(
                                                                lessThanToken: SyntaxFactory.Token(SyntaxKind.LessThanToken),
                                                                name: SyntaxFactory.XmlName(
                                                                    prefix: default,
                                                                    localName: SyntaxFactory.Identifier(""see"")),
                                                                attributes: SyntaxFactory.SingletonList<XmlAttributeSyntax>(
                                                                    SyntaxFactory.XmlCrefAttribute(
                                                                        name: SyntaxFactory.XmlName(
                                                                            prefix: default,
                                                                            localName: SyntaxFactory.Identifier(""cref"")),
                                                                        equalsToken: SyntaxFactory.Token(SyntaxKind.EqualsToken),
                                                                        startQuoteToken: SyntaxFactory.Token(SyntaxKind.DoubleQuoteToken),
                                                                        cref: SyntaxFactory.NameMemberCref(
                                                                            name: SyntaxFactory.IdentifierName(
                                                                                identifier: SyntaxFactory.Identifier(""Context"")),
                                                                            parameters: default),
                                                                        endQuoteToken: SyntaxFactory.Token(SyntaxKind.DoubleQuoteToken))),
                                                                slashGreaterThanToken: SyntaxFactory.Token(SyntaxKind.SlashGreaterThanToken)),
                                                            SyntaxFactory.XmlText(
                                                                textTokens: SyntaxFactory.TokenList(
                                                                    SyntaxFactory.XmlTextLiteral(
                                                                        text: "" by BenchmarkAnalysisContext.RegisterSyntaxNodeAction(action, syntaxKinds)\""/>."",
                                                                        value: "" by BenchmarkAnalysisContext.RegisterSyntaxNodeAction(action, syntaxKinds)\""/>.""),
                                                                    SyntaxFactory.XmlTextNewLine(
                                                                        leading: default,
                                                                        text: ""\r\n"",
                                                                        value: ""\r\n"",
                                                                        trailing: default),
                                                                    SyntaxFactory.XmlTextLiteral(
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
                                                            localName: SyntaxFactory.Identifier(""summary"")),
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
                    keyword: SyntaxFactory.Token(SyntaxKind.ClassKeyword),
                    identifier: SyntaxFactory.Identifier(""C""),
                    typeParameterList: default,
                    baseList: default,
                    constraintClauses: default,
                    openBraceToken: SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
                    members: default,
                    closeBraceToken: SyntaxFactory.Token(SyntaxKind.CloseBraceToken),
                    semicolonToken: default)),
            closeBraceToken: SyntaxFactory.Token(SyntaxKind.CloseBraceToken),
            semicolonToken: default)),
    endOfFileToken: SyntaxFactory.Token(SyntaxKind.EndOfFileToken))";
            CodeAssert.AreEqual(expected, call);
            await AssertRoundtrip(code).ConfigureAwait(false);
        }

        [Test]
        public static async Task ClassWithLineDirective()
        {
            var code = @"namespace A
{
    class C 
    {
        void InitializeComponent()
        {
            #line 1 ""..\..\MainWindow.xaml""
            
            #line default
            #line hidden
        }
    }
}";
            var call = SyntaxFactoryWriter.Serialize(code, Settings);
            var expected = @"SyntaxFactory.CompilationUnit(
    externs: default,
    usings: default,
    attributeLists: default,
    members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
        SyntaxFactory.NamespaceDeclaration(
            attributeLists: default,
            modifiers: default,
            namespaceKeyword: SyntaxFactory.Token(SyntaxKind.NamespaceKeyword),
            name: SyntaxFactory.IdentifierName(
                identifier: SyntaxFactory.Identifier(""A"")),
            openBraceToken: SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
            externs: default,
            usings: default,
            members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                SyntaxFactory.ClassDeclaration(
                    attributeLists: default,
                    modifiers: default,
                    keyword: SyntaxFactory.Token(SyntaxKind.ClassKeyword),
                    identifier: SyntaxFactory.Identifier(""C""),
                    typeParameterList: default,
                    baseList: default,
                    constraintClauses: default,
                    openBraceToken: SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
                    members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                        SyntaxFactory.MethodDeclaration(
                            attributeLists: default,
                            modifiers: default,
                            returnType: SyntaxFactory.PredefinedType(
                                keyword: SyntaxFactory.Token(SyntaxKind.VoidKeyword)),
                            explicitInterfaceSpecifier: default,
                            identifier: SyntaxFactory.Identifier(""InitializeComponent""),
                            typeParameterList: default,
                            parameterList: SyntaxFactory.ParameterList(
                                openParenToken: SyntaxFactory.Token(SyntaxKind.OpenParenToken),
                                parameters: default,
                                closeParenToken: SyntaxFactory.Token(SyntaxKind.CloseParenToken)),
                            constraintClauses: default,
                            body: SyntaxFactory.Block(
                                attributeLists: default,
                                openBraceToken: SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
                                statements: default,
                                closeBraceToken: SyntaxFactory.Token(
                                    leading: SyntaxFactory.TriviaList(
                                        SyntaxFactory.Whitespace(""            ""),
                                        SyntaxFactory.Trivia(
                                            SyntaxFactory.LineDirectiveTrivia(
                                                hashToken: SyntaxFactory.Token(SyntaxKind.HashToken),
                                                lineKeyword: SyntaxFactory.Token(SyntaxKind.LineKeyword),
                                                line: SyntaxFactory.Literal(
                                                    text: ""1"",
                                                    value: 1),
                                                file: SyntaxFactory.Literal(
                                                    text: ""\""..\\..\\MainWindow.xaml\"""",
                                                    value: ""..\\..\\MainWindow.xaml""),
                                                endOfDirectiveToken: SyntaxFactory.Token(SyntaxKind.EndOfDirectiveToken),
                                                isActive: true)),
                                        SyntaxFactory.Whitespace(""            ""),
                                        SyntaxFactory.LineFeed,
                                        SyntaxFactory.Whitespace(""            ""),
                                        SyntaxFactory.Trivia(
                                            SyntaxFactory.LineDirectiveTrivia(
                                                hashToken: SyntaxFactory.Token(SyntaxKind.HashToken),
                                                lineKeyword: SyntaxFactory.Token(SyntaxKind.LineKeyword),
                                                line: SyntaxFactory.Token(SyntaxKind.DefaultKeyword),
                                                file: default,
                                                endOfDirectiveToken: SyntaxFactory.Token(SyntaxKind.EndOfDirectiveToken),
                                                isActive: true)),
                                        SyntaxFactory.Whitespace(""            ""),
                                        SyntaxFactory.Trivia(
                                            SyntaxFactory.LineDirectiveTrivia(
                                                hashToken: SyntaxFactory.Token(SyntaxKind.HashToken),
                                                lineKeyword: SyntaxFactory.Token(SyntaxKind.LineKeyword),
                                                line: SyntaxFactory.Token(SyntaxKind.HiddenKeyword),
                                                file: default,
                                                endOfDirectiveToken: SyntaxFactory.Token(SyntaxKind.EndOfDirectiveToken),
                                                isActive: true)),
                                        SyntaxFactory.Whitespace(""        "")),
                                    kind: SyntaxKind.CloseBraceToken,
                                    trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed))),
                            expressionBody: default,
                            semicolonToken: default)),
                    closeBraceToken: SyntaxFactory.Token(SyntaxKind.CloseBraceToken),
                    semicolonToken: default)),
            closeBraceToken: SyntaxFactory.Token(SyntaxKind.CloseBraceToken),
            semicolonToken: default)),
    endOfFileToken: SyntaxFactory.Token(SyntaxKind.EndOfFileToken))";
            CodeAssert.AreEqual(expected, call);
            await AssertRoundtrip(code).ConfigureAwait(false);
        }

        [Test]
        public static async Task ClassWithIfDirective()
        {
            var code = @"namespace A
{
    public class C
    {
        public static int M()
        {
#if true
            return 1;
#else
            return 2;
#endif
        }
    }
}";
            var call = SyntaxFactoryWriter.Serialize(code, Settings);
            var expected = @"SyntaxFactory.CompilationUnit(
    externs: default,
    usings: default,
    attributeLists: default,
    members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
        SyntaxFactory.NamespaceDeclaration(
            attributeLists: default,
            modifiers: default,
            namespaceKeyword: SyntaxFactory.Token(SyntaxKind.NamespaceKeyword),
            name: SyntaxFactory.IdentifierName(
                identifier: SyntaxFactory.Identifier(""A"")),
            openBraceToken: SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
            externs: default,
            usings: default,
            members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                SyntaxFactory.ClassDeclaration(
                    attributeLists: default,
                    modifiers: SyntaxFactory.TokenList(
                        SyntaxFactory.Token(SyntaxKind.PublicKeyword)),
                    keyword: SyntaxFactory.Token(SyntaxKind.ClassKeyword),
                    identifier: SyntaxFactory.Identifier(""C""),
                    typeParameterList: default,
                    baseList: default,
                    constraintClauses: default,
                    openBraceToken: SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
                    members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                        SyntaxFactory.MethodDeclaration(
                            attributeLists: default,
                            modifiers: SyntaxFactory.TokenList(
                                SyntaxFactory.Token(SyntaxKind.PublicKeyword),
                                SyntaxFactory.Token(SyntaxKind.StaticKeyword)),
                            returnType: SyntaxFactory.PredefinedType(
                                keyword: SyntaxFactory.Token(SyntaxKind.IntKeyword)),
                            explicitInterfaceSpecifier: default,
                            identifier: SyntaxFactory.Identifier(""M""),
                            typeParameterList: default,
                            parameterList: SyntaxFactory.ParameterList(
                                openParenToken: SyntaxFactory.Token(SyntaxKind.OpenParenToken),
                                parameters: default,
                                closeParenToken: SyntaxFactory.Token(SyntaxKind.CloseParenToken)),
                            constraintClauses: default,
                            body: SyntaxFactory.Block(
                                attributeLists: default,
                                openBraceToken: SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
                                statements: SyntaxFactory.SingletonList<StatementSyntax>(
                                    SyntaxFactory.ReturnStatement(
                                        attributeLists: default,
                                        returnKeyword: SyntaxFactory.Token(
                                            leading: SyntaxFactory.TriviaList(
                                                SyntaxFactory.Trivia(
                                                    SyntaxFactory.IfDirectiveTrivia(
                                                        hashToken: SyntaxFactory.Token(SyntaxKind.HashToken),
                                                        ifKeyword: SyntaxFactory.Token(SyntaxKind.IfKeyword),
                                                        condition: SyntaxFactory.LiteralExpression(
                                                            kind: SyntaxKind.TrueLiteralExpression,
                                                            token: SyntaxFactory.Token(SyntaxKind.TrueKeyword)),
                                                        endOfDirectiveToken: SyntaxFactory.Token(SyntaxKind.EndOfDirectiveToken),
                                                        isActive: true,
                                                        branchTaken: true,
                                                        conditionValue: true)),
                                                SyntaxFactory.Whitespace(""            "")),
                                            kind: SyntaxKind.ReturnKeyword,
                                            trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space)),
                                        expression: SyntaxFactory.LiteralExpression(
                                            kind: SyntaxKind.NumericLiteralExpression,
                                            token: SyntaxFactory.Literal(
                                                text: ""1"",
                                                value: 1)),
                                        semicolonToken: SyntaxFactory.Token(SyntaxKind.SemicolonToken))),
                                closeBraceToken: SyntaxFactory.Token(
                                    leading: SyntaxFactory.TriviaList(
                                        SyntaxFactory.Trivia(
                                            SyntaxFactory.ElseDirectiveTrivia(
                                                hashToken: SyntaxFactory.Token(SyntaxKind.HashToken),
                                                elseKeyword: SyntaxFactory.Token(SyntaxKind.ElseKeyword),
                                                endOfDirectiveToken: SyntaxFactory.Token(SyntaxKind.EndOfDirectiveToken),
                                                isActive: true,
                                                branchTaken: false)),
                                        SyntaxFactory.DisabledText(""            return 2;\r\n""),
                                        SyntaxFactory.Trivia(
                                            SyntaxFactory.EndIfDirectiveTrivia(
                                                hashToken: SyntaxFactory.Token(SyntaxKind.HashToken),
                                                endIfKeyword: SyntaxFactory.Token(SyntaxKind.EndIfKeyword),
                                                endOfDirectiveToken: SyntaxFactory.Token(SyntaxKind.EndOfDirectiveToken),
                                                isActive: true)),
                                        SyntaxFactory.Whitespace(""        "")),
                                    kind: SyntaxKind.CloseBraceToken,
                                    trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed))),
                            expressionBody: default,
                            semicolonToken: default)),
                    closeBraceToken: SyntaxFactory.Token(SyntaxKind.CloseBraceToken),
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
            var call = SyntaxFactoryWriter.Serialize(code, Settings);
            var expected = @"SyntaxFactory.CompilationUnit(
    externs: default,
    usings: default,
    attributeLists: default,
    members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
        SyntaxFactory.NamespaceDeclaration(
            attributeLists: default,
            modifiers: default,
            namespaceKeyword: SyntaxFactory.Token(SyntaxKind.NamespaceKeyword),
            name: SyntaxFactory.IdentifierName(
                identifier: SyntaxFactory.Identifier(""A"")),
            openBraceToken: SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
            externs: default,
            usings: default,
            members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                SyntaxFactory.EnumDeclaration(
                    attributeLists: default,
                    modifiers: SyntaxFactory.TokenList(
                        SyntaxFactory.Token(SyntaxKind.PublicKeyword)),
                    enumKeyword: SyntaxFactory.Token(SyntaxKind.EnumKeyword),
                    identifier: SyntaxFactory.Identifier(""E""),
                    baseList: default,
                    openBraceToken: SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
                    members: SyntaxFactory.SingletonSeparatedList<EnumMemberDeclarationSyntax>(
                        SyntaxFactory.EnumMemberDeclaration(
                            attributeLists: default,
                            modifiers: default,
                            identifier: SyntaxFactory.Identifier(""M""),
                            equalsValue: default)),
                    closeBraceToken: SyntaxFactory.Token(SyntaxKind.CloseBraceToken),
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
            var call = SyntaxFactoryWriter.Serialize(code, Settings);
            var expected = @"SyntaxFactory.CompilationUnit(
    externs: default,
    usings: default,
    attributeLists: default,
    members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
        SyntaxFactory.NamespaceDeclaration(
            attributeLists: default,
            modifiers: default,
            namespaceKeyword: SyntaxFactory.Token(SyntaxKind.NamespaceKeyword),
            name: SyntaxFactory.IdentifierName(
                identifier: SyntaxFactory.Identifier(""A"")),
            openBraceToken: SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
            externs: default,
            usings: default,
            members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                SyntaxFactory.EnumDeclaration(
                    attributeLists: default,
                    modifiers: SyntaxFactory.TokenList(
                        SyntaxFactory.Token(SyntaxKind.PublicKeyword)),
                    enumKeyword: SyntaxFactory.Token(SyntaxKind.EnumKeyword),
                    identifier: SyntaxFactory.Identifier(""E""),
                    baseList: default,
                    openBraceToken: SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
                    members: SyntaxFactory.SeparatedList(
                        new EnumMemberDeclarationSyntax[]
                        {
                            SyntaxFactory.EnumMemberDeclaration(
                                attributeLists: default,
                                modifiers: default,
                                identifier: SyntaxFactory.Identifier(""M1""),
                                equalsValue: default),
                            SyntaxFactory.EnumMemberDeclaration(
                                attributeLists: default,
                                modifiers: default,
                                identifier: SyntaxFactory.Identifier(""M2""),
                                equalsValue: default),
                        },
                        new SyntaxToken[]
                        {
                            SyntaxFactory.Token(SyntaxKind.CommaToken),
                            SyntaxFactory.Token(SyntaxKind.CommaToken),
                        }),
                    closeBraceToken: SyntaxFactory.Token(SyntaxKind.CloseBraceToken),
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
            var call = SyntaxFactoryWriter.Serialize(code, Settings);
            var expected = @"SyntaxFactory.CompilationUnit(
    externs: default,
    usings: default,
    attributeLists: default,
    members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
        SyntaxFactory.NamespaceDeclaration(
            attributeLists: default,
            modifiers: default,
            namespaceKeyword: SyntaxFactory.Token(SyntaxKind.NamespaceKeyword),
            name: SyntaxFactory.IdentifierName(
                identifier: SyntaxFactory.Identifier(""A"")),
            openBraceToken: SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
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
                                                        SyntaxFactory.XmlTextLiteral(
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
                                                            localName: SyntaxFactory.Identifier(""summary"")),
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
                                                                SyntaxFactory.XmlTextLiteral(
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
                                                                SyntaxFactory.XmlTextLiteral(
                                                                    leading: SyntaxFactory.TriviaList(
                                                                        SyntaxFactory.DocumentationCommentExterior(""    ///"")),
                                                                    text: "" "",
                                                                    value: "" "",
                                                                    trailing: default)))),
                                                    endTag: SyntaxFactory.XmlElementEndTag(
                                                        lessThanSlashToken: SyntaxFactory.Token(SyntaxKind.LessThanSlashToken),
                                                        name: SyntaxFactory.XmlName(
                                                            prefix: default,
                                                            localName: SyntaxFactory.Identifier(""summary"")),
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
                    enumKeyword: SyntaxFactory.Token(SyntaxKind.EnumKeyword),
                    identifier: SyntaxFactory.Identifier(""E""),
                    baseList: default,
                    openBraceToken: SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
                    members: SyntaxFactory.SeparatedList(
                        new EnumMemberDeclarationSyntax[]
                        {
                            SyntaxFactory.EnumMemberDeclaration(
                                attributeLists: default,
                                modifiers: default,
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
                                                                SyntaxFactory.XmlTextLiteral(
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
                                                                    localName: SyntaxFactory.Identifier(""summary"")),
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
                                                                        SyntaxFactory.XmlTextLiteral(
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
                                                                        SyntaxFactory.XmlTextLiteral(
                                                                            leading: SyntaxFactory.TriviaList(
                                                                                SyntaxFactory.DocumentationCommentExterior(""        ///"")),
                                                                            text: "" "",
                                                                            value: "" "",
                                                                            trailing: default)))),
                                                            endTag: SyntaxFactory.XmlElementEndTag(
                                                                lessThanSlashToken: SyntaxFactory.Token(SyntaxKind.LessThanSlashToken),
                                                                name: SyntaxFactory.XmlName(
                                                                    prefix: default,
                                                                    localName: SyntaxFactory.Identifier(""summary"")),
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
                                modifiers: default,
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
                                                                SyntaxFactory.XmlTextLiteral(
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
                                                                    localName: SyntaxFactory.Identifier(""summary"")),
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
                                                                        SyntaxFactory.XmlTextLiteral(
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
                                                                        SyntaxFactory.XmlTextLiteral(
                                                                            leading: SyntaxFactory.TriviaList(
                                                                                SyntaxFactory.DocumentationCommentExterior(""        ///"")),
                                                                            text: "" "",
                                                                            value: "" "",
                                                                            trailing: default)))),
                                                            endTag: SyntaxFactory.XmlElementEndTag(
                                                                lessThanSlashToken: SyntaxFactory.Token(SyntaxKind.LessThanSlashToken),
                                                                name: SyntaxFactory.XmlName(
                                                                    prefix: default,
                                                                    localName: SyntaxFactory.Identifier(""summary"")),
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
                            SyntaxFactory.Token(SyntaxKind.CommaToken),
                            SyntaxFactory.Token(SyntaxKind.CommaToken),
                        }),
                    closeBraceToken: SyntaxFactory.Token(SyntaxKind.CloseBraceToken),
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
            Assert.DoesNotThrow(() => SyntaxFactoryWriter.Serialize(code, Settings));
            await AssertRoundtrip(code).ConfigureAwait(false);
        }

        [Test]
        public static async Task IfDirectiveNoNamespace()
        {
            var code = @"// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

﻿#if NET40PLUS && !NET45PLUS

using System.Runtime.CompilerServices;

[assembly: TypeForwardedTo(typeof(AwaitExtensions))]

#else
class C
{
}";
            var call = SyntaxFactoryWriter.Serialize(code, Settings);
            var expected = @"SyntaxFactory.CompilationUnit(
    externs: default,
    usings: default,
    attributeLists: default,
    members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
        SyntaxFactory.ClassDeclaration(
            attributeLists: default,
            modifiers: default,
            keyword: SyntaxFactory.Token(
                leading: SyntaxFactory.TriviaList(
                    SyntaxFactory.Comment(""// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.""),
                    SyntaxFactory.LineFeed,
                    SyntaxFactory.Comment(""// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.""),
                    SyntaxFactory.LineFeed,
                    SyntaxFactory.LineFeed,
                    SyntaxFactory.Whitespace(""﻿""),
                    SyntaxFactory.Trivia(
                        SyntaxFactory.IfDirectiveTrivia(
                            hashToken: SyntaxFactory.Token(SyntaxKind.HashToken),
                            ifKeyword: SyntaxFactory.Token(SyntaxKind.IfKeyword),
                            condition: SyntaxFactory.BinaryExpression(
                                kind: SyntaxKind.LogicalAndExpression,
                                left: SyntaxFactory.IdentifierName(
                                    identifier: SyntaxFactory.Identifier(""NET40PLUS"")),
                                operatorToken: SyntaxFactory.Token(SyntaxKind.AmpersandAmpersandToken),
                                right: SyntaxFactory.PrefixUnaryExpression(
                                    kind: SyntaxKind.LogicalNotExpression,
                                    operatorToken: SyntaxFactory.Token(SyntaxKind.ExclamationToken),
                                    operand: SyntaxFactory.IdentifierName(
                                        identifier: SyntaxFactory.Identifier(""NET45PLUS"")))),
                            endOfDirectiveToken: SyntaxFactory.Token(SyntaxKind.EndOfDirectiveToken),
                            isActive: true,
                            branchTaken: false,
                            conditionValue: false)),
                    SyntaxFactory.DisabledText(""\r\nusing System.Runtime.CompilerServices;\r\n\r\n[assembly: TypeForwardedTo(typeof(AwaitExtensions))]\r\n\r\n""),
                    SyntaxFactory.Trivia(
                        SyntaxFactory.ElseDirectiveTrivia(
                            hashToken: SyntaxFactory.Token(SyntaxKind.HashToken),
                            elseKeyword: SyntaxFactory.Token(SyntaxKind.ElseKeyword),
                            endOfDirectiveToken: SyntaxFactory.Token(SyntaxKind.EndOfDirectiveToken),
                            isActive: true,
                            branchTaken: true))),
                kind: SyntaxKind.ClassKeyword,
                trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space)),
            identifier: SyntaxFactory.Identifier(""C""),
            typeParameterList: default,
            baseList: default,
            constraintClauses: default,
            openBraceToken: SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
            members: default,
            closeBraceToken: SyntaxFactory.Token(SyntaxKind.CloseBraceToken),
            semicolonToken: default)),
    endOfFileToken: SyntaxFactory.Token(SyntaxKind.EndOfFileToken))";
            CodeAssert.AreEqual(expected, call);
            await AssertRoundtrip(code).ConfigureAwait(false);
        }

        [Test]
        public static async Task ClassInRegion()
        {
            var code = @"namespace A
{
#region meh
    public class C
    {
    }
#endregion
}";
            var call = SyntaxFactoryWriter.Serialize(code, Settings);
            var expected = @"SyntaxFactory.CompilationUnit(
    externs: default,
    usings: default,
    attributeLists: default,
    members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
        SyntaxFactory.NamespaceDeclaration(
            attributeLists: default,
            modifiers: default,
            namespaceKeyword: SyntaxFactory.Token(SyntaxKind.NamespaceKeyword),
            name: SyntaxFactory.IdentifierName(
                identifier: SyntaxFactory.Identifier(""A"")),
            openBraceToken: SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
            externs: default,
            usings: default,
            members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                SyntaxFactory.ClassDeclaration(
                    attributeLists: default,
                    modifiers: SyntaxFactory.TokenList(
                        SyntaxFactory.Token(
                            leading: SyntaxFactory.TriviaList(
                                SyntaxFactory.Trivia(
                                    SyntaxFactory.RegionDirectiveTrivia(
                                        hashToken: SyntaxFactory.Token(SyntaxKind.HashToken),
                                        regionKeyword: SyntaxFactory.Token(SyntaxKind.RegionKeyword),
                                        endOfDirectiveToken: SyntaxFactory.Token(
                                            leading: SyntaxFactory.TriviaList(
                                                SyntaxFactory.PreprocessingMessage(""meh"")),
                                            kind: SyntaxKind.EndOfDirectiveToken,
                                            trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),
                                        isActive: true)),
                                SyntaxFactory.Whitespace(""    "")),
                            kind: SyntaxKind.PublicKeyword,
                            trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space))),
                    keyword: SyntaxFactory.Token(SyntaxKind.ClassKeyword),
                    identifier: SyntaxFactory.Identifier(""C""),
                    typeParameterList: default,
                    baseList: default,
                    constraintClauses: default,
                    openBraceToken: SyntaxFactory.Token(SyntaxKind.OpenBraceToken),
                    members: default,
                    closeBraceToken: SyntaxFactory.Token(SyntaxKind.CloseBraceToken),
                    semicolonToken: default)),
            closeBraceToken: SyntaxFactory.Token(
                leading: SyntaxFactory.TriviaList(
                    SyntaxFactory.Trivia(
                        SyntaxFactory.EndRegionDirectiveTrivia(
                            hashToken: SyntaxFactory.Token(SyntaxKind.HashToken),
                            endRegionKeyword: SyntaxFactory.Token(SyntaxKind.EndRegionKeyword),
                            endOfDirectiveToken: SyntaxFactory.Token(SyntaxKind.EndOfDirectiveToken),
                            isActive: true))),
                kind: SyntaxKind.CloseBraceToken,
                trailing: default),
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
        [TestCase("string x = \"a\\n\"")]
        [TestCase("string x = \"a\\n\" ")]
        [TestCase("string x = \"a\\r\\n\"")]
        [TestCase("string x = \"a\\r\\n\" ")]
        [TestCase("string x = $\"a{1}\"")]
        [TestCase("string x = $\"a{1}\" ")]
        [TestCase("string x = \"1\\u00A0mm\"")]
        [TestCase("string x = \"1\\u00A0mm\" ")]
        [TestCase("string x = \"\\\"\"")]
        [TestCase("string x = \"\\\"\" ")]
        [TestCase("string x = @\"a\"")]
        [TestCase("string x = @\"a\" ")]
        [TestCase("char x = 'a'")]
        [TestCase("char x = 'a' ")]
        [TestCase("char x = '\\\\'")]
        [TestCase("char x = '\\\\' ")]
        [TestCase("char x = '\\u00A0'")]
        [TestCase("char x = '\\u00A0' ")]
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
            catch (CompilationErrorException)
            {
                Console.Write(call);
                throw;
            }
        }

#pragma warning disable IDE0051 // Remove unused private members Run by Roundtrip()
        private static void Samples()
#pragma warning restore IDE0051 // Remove unused private members
        {
#pragma warning disable IDE0059 // Value assigned to symbol is never used
#pragma warning disable CS0219 // Variable is assigned but its value is never used
            // ReSharper disable UnusedVariable
            const string s1 = "1\u0040mm";
            const string s2 = "1\r\n";
            const string s3 = @"1""";
            //// ReSharper restore UnusedVariable
#pragma warning restore CS0219 // Variable is assigned but its value is never used
#pragma warning restore IDE0059 // Value assigned to symbol is never used
        }
    }
}
