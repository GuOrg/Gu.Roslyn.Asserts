namespace Gu.Roslyn.Asserts.Tests
{
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public class AstWriterTests
    {
        [Test]
        [Explicit]
        public void Class()
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
            var expected = "{ ClassDeclaration LeadingTrivia: [ { WhitespaceTrivia: \"    \" } ] TrailingTrivia: [ EndOfLineTrivia ],\r\n" +
                           "    ChildTokens: [ { PublicKeyword LeadingTrivia: [ { WhitespaceTrivia: \"    \" } ] TrailingTrivia: [ { WhitespaceTrivia: \" \" } ] },\r\n" +
                           "                   { ClassKeyword TrailingTrivia: [ { WhitespaceTrivia: \" \" } ] },\r\n" +
                           "                   { IdentifierToken Text: \"Foo\" TrailingTrivia: [ EndOfLineTrivia ] },\r\n" +
                           "                   { OpenBraceToken LeadingTrivia: [ { WhitespaceTrivia: \"    \" } ], TrailingTrivia: [ EndOfLineTrivia ] },\r\n" +
                           "                   { CloseBraceToken LeadingTrivia: [ { WhitespaceTrivia: \"    \" } ], TrailingTrivia: [ EndOfLineTrivia ] } ] }";
            CodeAssert.AreEqual(expected, actual);
        }

        [Test]
        public void SimpleClassJson()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class Foo
    {
    }
}");
            var node = tree.FindClassDeclaration("Foo");
            var actual = AstWriter.Serialize(node, AstWriterSettings.EverythingJson);
            var expected = "{ \"Kind\": \"ClassDeclaration\", \"LeadingTrivia\": [ { \"Kind\": \"WhitespaceTrivia\", \"Text\": \"    \" } ], \"TrailingTrivia\": [ { \"Kind\": \"EndOfLineTrivia\", \"Text\": \"\\r\\n\" } ],\r\n" +
                           "  \"ChildTokens\": [ { \"Kind\": \"PublicKeyword\", \"Text\": \"public\", \"LeadingTrivia\": [ { \"Kind\": \"WhitespaceTrivia\", \"Text\": \"    \" } ], \"TrailingTrivia\": [ { \"Kind\": \"WhitespaceTrivia\", \"Text\": \" \" } ] },\r\n" +
                           "                   { \"Kind\": \"ClassKeyword\", \"Text\": \"class\", \"TrailingTrivia\": [ { \"Kind\": \"WhitespaceTrivia\", \"Text\": \" \" } ] },\r\n" +
                           "                   { \"Kind\": \"IdentifierToken\", \"Text\": \"Foo\", \"TrailingTrivia\": [ { \"Kind\": \"EndOfLineTrivia\", \"Text\": \"\\r\\n\" } ] },\r\n" +
                           "                   { \"Kind\": \"OpenBraceToken\", \"Text\": \"{\", \"LeadingTrivia\": [ { \"Kind\": \"WhitespaceTrivia\", \"Text\": \"    \" } ], \"TrailingTrivia\": [ { \"Kind\": \"EndOfLineTrivia\", \"Text\": \"\\r\\n\" } ] },\r\n" +
                           "                   { \"Kind\": \"CloseBraceToken\", \"Text\": \"}\", \"LeadingTrivia\": [ { \"Kind\": \"WhitespaceTrivia\", \"Text\": \"    \" } ], \"TrailingTrivia\": [ { \"Kind\": \"EndOfLineTrivia\", \"Text\": \"\\r\\n\" } ] } ] }";
            CodeAssert.AreEqual(expected, actual);
        }

        [Test]
        public void CommentJson()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    /// <summary>
    /// Some text
    /// </summary>
    public class Foo
    {
    }
}");
            var comment = (DocumentationCommentTriviaSyntax)tree.FindClassDeclaration("Foo").GetLeadingTrivia().Single(x => x.HasStructure).GetStructure();
            var actual = AstWriter.Serialize(comment, AstWriterSettings.EverythingJson);
            var expected = "{ \"Kind\": \"SingleLineDocumentationCommentTrivia\", \"LeadingTrivia\": [ { \"Kind\": \"DocumentationCommentExteriorTrivia\", \"Text\": \"///\" } ],\n" +
                           "  \"ChildTokens\": [ { \"Kind\": \"EndOfDocumentationCommentToken\", \"Text\": \"\" } ],\n" +
                           "  \"ChildNodes\":  [ { \"Kind\": \"XmlText\", \"LeadingTrivia\": [ { \"Kind\": \"DocumentationCommentExteriorTrivia\", \"Text\": \"///\" } ],\n" +
                           "                     \"ChildTokens\": [ { \"Kind\": \"XmlTextLiteralToken\", \"Text\": \" \", \"LeadingTrivia\": [ { \"Kind\": \"DocumentationCommentExteriorTrivia\", \"Text\": \"///\" } ] } ] },\n" +
                           "                   { \"Kind\": \"XmlElement\",\n" +
                           "                     \"ChildNodes\":  [ { \"Kind\": \"XmlElementStartTag\",\n" +
                           "                                        \"ChildTokens\": [ { \"Kind\": \"LessThanToken\", \"Text\": \"<\" },\n" +
                           "                                                         { \"Kind\": \"GreaterThanToken\", \"Text\": \">\" } ],\n" +
                           "                                        \"ChildNodes\":  [ { \"Kind\": \"XmlName\",\n" +
                           "                                                           \"ChildTokens\": [ { \"Kind\": \"IdentifierToken\", \"Text\": \"summary\" } ] } ] },\n" +
                           "                                      { \"Kind\": \"XmlText\",\n" +
                           "                                        \"ChildTokens\": [ { \"Kind\": \"XmlTextLiteralNewLineToken\", \"Text\": \"\\r\\n\" },\n" +
                           "                                                         { \"Kind\": \"XmlTextLiteralToken\", \"Text\": \" Some text\", \"LeadingTrivia\": [ { \"Kind\": \"DocumentationCommentExteriorTrivia\", \"Text\": \"    ///\" } ] },\n" +
                           "                                                         { \"Kind\": \"XmlTextLiteralNewLineToken\", \"Text\": \"\\r\\n\" },\n" +
                           "                                                         { \"Kind\": \"XmlTextLiteralToken\", \"Text\": \" \", \"LeadingTrivia\": [ { \"Kind\": \"DocumentationCommentExteriorTrivia\", \"Text\": \"    ///\" } ] } ] },\n" +
                           "                                      { \"Kind\": \"XmlElementEndTag\",\n" +
                           "                                        \"ChildTokens\": [ { \"Kind\": \"LessThanSlashToken\", \"Text\": \"</\" },\n" +
                           "                                                         { \"Kind\": \"GreaterThanToken\", \"Text\": \">\" } ],\n" +
                           "                                        \"ChildNodes\":  [ { \"Kind\": \"XmlName\",\n" +
                           "                                                           \"ChildTokens\": [ { \"Kind\": \"IdentifierToken\", \"Text\": \"summary\" } ] } ] } ] },\n" +
                           "                   { \"Kind\": \"XmlText\",\n" +
                           "                     \"ChildTokens\": [ { \"Kind\": \"XmlTextLiteralNewLineToken\", \"Text\": \"\\r\\n\" } ] } ] }";
            CodeAssert.AreEqual(expected, actual);
        }
    }
}
