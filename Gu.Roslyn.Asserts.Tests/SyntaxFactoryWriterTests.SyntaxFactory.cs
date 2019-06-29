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

        [Explicit("Fix")]
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
            var expected = "class C\n{\n}";
            CodeAssert.AreEqual(expected, call);
        }

        private static async Task AssertRoundtrip(SyntaxNode node)
        {
            var code = AstWriter.Serialize(node, AstWriterSettings.Default);
            var result = await CSharpScript.EvaluateAsync<SyntaxNode>(code, ScriptOptions);
            CodeAssert.AreEqual(node.ToString(), result.ToString());
        }
    }
}
