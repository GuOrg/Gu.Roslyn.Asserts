namespace Gu.Roslyn.Asserts.Tests
{
    using System.Threading.Tasks;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Scripting;
    using Microsoft.CodeAnalysis.Scripting;
    using NUnit.Framework;

    public static partial class AstWriterTests
    {
        public static class SyntaxFactoryCode
        {
            private static readonly ScriptOptions ScriptOptions = ScriptOptions.Default
                                                                               .WithReferences(Gu.Roslyn.Asserts.MetadataReferences.Transitive(typeof(SyntaxFactory)))
                                                                               .WithImports("Microsoft.CodeAnalysis.CSharp", "Microsoft.CodeAnalysis.CSharp.Syntax")
                                                                               .WithEmitDebugInformation(emitDebugInformation: true);

            [Test]
            public static async Task SomeTest()
            {
                var expression = @"SyntaxFactory.CompilationUnit(
                    externs: default,
                    usings: default,
                    members: SyntaxFactory.SingletonList<MemberDeclarationSyntax>(
                        SyntaxFactory.ClassDeclaration(
                            attributeLists: default,
                            modifiers: default,
                            keyword: SyntaxFactory.Token(SyntaxFactory.TriviaList(), SyntaxKind.ClassKeyword, SyntaxFactory.TriviaList(SyntaxFactory.Space)),
                            identifier: SyntaxFactory.Identifier(SyntaxFactory.TriviaList(), ""C"", SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),
                            typeParameterList: default,
                            baseList: default,
                            constraintClauses: default,
                            openBraceToken: SyntaxFactory.Token(SyntaxFactory.TriviaList(), SyntaxKind.OpenBraceToken, SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)),
                            members: default,
                            closeBraceToken: SyntaxFactory.Token(SyntaxKind.CloseBraceToken),
                            semicolonToken: SyntaxFactory.Token(SyntaxKind.None))),
                    attributeLists: default)";
                var result = await CSharpScript.EvaluateAsync<SyntaxNode>(expression, ScriptOptions);
                CodeAssert.AreEqual("class C\n{\n}", result.ToString());
            }

            private static async Task AssertRoundtrip(SyntaxNode node)
            {
                var code = AstWriter.Serialize(node, AstWriterSettings.Default);
                var result = await CSharpScript.EvaluateAsync<SyntaxNode>(code, ScriptOptions);
                CodeAssert.AreEqual(node.ToString(), result.ToString());
            }
        }
    }
}
