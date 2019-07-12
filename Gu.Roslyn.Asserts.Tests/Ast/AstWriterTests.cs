namespace Gu.Roslyn.Asserts.Tests
{
    using System.Linq;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;
    using NUnit.Framework;

    public static class AstWriterTests
    {
        [Test]
        public static void SimpleClass()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class C
    {
    }
}");
            var node = tree.FindClassDeclaration("C");
            var actual = AstWriter.Serialize(node);
            var expected = "{ ClassDeclaration\r\n" +
                           "  ChildTokens: [ { PublicKeyword LeadingTrivia: [ { WhitespaceTrivia: \"    \" } ] TrailingTrivia: [ { WhitespaceTrivia: \" \" } ] }\r\n" +
                           "                 { ClassKeyword TrailingTrivia: [ { WhitespaceTrivia: \" \" } ] }\r\n" +
                           "                 { IdentifierToken Text: \"C\" TrailingTrivia: [ EndOfLineTrivia ] }\r\n" +
                           "                 { OpenBraceToken Text: \"{\" LeadingTrivia: [ { WhitespaceTrivia: \"    \" } ] TrailingTrivia: [ EndOfLineTrivia ] }\r\n" +
                           "                 { CloseBraceToken Text: \"}\" LeadingTrivia: [ { WhitespaceTrivia: \"    \" } ] TrailingTrivia: [ EndOfLineTrivia ] } ] }";
            CodeAssert.AreEqual(expected, actual);
        }

        [Test]
        public static void SimpleClassAllTrivia()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class C
    {
    }
}");
            var node = tree.FindClassDeclaration("C");
            var actual = AstWriter.Serialize(node, new AstWriterSettings(AstFormat.Light, AstTrivia.Node | AstTrivia.Token, ignoreEmptyTriva: false));
            var expected = "{ ClassDeclaration LeadingTrivia: [ { WhitespaceTrivia: \"    \" } ] TrailingTrivia: [ EndOfLineTrivia ]\r\n" +
                           "  ChildTokens: [ { PublicKeyword LeadingTrivia: [ { WhitespaceTrivia: \"    \" } ] TrailingTrivia: [ { WhitespaceTrivia: \" \" } ] }\r\n" +
                           "                 { ClassKeyword TrailingTrivia: [ { WhitespaceTrivia: \" \" } ] }\r\n" +
                           "                 { IdentifierToken Text: \"C\" TrailingTrivia: [ EndOfLineTrivia ] }\r\n" +
                           "                 { OpenBraceToken Text: \"{\" LeadingTrivia: [ { WhitespaceTrivia: \"    \" } ] TrailingTrivia: [ EndOfLineTrivia ] }\r\n" +
                           "                 { CloseBraceToken Text: \"}\" LeadingTrivia: [ { WhitespaceTrivia: \"    \" } ] TrailingTrivia: [ EndOfLineTrivia ] } ] }";
            CodeAssert.AreEqual(expected, actual);
        }

        [Test]
        public static void SimpleClassJson()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class C
    {
    }
}");
            var node = tree.FindClassDeclaration("C");
            var actual = AstWriter.Serialize(node, AstWriterSettings.DefaultJson);
            var expected = "{ \"Kind\": \"ClassDeclaration\",\r\n" +
                           "  \"ChildTokens\": [ { \"Kind\": \"PublicKeyword\", \"Text\": \"public\", \"LeadingTrivia\": [ { \"Kind\": \"WhitespaceTrivia\", \"Text\": \"    \" } ], \"TrailingTrivia\": [ { \"Kind\": \"WhitespaceTrivia\", \"Text\": \" \" } ] },\r\n" +
                           "                   { \"Kind\": \"ClassKeyword\", \"Text\": \"class\", \"TrailingTrivia\": [ { \"Kind\": \"WhitespaceTrivia\", \"Text\": \" \" } ] },\r\n" +
                           "                   { \"Kind\": \"IdentifierToken\", \"Text\": \"C\", \"TrailingTrivia\": [ { \"Kind\": \"EndOfLineTrivia\", \"Text\": \"\\r\\n\" } ] },\r\n" +
                           "                   { \"Kind\": \"OpenBraceToken\", \"Text\": \"{\", \"LeadingTrivia\": [ { \"Kind\": \"WhitespaceTrivia\", \"Text\": \"    \" } ], \"TrailingTrivia\": [ { \"Kind\": \"EndOfLineTrivia\", \"Text\": \"\\r\\n\" } ] },\r\n" +
                           "                   { \"Kind\": \"CloseBraceToken\", \"Text\": \"}\", \"LeadingTrivia\": [ { \"Kind\": \"WhitespaceTrivia\", \"Text\": \"    \" } ], \"TrailingTrivia\": [ { \"Kind\": \"EndOfLineTrivia\", \"Text\": \"\\r\\n\" } ] } ] }";
            CodeAssert.AreEqual(expected, actual);
        }

        [Test]
        public static void SimpleClassJsonAllTrivia()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    public class C
    {
    }
}");
            var node = tree.FindClassDeclaration("C");
            var actual = AstWriter.Serialize(node, new AstWriterSettings(AstFormat.Json, AstTrivia.Node | AstTrivia.Token, ignoreEmptyTriva: false));
            var expected = "{ \"Kind\": \"ClassDeclaration\", \"LeadingTrivia\": [ { \"Kind\": \"WhitespaceTrivia\", \"Text\": \"    \" } ], \"TrailingTrivia\": [ { \"Kind\": \"EndOfLineTrivia\", \"Text\": \"\\r\\n\" } ],\r\n" +
                           "  \"ChildTokens\": [ { \"Kind\": \"PublicKeyword\", \"Text\": \"public\", \"LeadingTrivia\": [ { \"Kind\": \"WhitespaceTrivia\", \"Text\": \"    \" } ], \"TrailingTrivia\": [ { \"Kind\": \"WhitespaceTrivia\", \"Text\": \" \" } ] },\r\n" +
                           "                   { \"Kind\": \"ClassKeyword\", \"Text\": \"class\", \"TrailingTrivia\": [ { \"Kind\": \"WhitespaceTrivia\", \"Text\": \" \" } ] },\r\n" +
                           "                   { \"Kind\": \"IdentifierToken\", \"Text\": \"C\", \"TrailingTrivia\": [ { \"Kind\": \"EndOfLineTrivia\", \"Text\": \"\\r\\n\" } ] },\r\n" +
                           "                   { \"Kind\": \"OpenBraceToken\", \"Text\": \"{\", \"LeadingTrivia\": [ { \"Kind\": \"WhitespaceTrivia\", \"Text\": \"    \" } ], \"TrailingTrivia\": [ { \"Kind\": \"EndOfLineTrivia\", \"Text\": \"\\r\\n\" } ] },\r\n" +
                           "                   { \"Kind\": \"CloseBraceToken\", \"Text\": \"}\", \"LeadingTrivia\": [ { \"Kind\": \"WhitespaceTrivia\", \"Text\": \"    \" } ], \"TrailingTrivia\": [ { \"Kind\": \"EndOfLineTrivia\", \"Text\": \"\\r\\n\" } ] } ] }";
            CodeAssert.AreEqual(expected, actual);
        }

        [Test]
        public static void Comment()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    /// <summary>
    /// Some text
    /// </summary>
    public class C
    {
    }
}");
            var comment = (DocumentationCommentTriviaSyntax)tree.FindClassDeclaration("C").GetLeadingTrivia().Single(x => x.HasStructure).GetStructure();
            var actual = AstWriter.Serialize(comment);
            var expected = "{ SingleLineDocumentationCommentTrivia\n" +
                           "  ChildTokens: [ { EndOfDocumentationCommentToken Text: \"\" } ]\n" +
                           "  ChildNodes:  [ { XmlText\n" +
                           "                   ChildTokens: [ { XmlTextLiteralToken Text: \" \" LeadingTrivia: [ { DocumentationCommentExteriorTrivia: \"///\" } ] } ] }\n" +
                           "                 { XmlElement\n" +
                           "                   ChildNodes:  [ { XmlElementStartTag\n" +
                           "                                    ChildTokens: [ { LessThanToken Text: \"<\" }\n" +
                           "                                                   { GreaterThanToken Text: \">\" } ]\n" +
                           "                                    ChildNodes:  [ { XmlName\n" +
                           "                                                     ChildTokens: [ { IdentifierToken Text: \"summary\" } ] } ] }\n" +
                           "                                  { XmlText\n" +
                           "                                    ChildTokens: [ { XmlTextLiteralNewLineToken Text: \"\\r\\n\" }\n" +
                           "                                                   { XmlTextLiteralToken Text: \" Some text\" LeadingTrivia: [ { DocumentationCommentExteriorTrivia: \"    ///\" } ] }\n" +
                           "                                                   { XmlTextLiteralNewLineToken Text: \"\\r\\n\" }\n" +
                           "                                                   { XmlTextLiteralToken Text: \" \" LeadingTrivia: [ { DocumentationCommentExteriorTrivia: \"    ///\" } ] } ] }\n" +
                           "                                  { XmlElementEndTag\n" +
                           "                                    ChildTokens: [ { LessThanSlashToken Text: \"</\" }\n" +
                           "                                                   { GreaterThanToken Text: \">\" } ]\n" +
                           "                                    ChildNodes:  [ { XmlName\n" +
                           "                                                     ChildTokens: [ { IdentifierToken Text: \"summary\" } ] } ] } ] }\n" +
                           "                 { XmlText\n" +
                           "                   ChildTokens: [ { XmlTextLiteralNewLineToken Text: \"\\r\\n\" } ] } ] }";
            CodeAssert.AreEqual(expected, actual);
        }

        [Test]
        public static void CommentAllTrivia()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    /// <summary>
    /// Some text
    /// </summary>
    public class C
    {
    }
}");
            var comment = (DocumentationCommentTriviaSyntax)tree.FindClassDeclaration("C").GetLeadingTrivia().Single(x => x.HasStructure).GetStructure();
            var actual = AstWriter.Serialize(comment, new AstWriterSettings(AstFormat.Light, AstTrivia.Node | AstTrivia.Token, ignoreEmptyTriva: false));
            var expected = "{ SingleLineDocumentationCommentTrivia LeadingTrivia: [ { DocumentationCommentExteriorTrivia: \"///\" } ]\n" +
                           "  ChildTokens: [ { EndOfDocumentationCommentToken Text: \"\" } ]\n" +
                           "  ChildNodes:  [ { XmlText LeadingTrivia: [ { DocumentationCommentExteriorTrivia: \"///\" } ]\n" +
                           "                   ChildTokens: [ { XmlTextLiteralToken Text: \" \" LeadingTrivia: [ { DocumentationCommentExteriorTrivia: \"///\" } ] } ] }\n" +
                           "                 { XmlElement\n" +
                           "                   ChildNodes:  [ { XmlElementStartTag\n" +
                           "                                    ChildTokens: [ { LessThanToken Text: \"<\" }\n" +
                           "                                                   { GreaterThanToken Text: \">\" } ]\n" +
                           "                                    ChildNodes:  [ { XmlName\n" +
                           "                                                     ChildTokens: [ { IdentifierToken Text: \"summary\" } ] } ] }\n" +
                           "                                  { XmlText\n" +
                           "                                    ChildTokens: [ { XmlTextLiteralNewLineToken Text: \"\\r\\n\" }\n" +
                           "                                                   { XmlTextLiteralToken Text: \" Some text\" LeadingTrivia: [ { DocumentationCommentExteriorTrivia: \"    ///\" } ] }\n" +
                           "                                                   { XmlTextLiteralNewLineToken Text: \"\\r\\n\" }\n" +
                           "                                                   { XmlTextLiteralToken Text: \" \" LeadingTrivia: [ { DocumentationCommentExteriorTrivia: \"    ///\" } ] } ] }\n" +
                           "                                  { XmlElementEndTag\n" +
                           "                                    ChildTokens: [ { LessThanSlashToken Text: \"</\" }\n" +
                           "                                                   { GreaterThanToken Text: \">\" } ]\n" +
                           "                                    ChildNodes:  [ { XmlName\n" +
                           "                                                     ChildTokens: [ { IdentifierToken Text: \"summary\" } ] } ] } ] }\n" +
                           "                 { XmlText\n" +
                           "                   ChildTokens: [ { XmlTextLiteralNewLineToken Text: \"\\r\\n\" } ] } ] }";
            CodeAssert.AreEqual(expected, actual);
        }

        [Test]
        public static void CommentIgnoreTrivia()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    /// <summary>
    /// Some text
    /// </summary>
    public class C
    {
    }
}");
            var comment = (DocumentationCommentTriviaSyntax)tree.FindClassDeclaration("C").GetLeadingTrivia().Single(x => x.HasStructure).GetStructure();
            var actual = AstWriter.Serialize(comment, new AstWriterSettings(AstFormat.Light, AstTrivia.Ignore, ignoreEmptyTriva: false));
            var expected = "{ SingleLineDocumentationCommentTrivia\n" +
                           "  ChildTokens: [ { EndOfDocumentationCommentToken Text: \"\" } ]\n" +
                           "  ChildNodes:  [ { XmlText\n" +
                           "                   ChildTokens: [ { XmlTextLiteralToken Text: \" \" } ] }\n" +
                           "                 { XmlElement\n" +
                           "                   ChildNodes:  [ { XmlElementStartTag\n" +
                           "                                    ChildTokens: [ { LessThanToken Text: \"<\" }\n" +
                           "                                                   { GreaterThanToken Text: \">\" } ]\n" +
                           "                                    ChildNodes:  [ { XmlName\n" +
                           "                                                     ChildTokens: [ { IdentifierToken Text: \"summary\" } ] } ] }\n" +
                           "                                  { XmlText\n" +
                           "                                    ChildTokens: [ { XmlTextLiteralNewLineToken Text: \"\\r\\n\" }\n" +
                           "                                                   { XmlTextLiteralToken Text: \" Some text\" }\n" +
                           "                                                   { XmlTextLiteralNewLineToken Text: \"\\r\\n\" }\n" +
                           "                                                   { XmlTextLiteralToken Text: \" \" } ] }\n" +
                           "                                  { XmlElementEndTag\n" +
                           "                                    ChildTokens: [ { LessThanSlashToken Text: \"</\" }\n" +
                           "                                                   { GreaterThanToken Text: \">\" } ]\n" +
                           "                                    ChildNodes:  [ { XmlName\n" +
                           "                                                     ChildTokens: [ { IdentifierToken Text: \"summary\" } ] } ] } ] }\n" +
                           "                 { XmlText\n" +
                           "                   ChildTokens: [ { XmlTextLiteralNewLineToken Text: \"\\r\\n\" } ] } ] }";
            CodeAssert.AreEqual(expected, actual);
        }

        [Test]
        public static void CommentJson()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    /// <summary>
    /// Some text
    /// </summary>
    public class C
    {
    }
}");
            var comment = (DocumentationCommentTriviaSyntax)tree.FindClassDeclaration("C").GetLeadingTrivia().Single(x => x.HasStructure).GetStructure();
            var actual = AstWriter.Serialize(comment, AstWriterSettings.DefaultJson);
            var expected = "{ \"Kind\": \"SingleLineDocumentationCommentTrivia\",\n" +
                           "  \"ChildTokens\": [ { \"Kind\": \"EndOfDocumentationCommentToken\", \"Text\": \"\" } ],\n" +
                           "  \"ChildNodes\":  [ { \"Kind\": \"XmlText\",\n" +
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

        [Test]
        public static void CommentJsonAllTrivia()
        {
            var tree = CSharpSyntaxTree.ParseText(@"
namespace RoslynSandbox
{
    /// <summary>
    /// Some text
    /// </summary>
    public class C
    {
    }
}");
            var comment = (DocumentationCommentTriviaSyntax)tree.FindClassDeclaration("C").GetLeadingTrivia().Single(x => x.HasStructure).GetStructure();
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
            var actual = AstWriter.Serialize(comment, new AstWriterSettings(AstFormat.Json, AstTrivia.Node | AstTrivia.Token, ignoreEmptyTriva: false));
            CodeAssert.AreEqual(expected, actual);
        }

        [Test]
        public static void SyntaxFactoryArgumentListIgnoreEmptyTrivia()
        {
            var actual = AstWriter.Serialize(SyntaxFactory.ArgumentList(), new AstWriterSettings(AstFormat.Light, AstTrivia.Token | AstTrivia.Node, ignoreEmptyTriva: true));
            var expected = "{ ArgumentList\n" +
                           "  ChildTokens: [ { OpenParenToken Text: \"(\" }\n" +
                           "                 { CloseParenToken Text: \")\" } ] }";
            CodeAssert.AreEqual(expected, actual);
        }

        [Test]
        public static void SyntaxFactoryArgumentListIncludeEmptyTrivia()
        {
            var actual = AstWriter.Serialize(SyntaxFactory.ArgumentList(), new AstWriterSettings(AstFormat.Light, AstTrivia.Token | AstTrivia.Node, ignoreEmptyTriva: false));
            var expected = "{ ArgumentList LeadingTrivia: [ { WhitespaceTrivia: \"\" } ] TrailingTrivia: [ { WhitespaceTrivia: \"\" } ]\n" +
                           "  ChildTokens: [ { OpenParenToken Text: \"(\" LeadingTrivia: [ { WhitespaceTrivia: \"\" } ] TrailingTrivia: [ { WhitespaceTrivia: \"\" } ] }\n" +
                           "                 { CloseParenToken Text: \")\" LeadingTrivia: [ { WhitespaceTrivia: \"\" } ] TrailingTrivia: [ { WhitespaceTrivia: \"\" } ] } ] }";
            CodeAssert.AreEqual(expected, actual);
        }
    }
}
