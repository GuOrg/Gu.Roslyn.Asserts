namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using Gu.Roslyn.Asserts.Internals;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    public class SyntaxFactoryWriter
    {
        private readonly Writer writer = new Writer();

        public static string Write(string code)
        {
            var compilationUnit = SyntaxFactory.ParseCompilationUnit(code);
            var writer = new SyntaxFactoryWriter().Write(compilationUnit);
            return writer.writer.ToString();
        }

        private SyntaxFactoryWriter Write(SyntaxNode node)
        {
            switch (node)
            {
                case CompilationUnitSyntax compilationUnit:
                    return this.AppendLine("SyntaxFactory.CompilationUnit(")
                               .PushIndent()
                               .WriteArgument("externs", compilationUnit.Externs)
                               .WriteArgument("usings", compilationUnit.Usings)
                               .WriteArgument("members", compilationUnit.Members)
                               .WriteArgument("attributeLists", compilationUnit.AttributeLists, closeArgumentList: true)
                               .PopIndent();
                case NamespaceDeclarationSyntax namespaceDeclaration:
                    return this.AppendLine("SyntaxFactory.NamespaceDeclaration(")
                               .PushIndent()
                               .WriteArgument("namespaceKeyword", namespaceDeclaration.NamespaceKeyword)
                               .WriteArgument("name", namespaceDeclaration.Name)
                               .WriteArgument("openBraceToken", namespaceDeclaration.OpenBraceToken)
                               .WriteArgument("externs", namespaceDeclaration.Externs)
                               .WriteArgument("usings", namespaceDeclaration.Usings)
                               .WriteArgument("members", namespaceDeclaration.Members)
                               .WriteArgument("closeBraceToken", namespaceDeclaration.CloseBraceToken)
                               .WriteArgument("semicolonToken", namespaceDeclaration.SemicolonToken, closeArgumentList: true)
                               .PopIndent();
                case ClassDeclarationSyntax classDeclaration:
                    return this.AppendLine("SyntaxFactory.ClassDeclaration(")
                               .PushIndent()
                               .WriteArgument("attributeLists", classDeclaration.AttributeLists)
                               .WriteArgument("modifiers", classDeclaration.Modifiers)
                               .WriteArgument("keyword", classDeclaration.Keyword)
                               .WriteArgument("identifier", classDeclaration.Identifier)
                               .WriteArgument("typeParameterList", classDeclaration.TypeParameterList)
                               .WriteArgument("baseList", classDeclaration.BaseList)
                               .WriteArgument("constraintClauses", classDeclaration.ConstraintClauses)
                               .WriteArgument("openBraceToken", classDeclaration.OpenBraceToken)
                               .WriteArgument("members", classDeclaration.Members)
                               .WriteArgument("closeBraceToken", classDeclaration.CloseBraceToken)
                               .WriteArgument("semicolonToken", classDeclaration.SemicolonToken, closeArgumentList: true)
                               .PopIndent();
                case ConstructorDeclarationSyntax constructorDeclaration:
                    return this.AppendLine("SyntaxFactory.ConstructorDeclaration(")
                               .PushIndent()
                               .WriteArgument("attributeLists", constructorDeclaration.AttributeLists)
                               .WriteArgument("modifiers", constructorDeclaration.Modifiers)
                               .WriteArgument("identifier", constructorDeclaration.Identifier)
                               .WriteArgument("parameterList", constructorDeclaration.ParameterList)
                               .WriteArgument("initializer", constructorDeclaration.Initializer)
                               .WriteArgument("body", constructorDeclaration.Body)
                               .WriteArgument("expressionBody", constructorDeclaration.ExpressionBody)
                               .WriteArgument("semicolonToken", constructorDeclaration.SemicolonToken, closeArgumentList: true)
                               .PopIndent();
                case ParameterListSyntax parameterList:
                    return this.AppendLine("SyntaxFactory.ParameterList(")
                               .PushIndent()
                               .WriteArgument("openParenToken", parameterList.OpenParenToken)
                               .WriteArgument("parameters", parameterList.Parameters)
                               .WriteArgument("closeParenToken", parameterList.CloseParenToken, closeArgumentList: true)
                               .PopIndent();
                case BlockSyntax block:
                    return this.AppendLine("SyntaxFactory.Block(")
                               .PushIndent()
                               .WriteArgument("openBraceToken", block.OpenBraceToken)
                               .WriteArgument("statements", block.Statements)
                               .WriteArgument("closeBraceToken", block.CloseBraceToken, closeArgumentList: true)
                               .PopIndent();
                case LocalDeclarationStatementSyntax localDeclarationStatement:
                    return this.AppendLine("SyntaxFactory.LocalDeclarationStatement(")
                               .PushIndent()
                               .WriteArgument("modifiers", localDeclarationStatement.Modifiers)
                               .WriteArgument("declaration", localDeclarationStatement.Declaration)
                               .WriteArgument("semicolonToken", localDeclarationStatement.SemicolonToken, closeArgumentList: true)
                               .PopIndent();
                case FieldDeclarationSyntax fieldDeclaration:
                    return this.AppendLine("SyntaxFactory.FieldDeclaration(")
                               .PushIndent()
                               .WriteArgument("attributeLists", fieldDeclaration.AttributeLists)
                               .WriteArgument("modifiers", fieldDeclaration.Modifiers)
                               .WriteArgument("declaration", fieldDeclaration.Declaration)
                               .WriteArgument("semicolonToken", fieldDeclaration.SemicolonToken, closeArgumentList: true)
                               .PopIndent();
                case VariableDeclarationSyntax variableDeclaration:
                    return this.AppendLine("SyntaxFactory.VariableDeclaration(")
                               .PushIndent()
                               .WriteArgument("type", variableDeclaration.Type)
                               .WriteArgument("variables", variableDeclaration.Variables, closeArgumentList: true)
                               .PopIndent();
                case VariableDeclaratorSyntax variableDeclarator:
                    return this.AppendLine("SyntaxFactory.VariableDeclarator(")
                               .PushIndent()
                               .WriteArgument("identifier", variableDeclarator.Identifier)
                               .WriteArgument("argumentList", variableDeclarator.ArgumentList)
                               .WriteArgument("initializer", variableDeclarator.Initializer, closeArgumentList: true)
                               .PopIndent();
                case EqualsValueClauseSyntax equalsValueClause:
                    return this.AppendLine("SyntaxFactory.EqualsValueClause(")
                               .PushIndent()
                               .WriteArgument("equalsToken", equalsValueClause.EqualsToken)
                               .WriteArgument("value", equalsValueClause.Value, closeArgumentList: true)
                               .PopIndent();
                case LiteralExpressionSyntax literalExpression:
                    return this.AppendLine("SyntaxFactory.LiteralExpression(")
                               .PushIndent()
                               .WriteArgument("kind", literalExpression.Kind())
                               .WriteArgument("token", literalExpression.Token, closeArgumentList: true)
                               .PopIndent();
                case PredefinedTypeSyntax predefinedType:
                    return this.AppendLine("SyntaxFactory.PredefinedType(")
                               .PushIndent()
                               .WriteArgument("keyword", predefinedType.Keyword, closeArgumentList: true)
                               .PopIndent();
                case InterpolatedStringExpressionSyntax interpolatedStringExpression:
                    return this.AppendLine("SyntaxFactory.InterpolatedStringExpression(")
                               .PushIndent()
                               .WriteArgument("stringStartToken", interpolatedStringExpression.StringStartToken)
                               .WriteArgument("contents", interpolatedStringExpression.Contents)
                               .WriteArgument("stringEndToken", interpolatedStringExpression.StringEndToken, closeArgumentList: true)
                               .PopIndent();
                case InterpolatedStringTextSyntax interpolatedStringText:
                    return this.AppendLine("SyntaxFactory.InterpolatedStringText(")
                               .PushIndent()
                               .WriteArgument("textToken", interpolatedStringText.TextToken, closeArgumentList: true)
                               .PopIndent();
                case InterpolationSyntax interpolation:
                    return this.AppendLine("SyntaxFactory.Interpolation(")
                               .PushIndent()
                               .WriteArgument("openBraceToken", interpolation.OpenBraceToken)
                               .WriteArgument("expression", interpolation.Expression)
                               .WriteArgument("alignmentClause", interpolation.AlignmentClause)
                               .WriteArgument("formatClause", interpolation.FormatClause)
                               .WriteArgument("closeBraceToken", interpolation.CloseBraceToken, closeArgumentList: true)
                               .PopIndent();
                case IdentifierNameSyntax identifierName when !identifierName.Identifier.HasLeadingTrivia &&
                                                              !identifierName.Identifier.HasTrailingTrivia:
                    return this.Append("SyntaxFactory.IdentifierName(\"")
                               .Append(identifierName.Identifier.Text)
                               .Append("\")");
                case IdentifierNameSyntax identifierName:
                    return this.AppendLine("SyntaxFactory.IdentifierName(")
                               .PushIndent()
                               .WriteArgument("identifier", identifierName.Identifier, closeArgumentList: true)
                               .PopIndent();
                case QualifiedNameSyntax qualifiedName:
                    return this.AppendLine("SyntaxFactory.QualifiedName(")
                               .PushIndent()
                               .WriteArgument("left", qualifiedName.Left)
                               .WriteArgument("dotToken", qualifiedName.DotToken)
                               .WriteArgument("right", qualifiedName.Right, closeArgumentList: true)
                               .PopIndent();
                default:
#pragma warning disable GU0090 // Don't throw NotImplementedException.
                    throw new NotImplementedException($"{nameof(SyntaxFactoryWriter)}.{nameof(this.Write)}({nameof(SyntaxNode)}) does not handle {node.Kind()}");
#pragma warning restore GU0090 // Don't throw NotImplementedException.
            }
        }

        private SyntaxFactoryWriter Write(SyntaxToken token)
        {
            if (token.Kind() == SyntaxKind.None)
            {
                this.writer.Append("default");
                return this;
            }

            switch (token.Kind())
            {
                case SyntaxKind.BadToken:
                    return this.AppendLine("SyntaxFactory.BadToken(")
                               .PushIndent()
                               .WriteArgument("leading", token.LeadingTrivia)
                               .WriteArgument("text", token.Text)
                               .WriteArgument("trailing", token.TrailingTrivia, closeArgumentList: true)
                               .PopIndent();
                case SyntaxKind.IdentifierToken when token.IsContextualKeyword():
                    return this.AppendLine("SyntaxFactory.Identifier(")
                               .PushIndent()
                               .WriteArgument("leading", token.LeadingTrivia)
                               .WriteArgument("contextualKind", token.Kind())
                               .WriteArgument("text", token.Text)
                               .WriteArgument("valueText", token.ValueText)
                               .WriteArgument("trailing", token.TrailingTrivia, closeArgumentList: true)
                               .PopIndent();
                case SyntaxKind.IdentifierToken when token.Text != token.ValueText:
                    return this.AppendLine("SyntaxFactory.VerbatimIdentifier(")
                               .PushIndent()
                               .WriteArgument("leading", token.LeadingTrivia)
                               .WriteArgument("text", token.Text)
                               .WriteArgument("valueText", token.ValueText)
                               .WriteArgument("trailing", token.TrailingTrivia, closeArgumentList: true)
                               .PopIndent();
                case SyntaxKind.IdentifierToken when token.HasLeadingTrivia || token.HasTrailingTrivia:
                    return this.AppendLine("SyntaxFactory.Identifier(")
                               .PushIndent()
                               .WriteArgument("leading", token.LeadingTrivia)
                               .WriteArgument("text", token.Text)
                               .WriteArgument("trailing", token.TrailingTrivia, closeArgumentList: true)
                               .PopIndent();
                case SyntaxKind.IdentifierToken:
                    return this.Append($"SyntaxFactory.Identifier(\"{token.Text}\")");
                case SyntaxKind.CharacterLiteralToken when token.HasLeadingTrivia || token.HasTrailingTrivia:
                case SyntaxKind.NumericLiteralToken when token.HasLeadingTrivia || token.HasTrailingTrivia:
                    return this.AppendLine("SyntaxFactory.Literal(")
                               .PushIndent()
                               .WriteArgument("leading", token.LeadingTrivia)
                               .WriteArgument("text", token.Text)
                               .WriteArgument("value", token.Value)
                               .WriteArgument("trailing", token.TrailingTrivia, closeArgumentList: true)
                               .PopIndent();
                case SyntaxKind.CharacterLiteralToken when token.Text != token.ValueText:
                case SyntaxKind.NumericLiteralToken when token.Text != token.ValueText:
                    return this.AppendLine("SyntaxFactory.Literal(")
                               .PushIndent()
                               .WriteArgument("text", token.Text)
                               .WriteArgument("value", token.Value, closeArgumentList: true)
                               .PopIndent();
                case SyntaxKind.CharacterLiteralToken:
                case SyntaxKind.NumericLiteralToken:
                    return this.Append($"SyntaxFactory.Literal({token.Value})");
                case SyntaxKind.XmlEntityLiteralToken:
                    return this.AppendLine("SyntaxFactory.XmlEntity(")
                               .PushIndent()
                               .WriteArgument("leading", token.LeadingTrivia)
                               .WriteArgument("text", token.Text)
                               .WriteArgument("value", token.Value)
                               .WriteArgument("trailing", token.TrailingTrivia, closeArgumentList: true)
                               .PopIndent();
                case SyntaxKind.XmlTextLiteralToken when token.HasLeadingTrivia || token.HasTrailingTrivia:
                    return this.AppendLine("SyntaxFactory.XmlTextLiteral(")
                               .PushIndent()
                               .WriteArgument("leading", token.LeadingTrivia)
                               .WriteArgument("text", token.Text)
                               .WriteArgument("value", token.Value)
                               .WriteArgument("trailing", token.TrailingTrivia, closeArgumentList: true)
                               .PopIndent();
                case SyntaxKind.XmlTextLiteralToken when token.Text != token.ValueText:
                    return this.AppendLine("SyntaxFactory.XmlTextLiteral(")
                               .PushIndent()
                               .WriteArgument("text", token.Text)
                               .WriteArgument("value", token.Value, closeArgumentList: true)
                               .PopIndent();
                case SyntaxKind.XmlTextLiteralToken:
                    return this.Append($"SyntaxFactory.XmlTextLiteral({token.Value})");
                case SyntaxKind.XmlTextLiteralNewLineToken when token.HasLeadingTrivia || token.HasTrailingTrivia:
                    return this.AppendLine("SyntaxFactory.XmlTextNewLine(")
                               .PushIndent()
                               .WriteArgument("leading", token.LeadingTrivia)
                               .WriteArgument("text", token.Text)
                               .WriteArgument("value", token.Value)
                               .WriteArgument("trailing", token.TrailingTrivia, closeArgumentList: true)
                               .PopIndent();
                case SyntaxKind.XmlTextLiteralNewLineToken:
                    return this.Append($"SyntaxFactory.XmlTextNewLine({token.Text})");

                default:
                    if (token.Text != token.ValueText)
                    {
                        return this.AppendLine("SyntaxFactory.Token(")
                                   .PushIndent()
                                   .WriteArgument("leading", token.LeadingTrivia)
                                   .WriteArgument("kind", token.Kind())
                                   .WriteArgument("text", token.Text)
                                   .WriteArgument("valueText", token.ValueText)
                                   .WriteArgument("trailing", token.TrailingTrivia, closeArgumentList: true)
                                   .PopIndent();
                    }
                    else if (token.HasLeadingTrivia || token.HasTrailingTrivia)
                    {
                        return this.AppendLine("SyntaxFactory.Token(")
                                   .PushIndent()
                                   .WriteArgument("leading", token.LeadingTrivia)
                                   .WriteArgument("kind", token.Kind())
                                   .WriteArgument("trailing", token.TrailingTrivia, closeArgumentList: true)
                                   .PopIndent();
                    }

                    return this.Append($"SyntaxFactory.Token(SyntaxKind.{token.Kind()})");
            }
        }

        private SyntaxFactoryWriter Write(SyntaxTrivia trivia)
        {
            throw new NotImplementedException("Not writing trivia yet.");
        }

        private SyntaxFactoryWriter WriteArgument(string parameter, SyntaxNode node, bool closeArgumentList = false)
        {
            this.Append(parameter).Append(": ");
            if (node == null)
            {
                return this.Append("default").CloseArgument(closeArgumentList);
            }

            return this.Write(node).CloseArgument(closeArgumentList);
        }

        private SyntaxFactoryWriter WriteArgument(string parameter, SyntaxToken token, bool closeArgumentList = false)
        {
            return this.Append(parameter)
                       .Append(": ")
                       .Write(token)
                       .CloseArgument(closeArgumentList);
        }

        private SyntaxFactoryWriter WriteArgument<T>(string parameter, SyntaxList<T> syntaxList, bool closeArgumentList = false)
            where T : SyntaxNode
        {
            this.writer.Append(parameter).Append(": ");
            switch (syntaxList.Count)
            {
                case 0:
                    this.Append("default").CloseArgument(closeArgumentList);
                    return this;
                case 1:
                    return this.AppendLine($"SyntaxFactory.SingletonList<{typeof(T).Name}>(")
                               .PushIndent()
                               .Write(syntaxList[0])
                               .Append(")")
                               .CloseArgument(closeArgumentList)
                               .PopIndent();
                default:
                    this.AppendLine($"SyntaxFactory.List(")
                        .PushIndent()
                        .AppendLine($"new {typeof(T).Name}[]")
                        .AppendLine("{")
                        .PushIndent();
                    foreach (var node in syntaxList)
                    {
                        _ = this.Write(node).AppendLine(",");
                    }

                    return this.PopIndent()
                               .Append("})")
                               .CloseArgument(closeArgumentList)
                               .PopIndent();
            }
        }

        private SyntaxFactoryWriter WriteArgument<T>(string parameter, SeparatedSyntaxList<T> syntaxList, bool closeArgumentList = false)
            where T : SyntaxNode
        {
            this.writer.Append(parameter).Append(": ");
            switch (syntaxList.Count)
            {
                case 0:
                    this.Append("default").CloseArgument(closeArgumentList);
                    return this;
                case 1:
                    return this.AppendLine($"SyntaxFactory.SingletonSeparatedList(")
                               .PushIndent()
                               .Write(syntaxList[0])
                               .Append(")")
                               .CloseArgument(closeArgumentList)
                               .PopIndent();
                default:
                    this.AppendLine($"SyntaxFactory.SeparatedList<{typeof(T).Name}>(")
                        .PushIndent();
                    for (var i = 0; i < syntaxList.Count; i++)
                    {
                        _ = this.Write(syntaxList[i]);
                        if (i < syntaxList.Count - 1)
                        {
                            this.writer.AppendLine(",");
                        }
                    }

                    return this.Append(")")
                               .CloseArgument(closeArgumentList)
                               .PopIndent();
            }
        }

        private SyntaxFactoryWriter WriteArgument(string parameter, SyntaxTokenList tokenList, bool closeArgumentList = false)
        {
            this.writer.Append(parameter).Append(": ");
            switch (tokenList.Count)
            {
                case 0:
                    this.Append("default").CloseArgument(closeArgumentList);
                    return this;
                case 1:
                    return this.AppendLine($"SyntaxFactory.TokenList(")
                               .PushIndent()
                               .Write(tokenList[0])
                               .Append(")")
                               .CloseArgument(closeArgumentList)
                               .PopIndent();
                default:
                    this.AppendLine($"SyntaxFactory.TokenList(")
                        .PushIndent();
                    for (var i = 0; i < tokenList.Count; i++)
                    {
                        _ = this.Write(tokenList[i]);
                        if (i <= tokenList.Count - 1)
                        {
                            this.AppendLine(",");
                        }
                    }

                    this.writer.Append(")");
                    return this.CloseArgument(closeArgumentList)
                               .PopIndent();
            }
        }

        private SyntaxFactoryWriter WriteArgument(string parameter, SyntaxTriviaList triviaList, bool closeArgumentList = false)
        {
            this.writer.Append(parameter).Append(": ");
            switch (triviaList.Count)
            {
                case 0:
                    this.Append("default").CloseArgument(closeArgumentList);
                    return this;
                case 1 when TryGetSingleLine(out var text):
                    return this.Append(text)
                               .CloseArgument(closeArgumentList);
                default:
                    this.AppendLine($"SyntaxFactory.TriviaList(")
                        .PushIndent();
                    for (var i = 0; i < triviaList.Count; i++)
                    {
                        _ = this.Write(triviaList[i]);
                        if (i <= triviaList.Count - 1)
                        {
                            this.AppendLine(",");
                        }
                    }

                    this.writer.Append(")");
                    return this.CloseArgument(closeArgumentList)
                               .PopIndent();
            }

            bool TryGetSingleLine(out string result)
            {
                if (!triviaList.Any())
                {
                    result = "default";
                    return true;
                }

                if (triviaList.TrySingle(out var trivia))
                {
                    if (trivia.IsKind(SyntaxKind.None))
                    {
                        result = "default";
                        return true;
                    }

                    switch (trivia.Kind())
                    {
                        case SyntaxKind.WhitespaceTrivia:
                            result = trivia.ToString() == " "
                                ? "SyntaxFactory.TriviaList(SyntaxFactory.Space)"
                                : $"SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(\"{trivia.ToString()}\"))";
                            return true;
                        case SyntaxKind.EndOfLineTrivia:
                            result = "SyntaxFactory.TriviaList(SyntaxFactory.LineFeed)";
                            return true;
                    }
                }

                result = null;
                return false;
            }
        }

        private SyntaxFactoryWriter WriteArgument(string parameter, SyntaxKind kind, bool closeArgumentList = false)
        {
            return this.Append(parameter)
                       .Append(": ")
                       .Append("SyntaxKind.")
                       .Append(kind.ToString())
                       .CloseArgument(closeArgumentList);
        }

        private SyntaxFactoryWriter WriteArgument(string parameter, string text, bool closeArgumentList = false)
        {
            this.Append(parameter)
                      .Append(": ")
                      .Append("\"");
            foreach (var c in text)
            {
                if (c == '\\' ||
                    c == '"')
                {
                    this.writer.Append('\\');
                }

                this.writer.Append(c);
            }

            return this.Append("\"")
                      .CloseArgument(closeArgumentList);
        }

        private SyntaxFactoryWriter WriteArgument(string parameter, object value, bool closeArgumentList = false)
        {
            switch (value)
            {
                case string text:
                    return this.WriteArgument(parameter, text, closeArgumentList);
                case SyntaxToken _:
                case SyntaxNode _:
                    throw new InvalidOperationException("You did not want this overload.");
                default:
                    return this.Append(parameter)
                               .Append(": ")
                               .Append(value.ToString())
                               .CloseArgument(closeArgumentList);
            }
        }

        private SyntaxFactoryWriter Append(string text)
        {
            this.writer.Append(text);
            return this;
        }

        private SyntaxFactoryWriter AppendLine(string text)
        {
            this.writer.AppendLine(text);
            return this;
        }

        private SyntaxFactoryWriter CloseArgument(bool closeArgumentList)
        {
            if (closeArgumentList)
            {
                this.writer.Append(")");
                return this;
            }
            else
            {
                this.writer.AppendLine(",");
                return this;
            }
        }

        private SyntaxFactoryWriter PushIndent()
        {
            _ = this.writer.PushIndent();
            return this;
        }

        private SyntaxFactoryWriter PopIndent()
        {
            _ = this.writer.PopIndent();
            return this;
        }

        private class Writer
        {
            private readonly StringBuilder builder = new StringBuilder();
            private string indentation = string.Empty;
            private bool newLine = true;

            public Writer AppendLine(string text)
            {
                Debug.Assert(text != ")", "Probably a bug here, don't think we ever want newline after )");
                if (this.newLine)
                {
                    this.builder.Append(this.indentation);
                }

                this.builder.AppendLine(text);
                this.newLine = true;
                return this;
            }

            public Writer Append(string text)
            {
                if (this.newLine)
                {
                    this.builder.Append(this.indentation);
                }

                this.builder.Append(text);
                this.newLine = false;
                return this;
            }

            public Writer Append(char c)
            {
                if (this.newLine)
                {
                    this.builder.Append(this.indentation);
                }

                this.builder.Append(c);
                this.newLine = false;
                return this;
            }

            public Writer PushIndent()
            {
                this.indentation += "    ";
                return this;
            }

            public Writer PopIndent()
            {
                this.indentation = this.indentation.Substring(0, this.indentation.Length - 4);
                return this;
            }

            public override string ToString() => this.builder.ToString();
        }
    }
}
