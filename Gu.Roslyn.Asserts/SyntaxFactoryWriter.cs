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
                case IdentifierNameSyntax identifierName:
                    if (!identifierName.Identifier.HasLeadingTrivia &&
                        !identifierName.Identifier.HasTrailingTrivia)
                    {
                        return this.Append("SyntaxFactory.IdentifierName(\"")
                                   .Append(identifierName.Identifier.Text)
                                   .Append("\")");
                    }

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

            //SyntaxFactory.Identifier(SyntaxFactory.TriviaList(), "id", SyntaxFactory.TriviaList());
            //SyntaxFactory.Token(SyntaxTriviaList.Empty, SyntaxKind.AbstractKeyword, SyntaxTriviaList.Empty);
            //SyntaxFactory.Token(SyntaxTriviaList.Empty, SyntaxKind.IdentifierToken, "text", "valueText", SyntaxTriviaList.Empty);
            if (!token.HasLeadingTrivia &&
                !token.HasTrailingTrivia)
            {
                switch (token.Kind())
                {
                    case SyntaxKind.IdentifierToken:
                        this.writer.Append($"SyntaxFactory.Identifier(\"{token.Text}\")");
                        break;
                    default:
                        this.writer.Append($"SyntaxFactory.Token(SyntaxKind.{token.Kind()})");
                        break;
                }

                return this;
            }

            if (TryGetSingleLine(token.LeadingTrivia, out var leading) &&
                TryGetSingleLine(token.TrailingTrivia, out var trailing))
            {
                switch (token.Kind())
                {
                    case SyntaxKind.IdentifierToken:
                        this.writer.Append($"SyntaxFactory.Identifier({leading}, \"{token.Text}\", {trailing})");
                        break;
                    default:
                        this.writer.Append($"SyntaxFactory.Token({leading}, SyntaxKind.{token.Kind()}, {trailing})");
                        break;
                }

                return this;
            }

            throw new NotImplementedException();
            //switch (token.Kind())
            //{
            //    case SyntaxKind.IdentifierToken:
            //        this.AppendLine($"SyntaxFactory.Identifier(")
            //            .PushIndent()
            //            .WriteArgument("leading", token.LeadingTrivia, ",")
            //            .WriteArgument("name", token.Text, ",")
            //            .WriteArgument("leading", token.LeadingTrivia, ")")
            //            .PopIndent();
            //        break;
            //    default:
            //        this.AppendLine($"SyntaxFactory.Token(")
            //            .PushIndent()
            //            .WriteArgument("leading", token.LeadingTrivia, ",")
            //            .WriteArgument("name", token.Text, ",")
            //            .WriteArgument("leading", token.LeadingTrivia, ")")
            //            .PopIndent();
            //        break;
            //}

            return this;

            bool TryGetSingleLine(SyntaxTriviaList triviaList, out string result)
            {
                if (!triviaList.Any())
                {
                    result = "default";
                    return true;
                }

                if (triviaList.TrySingle(out var single))
                {
                    switch (single.Kind())
                    {
                        case SyntaxKind.WhitespaceTrivia:
                            result = single.ToString() == " "
                                ? "SyntaxFactory.TriviaList(SyntaxFactory.Space)"
                                : $"SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(\"{single.ToString()}\"))";
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

        private SyntaxFactoryWriter Write(SyntaxTriviaList trivia)
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
                    this.AppendLine($"SyntaxFactory.List<{typeof(T).Name}>(")
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

        private SyntaxFactoryWriter CloseArgument(bool last)
        {
            if (last)
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
