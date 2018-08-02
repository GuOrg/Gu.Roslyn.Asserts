namespace Gu.Roslyn.Asserts.Tests
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public class AstWriterTests
    {
        [Test]
        public void SimpleClass()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
    }
}");
            var node = tree.FindClassDeclaration("Foo");
            var actual = AstWriter.Serialize(node);
            var expected = "{ \"Kind\": \"ClassDeclaration\", \"LeadingTrivia\": [ { \"Kind\": \"WhitespaceTrivia\", \"Text\": \"    \" } ], \"TrailingTrivia\": [ { \"Kind\": \"EndOfLineTrivia\", \"Text\": \"\\r\\n\" } ],\r\n" +
                           "  \"ChildTokens\": [ { \"Kind\": \"PublicKeyword\", \"Text\": \"public\", \"ValueText\": \"public\", \"LeadingTrivia\": [ { \"Kind\": \"WhitespaceTrivia\", \"Text\": \"    \" } ], \"TrailingTrivia\": [ { \"Kind\": \"WhitespaceTrivia\", \"Text\": \" \" } ] },\r\n" +
                           "                   { \"Kind\": \"ClassKeyword\", \"Text\": \"class\", \"ValueText\": \"class\", \"TrailingTrivia\": [ { \"Kind\": \"WhitespaceTrivia\", \"Text\": \" \" } ] },\r\n" +
                           "                   { \"Kind\": \"IdentifierToken\", \"Text\": \"Foo\", \"ValueText\": \"Foo\", \"TrailingTrivia\": [ { \"Kind\": \"EndOfLineTrivia\", \"Text\": \"\\r\\n\" } ] },\r\n" +
                           "                   { \"Kind\": \"OpenBraceToken\", \"Text\": \"{\", \"ValueText\": \"{\", \"LeadingTrivia\": [ { \"Kind\": \"WhitespaceTrivia\", \"Text\": \"    \" } ], \"TrailingTrivia\": [ { \"Kind\": \"EndOfLineTrivia\", \"Text\": \"\\r\\n\" } ] },\r\n" +
                           "                   { \"Kind\": \"CloseBraceToken\", \"Text\": \"}\", \"ValueText\": \"}\", \"LeadingTrivia\": [ { \"Kind\": \"WhitespaceTrivia\", \"Text\": \"    \" } ], \"TrailingTrivia\": [ { \"Kind\": \"EndOfLineTrivia\", \"Text\": \"\\r\\n\" } ] } ] }";
            CodeAssert.AreEqual(expected, actual);
        }

        [Test]
        [Explicit]
        public void SummaryElement()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
public class Foo
{
}");
            var node = tree.FindClassDeclaration("Foo");
            var actual = AstWriter.Serialize(node);
            var expected =
                @"{ ""Kind"": ""ClassDeclaration"", ""LeadingTrivia"": [ { ""Kind"": ""EndOfLineTrivia"", ""Text"": ""\r\n"" } ], ""ChildTokens"": [ { ""Kind"": ""PublicKeyword"", ""Text"": ""public"", ""ValueText"": ""public"", ""LeadingTrivia"": [ { ""Kind"": ""EndOfLineTrivia"", ""Text"": ""\r\n"" } ], ""TrailingTrivia"": [ { ""Kind"": ""WhitespaceTrivia"", ""Text"": "" "" } ] }, 
{ ""Kind"": ""ClassKeyword"", ""Text"": ""class"", ""ValueText"": ""class"", ""TrailingTrivia"": [ { ""Kind"": ""WhitespaceTrivia"", ""Text"": "" "" } ] }, 
{ ""Kind"": ""IdentifierToken"", ""Text"": ""Foo"", ""ValueText"": ""Foo"", ""TrailingTrivia"": [ { ""Kind"": ""EndOfLineTrivia"", ""Text"": ""\r\n"" } ] }, 
{ ""Kind"": ""OpenBraceToken"", ""Text"": ""{"", ""ValueText"": ""{"", ""TrailingTrivia"": [ { ""Kind"": ""EndOfLineTrivia"", ""Text"": ""\r\n"" } ] }, 
{ ""Kind"": ""CloseBraceToken"", ""Text"": ""}"", ""ValueText"": ""}"" } ]}
";
            CodeAssert.AreEqual(expected, actual);
        }
    }
}
