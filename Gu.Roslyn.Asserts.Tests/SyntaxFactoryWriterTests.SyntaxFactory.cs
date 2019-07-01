namespace Gu.Roslyn.Asserts.Tests
{
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

        [Test]
        public static async Task SimpleClassInNamespace()
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
                           "            namespaceKeyword: SyntaxFactory.Token(\r\n" +
                           "                leading: default,\r\n" +
                           "                kind: SyntaxKind.NamespaceKeyword,\r\n" +
                           "                trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space)),\r\n" +
                           "            name: SyntaxFactory.QualifiedName(\r\n" +
                           "                left: SyntaxFactory.IdentifierName(\"A\"),\r\n" +
                           "                dotToken: SyntaxFactory.Token(SyntaxKind.DotToken),\r\n" +
                           "                right: SyntaxFactory.IdentifierName(\r\n" +
                           "                    identifier: SyntaxFactory.Identifier(\r\n" +
                           "                        leading: default,\r\n" +
                           "                        text: \"B\",\r\n" +
                           "                        trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)))),\r\n" +
                           "            openBraceToken: SyntaxFactory.Token(\r\n" +
                           "                leading: default,\r\n" +
                           "                kind: SyntaxKind.OpenBraceToken,\r\n" +
                           "                trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),\r\n" +
                           "            externs: default,\r\n" +
                           "            usings: default,\r\n" +
                           "            members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(\r\n" +
                           "                SyntaxFactory.ClassDeclaration(\r\n" +
                           "                    attributeLists: default,\r\n" +
                           "                    modifiers: SyntaxFactory.TokenList(\r\n" +
                           "                        SyntaxFactory.Token(\r\n" +
                           "                            leading: SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(\"    \")),\r\n" +
                           "                            kind: SyntaxKind.PublicKeyword,\r\n" +
                           "                            trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space))),\r\n" +
                           "                    keyword: SyntaxFactory.Token(\r\n" +
                           "                        leading: default,\r\n" +
                           "                        kind: SyntaxKind.ClassKeyword,\r\n" +
                           "                        trailing: SyntaxFactory.TriviaList(SyntaxFactory.Space)),\r\n" +
                           "                    identifier: SyntaxFactory.Identifier(\r\n" +
                           "                        leading: default,\r\n" +
                           "                        text: \"C\",\r\n" +
                           "                        trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),\r\n" +
                           "                    typeParameterList: default,\r\n" +
                           "                    baseList: default,\r\n" +
                           "                    constraintClauses: default,\r\n" +
                           "                    openBraceToken: SyntaxFactory.Token(\r\n" +
                           "                        leading: SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(\"    \")),\r\n" +
                           "                        kind: SyntaxKind.OpenBraceToken,\r\n" +
                           "                        trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),\r\n" +
                           "                    members: default,\r\n" +
                           "                    closeBraceToken: SyntaxFactory.Token(\r\n" +
                           "                        leading: SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(\"    \")),\r\n" +
                           "                        kind: SyntaxKind.CloseBraceToken,\r\n" +
                           "                        trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),\r\n" +
                           "                    semicolonToken: default)),\r\n" +
                           "            closeBraceToken: SyntaxFactory.Token(\r\n" +
                           "                leading: default,\r\n" +
                           "                kind: SyntaxKind.CloseBraceToken,\r\n" +
                           "                trailing: SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),\r\n" +
                           "            semicolonToken: default)),\r\n" +
                           "    attributeLists: default)";
            CodeAssert.AreEqual(expected, call);
            await AssertRoundtrip(code).ConfigureAwait(false);
        }

        [TestCase("int x = 1")]
        [TestCase("long x = 1")]
        [TestCase("double x = 1")]
        [TestCase("double x = 1.2")]
        [TestCase("object x = null")]
        [TestCase("var x = true")]
        [TestCase("var x = false")]
        [TestCase("var x = 1")]
        [TestCase("var x = 1.2")]
        [TestCase("string x = \"a\"")]
        [TestCase("string x = \"\\\"\"")]
        [TestCase("string x = @\"a\"")]
        [TestCase("char x = 'a'")]
        [TestCase("char x = '\\\\'")]
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
            var call = SyntaxFactoryWriter.Write(code);
            var result = await CSharpScript.EvaluateAsync<SyntaxNode>(call, ScriptOptions);
            CodeAssert.AreEqual(code, result.ToString());
        }
    }
}
