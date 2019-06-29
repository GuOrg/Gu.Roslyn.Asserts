namespace Gu.Roslyn.Asserts.Tests
{
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Scripting;
    using Microsoft.CodeAnalysis.Scripting;
    using NUnit.Framework;

    public static class SyntaxFactoryWriterTests
    {
        private static readonly ScriptOptions ScriptOptions = ScriptOptions.Default
                                                                           .WithReferences(Gu.Roslyn.Asserts.MetadataReferences.Transitive(typeof(SyntaxFactory)))
                                                                           .WithImports("Microsoft.CodeAnalysis.CSharp", "Microsoft.CodeAnalysis.CSharp.Syntax")
                                                                           .WithEmitDebugInformation(emitDebugInformation: true);

        [Test]
        public static async Task GenerateSyntaxFactoryCallForSimpleClass()
        {
            var code = "namespace A.B\n" +
                       "{\n" +
                       "    public class C\n" +
                       "    {\n" +
                       "    }\n" +
                       "}\n";
            var call = SyntaxFactoryWriter.Write(code);
            var expected = "SyntaxFactory.CompilationUnit(\r\n" +
                           "    externs: default,\r\n" +
                           "    usings: default,\r\n" +
                           "    members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(\r\n" +
                           "        SyntaxFactory.NamespaceDeclaration(\r\n" +
                           "            namespaceKeyword: SyntaxFactory.Token(default, SyntaxKind.NamespaceKeyword, SyntaxFactory.TriviaList(SyntaxFactory.Space)),\r\n" +
                           "            name: SyntaxFactory.QualifiedName(\r\n" +
                           "                left: SyntaxFactory.IdentifierName(\"A\"),\r\n" +
                           "                dotToken: SyntaxFactory.Token(SyntaxKind.DotToken),\r\n" +
                           "                right: SyntaxFactory.IdentifierName(\r\n" +
                           "                    identifier: SyntaxFactory.Identifier(default, \"B\", SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)))),\r\n" +
                           "            openBraceToken: SyntaxFactory.Token(default, SyntaxKind.OpenBraceToken, SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),\r\n" +
                           "            externs: default,\r\n" +
                           "            usings: default,\r\n" +
                           "            members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(\r\n" +
                           "                SyntaxFactory.ClassDeclaration(\r\n" +
                           "                    attributeLists: default,\r\n" +
                           "                    modifiers: SyntaxFactory.TokenList(\r\n" +
                           "                        SyntaxFactory.Token(SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(\"    \")), SyntaxKind.PublicKeyword, SyntaxFactory.TriviaList(SyntaxFactory.Space))),\r\n" +
                           "                    keyword: SyntaxFactory.Token(default, SyntaxKind.ClassKeyword, SyntaxFactory.TriviaList(SyntaxFactory.Space)),\r\n" +
                           "                    identifier: SyntaxFactory.Identifier(default, \"C\", SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),\r\n" +
                           "                    typeParameterList: default,\r\n" +
                           "                    baseList: default,\r\n" +
                           "                    constraintClauses: default,\r\n" +
                           "                    openBraceToken: SyntaxFactory.Token(SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(\"    \")), SyntaxKind.OpenBraceToken, SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),\r\n" +
                           "                    members: default,\r\n" +
                           "                    closeBraceToken: SyntaxFactory.Token(SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(\"    \")), SyntaxKind.CloseBraceToken, SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),\r\n" +
                           "                    semicolonToken: default)),\r\n" +
                           "            closeBraceToken: SyntaxFactory.Token(default, SyntaxKind.CloseBraceToken, SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),\r\n" +
                           "            semicolonToken: default)),\r\n" +
                           "    attributeLists: default)";
            CodeAssert.AreEqual(expected, call);
            await AssertRoundtrip(code).ConfigureAwait(false);
        }

        private static async Task AssertRoundtrip(string code)
        {
            var call = SyntaxFactoryWriter.Write(code);
            var result = await CSharpScript.EvaluateAsync<SyntaxNode>(call, ScriptOptions);
            CodeAssert.AreEqual(code, result.ToString());
        }
    }
}
