namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Text;
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
                               .WriteArgument("externs", compilationUnit.Externs, ",")
                               .WriteArgument("usings", compilationUnit.Usings, ",")
                               .WriteArgument("members", compilationUnit.Members, ",")
                               .WriteArgument("attributeLists", compilationUnit.AttributeLists, ")")
                               .PopIndent();
                case NamespaceDeclarationSyntax namespaceDeclarationSyntax:
                    return this.AppendLine("SyntaxFactory.NamespaceDeclaration(")
                               .PushIndent()
                               .WriteArgument("namespaceKeyword", namespaceDeclarationSyntax.NamespaceKeyword, ":")
                               .WriteArgument("name", namespaceDeclarationSyntax.Name, ":")
                               .WriteArgument("openBraceToken", namespaceDeclarationSyntax.OpenBraceToken, ":")
                               .WriteArgument("externs", namespaceDeclarationSyntax.Externs, ":")
                               .WriteArgument("usings", namespaceDeclarationSyntax.Usings, ":")
                               .WriteArgument("members", namespaceDeclarationSyntax.Members, ":")
                               .WriteArgument("closeBraceToken", namespaceDeclarationSyntax.CloseBraceToken, ":")
                               .WriteArgument("semicolonToken", namespaceDeclarationSyntax.SemicolonToken, ")")
                               .PopIndent();
                case ClassDeclarationSyntax classDeclarationSyntax:
                    return this.AppendLine("SyntaxFactory.ClassDeclaration(")
                               .PushIndent()
                               .WriteArgument("attributeLists", classDeclarationSyntax.AttributeLists, ":")
                               .WriteArgument("modifiers", classDeclarationSyntax.Modifiers, ":")
                               .WriteArgument("keyword", classDeclarationSyntax.Keyword, ":")
                               .WriteArgument("identifier", classDeclarationSyntax.Identifier, ":")
                               .WriteArgument("typeParameterList", classDeclarationSyntax.TypeParameterList, ":")
                               .WriteArgument("baseList", classDeclarationSyntax.BaseList, ":")
                               .WriteArgument("constraintClauses", classDeclarationSyntax.ConstraintClauses, ":")
                               .WriteArgument("openBraceToken", classDeclarationSyntax.OpenBraceToken, ":")
                               .WriteArgument("members", classDeclarationSyntax.Members, ":")
                               .WriteArgument("closeBraceToken", classDeclarationSyntax.CloseBraceToken, ":")
                               .WriteArgument("semicolonToken", classDeclarationSyntax.SemicolonToken, ")")
                               .PopIndent();
                default:
#pragma warning disable GU0090 // Don't throw NotImplementedException.
                    throw new NotImplementedException($"{nameof(SyntaxFactoryWriter)}.{nameof(this.Write)}({nameof(SyntaxNode)}) does not handle {node.Kind()}");
#pragma warning restore GU0090 // Don't throw NotImplementedException.
            }
        }

        private SyntaxFactoryWriter WriteArgument(string parameter, SyntaxNode node, string commaOrParen)
        {
            this.writer.Append(parameter).Append(": ");
            if (node == null)
            {
                this.writer.Append("default").AppendLine(commaOrParen);
            }

            return this.Write(node).AppendLine(commaOrParen);
        }

        private SyntaxFactoryWriter WriteArgument(string parameter, SyntaxToken token, string commaOrParen)
        {
            this.writer.Append(parameter).Append(": ");
            _ = this.Write(token);
            this.writer.AppendLine(commaOrParen);
            return this;
        }

        private SyntaxFactoryWriter Write(SyntaxToken token)
        {
            if (token.Kind() == SyntaxKind.None)
            {
                this.writer.Append("default");
                return this;
            }

            if (token.HasLeadingTrivia || token.HasTrailingTrivia)
            {
                throw new NotImplementedException();
            }

            this.writer.Append($"SyntaxFactory.Token(SyntaxKind.{token.Kind()})");
            return this;
        }

        private SyntaxFactoryWriter WriteArgument<T>(string parameter, SyntaxList<T> syntaxList, string commaOrParen)
            where T : SyntaxNode
        {
            this.writer.Append(parameter).Append(": ");
            switch (syntaxList.Count)
            {
                case 0:
                    this.writer.Append("default").AppendLine(commaOrParen);
                    return this;
                case 1:
                    return this.AppendLine($"SyntaxFactory.SingletonList<{typeof(T).Name}>(")
                               .PushIndent()
                               .Write(syntaxList[0])
                               .AppendLine(")")
                               .AppendLine(commaOrParen)
                               .PopIndent();
                default:
                    this.AppendLine($"SyntaxFactory.List<{typeof(T).Name}>(")
                        .PushIndent();
                    foreach (var node in syntaxList)
                    {
                        this.Write(node).AppendLine(ReferenceEquals(node, syntaxList.Last()) ? ")" : ",");
                    }

                    return this.AppendLine(")")
                               .AppendLine(commaOrParen)
                               .PopIndent();
            }
        }

        private SyntaxFactoryWriter WriteArgument(string parameter, SyntaxTokenList tokenList, string commaOrParen)
        {
            this.writer.Append(parameter).Append(": ");
            switch (tokenList.Count)
            {
                case 0:
                    this.writer.Append("default").AppendLine(commaOrParen);
                    return this;
                case 1:
                    return this.AppendLine($"SyntaxFactory.SingletonTokenList(")
                               .PushIndent()
                               .Write(tokenList[0])
                               .AppendLine(")")
                               .AppendLine(commaOrParen)
                               .PopIndent();
                default:
                    this.AppendLine($"SyntaxFactory.TokenList(")
                        .PushIndent();
                    foreach (var token in tokenList)
                    {
                        this.Write(token).AppendLine(ReferenceEquals(token, tokenList.Last()) ? ")" : ",");
                    }

                    return this.AppendLine(")")
                               .AppendLine(commaOrParen)
                               .PopIndent();
            }
        }

        private SyntaxFactoryWriter AppendLine(string text)
        {
            this.writer.AppendLine(text);
            return this;
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

                this.builder.AppendLine(text);
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
        }
    }
}
