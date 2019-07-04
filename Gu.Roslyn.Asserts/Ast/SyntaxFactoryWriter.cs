namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using Gu.Roslyn.Asserts.Internals;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using Microsoft.CodeAnalysis.CSharp.Syntax;

    /// <summary>
    /// For transforming code into the corresponding SyntaxFactory call.
    /// </summary>
    [Obsolete("WIP not finished yet.")]
    public class SyntaxFactoryWriter
    {
        private readonly Writer writer = new Writer();

        /// <summary>
        /// Transforms the code passed in to a call to SyntaxFactory that creates the same code.
        /// </summary>
        /// <param name="code">For example class C { }. </param>
        /// <returns>SyntaxFactory.Compilation(...)</returns>
        public static string Serialize(string code)
        {
            var compilationUnit = SyntaxFactory.ParseCompilationUnit(code);
            var writer = new SyntaxFactoryWriter().Write(compilationUnit);
            return writer.writer.ToString();
        }

        /// <summary>
        /// Transforms the code passed in to a call to SyntaxFactory that creates the same code.
        /// </summary>
        /// <param name="node">For example class C { }. </param>
        /// <returns>SyntaxFactory.Xxx(...)</returns>
        public static string Serialize(SyntaxNode node)
        {
            return new SyntaxFactoryWriter()
                   .Write(node)
                   .writer
                   .ToString();
        }

        private SyntaxFactoryWriter Write(SyntaxNode node)
        {
            switch (node)
            {
                case AccessorDeclarationSyntax accessorDeclaration:
                    return this.AppendLine("SyntaxFactory.AccessorDeclaration(")
                               .PushIndent()
                               .WriteArgument("kind", accessorDeclaration.Kind())
                               .WriteArgument("attributeLists", accessorDeclaration.AttributeLists)
                               .WriteArgument("modifiers", accessorDeclaration.Modifiers)
                               .WriteArgument("keyword", accessorDeclaration.Keyword)
                               .WriteArgument("body", accessorDeclaration.Body)
                               .WriteArgument("expressionBody", accessorDeclaration.ExpressionBody)
                               .WriteArgument("semicolonToken", accessorDeclaration.SemicolonToken, closeArgumentList: true)
                               .PopIndent();
                case AccessorListSyntax accessorList:
                    return this.AppendLine("SyntaxFactory.AccessorList(")
                               .PushIndent()
                               .WriteArgument("openBraceToken", accessorList.OpenBraceToken)
                               .WriteArgument("accessors", accessorList.Accessors)
                               .WriteArgument("closeBraceToken", accessorList.CloseBraceToken, closeArgumentList: true)
                               .PopIndent();
                case AliasQualifiedNameSyntax aliasQualifiedName:
                    return this.AppendLine("SyntaxFactory.AliasQualifiedName(")
                               .PushIndent()
                               .WriteArgument("alias", aliasQualifiedName.Alias)
                               .WriteArgument("colonColonToken", aliasQualifiedName.ColonColonToken)
                               .WriteArgument("name", aliasQualifiedName.Name, closeArgumentList: true)
                               .PopIndent();
                case AnonymousMethodExpressionSyntax anonymousMethodExpression:
                    return this.AppendLine("SyntaxFactory.AnonymousMethodExpression(")
                               .PushIndent()
                               .WriteArgument("asyncKeyword", anonymousMethodExpression.AsyncKeyword)
                               .WriteArgument("delegateKeyword", anonymousMethodExpression.DelegateKeyword)
                               .WriteArgument("parameterList", anonymousMethodExpression.ParameterList)
                               .WriteArgument("body", anonymousMethodExpression.Body, closeArgumentList: true)
                               .PopIndent();
                case AnonymousObjectCreationExpressionSyntax anonymousObjectCreationExpression:
                    return this.AppendLine("SyntaxFactory.AnonymousObjectCreationExpression(")
                               .PushIndent()
                               .WriteArgument("newKeyword", anonymousObjectCreationExpression.NewKeyword)
                               .WriteArgument("openBraceToken", anonymousObjectCreationExpression.OpenBraceToken)
                               .WriteArgument("initializers", anonymousObjectCreationExpression.Initializers)
                               .WriteArgument("closeBraceToken", anonymousObjectCreationExpression.CloseBraceToken, closeArgumentList: true)
                               .PopIndent();
                case AnonymousObjectMemberDeclaratorSyntax anonymousObjectMemberDeclarator:
                    return this.AppendLine("SyntaxFactory.AnonymousObjectMemberDeclarator(")
                               .PushIndent()
                               .WriteArgument("nameEquals", anonymousObjectMemberDeclarator.NameEquals)
                               .WriteArgument("expression", anonymousObjectMemberDeclarator.Expression, closeArgumentList: true)
                               .PopIndent();
                case ArgumentSyntax argument:
                    return this.AppendLine("SyntaxFactory.Argument(")
                               .PushIndent()
                               .WriteArgument("nameColon", argument.NameColon)
                               .WriteArgument("refKindKeyword", argument.RefKindKeyword)
                               .WriteArgument("expression", argument.Expression, closeArgumentList: true)
                               .PopIndent();
                case ArgumentListSyntax argumentList:
                    return this.AppendLine("SyntaxFactory.ArgumentList(")
                               .PushIndent()
                               .WriteArgument("openParenToken", argumentList.OpenParenToken)
                               .WriteArgument("arguments", argumentList.Arguments)
                               .WriteArgument("closeParenToken", argumentList.CloseParenToken, closeArgumentList: true)
                               .PopIndent();
                case ArrayCreationExpressionSyntax arrayCreationExpression:
                    return this.AppendLine("SyntaxFactory.ArrayCreationExpression(")
                               .PushIndent()
                               .WriteArgument("newKeyword", arrayCreationExpression.NewKeyword)
                               .WriteArgument("type", arrayCreationExpression.Type)
                               .WriteArgument("initializer", arrayCreationExpression.Initializer, closeArgumentList: true)
                               .PopIndent();
                case ArrayRankSpecifierSyntax arrayRankSpecifier:
                    return this.AppendLine("SyntaxFactory.ArrayRankSpecifier(")
                               .PushIndent()
                               .WriteArgument("openBracketToken", arrayRankSpecifier.OpenBracketToken)
                               .WriteArgument("sizes", arrayRankSpecifier.Sizes)
                               .WriteArgument("closeBracketToken", arrayRankSpecifier.CloseBracketToken, closeArgumentList: true)
                               .PopIndent();
                case ArrayTypeSyntax arrayType:
                    return this.AppendLine("SyntaxFactory.ArrayType(")
                               .PushIndent()
                               .WriteArgument("elementType", arrayType.ElementType)
                               .WriteArgument("rankSpecifiers", arrayType.RankSpecifiers, closeArgumentList: true)
                               .PopIndent();
                case ArrowExpressionClauseSyntax arrowExpressionClause:
                    return this.AppendLine("SyntaxFactory.ArrowExpressionClause(")
                               .PushIndent()
                               .WriteArgument("arrowToken", arrowExpressionClause.ArrowToken)
                               .WriteArgument("expression", arrowExpressionClause.Expression, closeArgumentList: true)
                               .PopIndent();
                case AssignmentExpressionSyntax assignmentExpression:
                    return this.AppendLine("SyntaxFactory.AssignmentExpression(")
                               .PushIndent()
                               .WriteArgument("kind", assignmentExpression.Kind())
                               .WriteArgument("left", assignmentExpression.Left)
                               .WriteArgument("operatorToken", assignmentExpression.OperatorToken)
                               .WriteArgument("right", assignmentExpression.Right, closeArgumentList: true)
                               .PopIndent();
                case AttributeSyntax attribute:
                    return this.AppendLine("SyntaxFactory.Attribute(")
                               .PushIndent()
                               .WriteArgument("name", attribute.Name)
                               .WriteArgument("argumentList", attribute.ArgumentList, closeArgumentList: true)
                               .PopIndent();
                case AttributeArgumentSyntax attributeArgument:
                    return this.AppendLine("SyntaxFactory.AttributeArgument(")
                               .PushIndent()
                               .WriteArgument("nameEquals", attributeArgument.NameEquals)
                               .WriteArgument("nameColon", attributeArgument.NameColon)
                               .WriteArgument("expression", attributeArgument.Expression, closeArgumentList: true)
                               .PopIndent();
                case AttributeArgumentListSyntax attributeArgumentList:
                    return this.AppendLine("SyntaxFactory.AttributeArgumentList(")
                               .PushIndent()
                               .WriteArgument("openParenToken", attributeArgumentList.OpenParenToken)
                               .WriteArgument("arguments", attributeArgumentList.Arguments)
                               .WriteArgument("closeParenToken", attributeArgumentList.CloseParenToken, closeArgumentList: true)
                               .PopIndent();
                case AttributeListSyntax attributeList:
                    return this.AppendLine("SyntaxFactory.AttributeList(")
                               .PushIndent()
                               .WriteArgument("openBracketToken", attributeList.OpenBracketToken)
                               .WriteArgument("target", attributeList.Target)
                               .WriteArgument("attributes", attributeList.Attributes)
                               .WriteArgument("closeBracketToken", attributeList.CloseBracketToken, closeArgumentList: true)
                               .PopIndent();
                case AttributeTargetSpecifierSyntax attributeTargetSpecifier:
                    return this.AppendLine("SyntaxFactory.AttributeTargetSpecifier(")
                               .PushIndent()
                               .WriteArgument("identifier", attributeTargetSpecifier.Identifier)
                               .WriteArgument("colonToken", attributeTargetSpecifier.ColonToken, closeArgumentList: true)
                               .PopIndent();
                case AwaitExpressionSyntax awaitExpression:
                    return this.AppendLine("SyntaxFactory.AwaitExpression(")
                               .PushIndent()
                               .WriteArgument("awaitKeyword", awaitExpression.AwaitKeyword)
                               .WriteArgument("expression", awaitExpression.Expression, closeArgumentList: true)
                               .PopIndent();
                case BadDirectiveTriviaSyntax badDirectiveTrivia:
                    return this.AppendLine("SyntaxFactory.BadDirectiveTrivia(")
                               .PushIndent()
                               .WriteArgument("hashToken", badDirectiveTrivia.HashToken)
                               .WriteArgument("identifier", badDirectiveTrivia.Identifier)
                               .WriteArgument("endOfDirectiveToken", badDirectiveTrivia.EndOfDirectiveToken)
                               .WriteArgument("isActive", badDirectiveTrivia.IsActive, closeArgumentList: true)
                               .PopIndent();
                case BaseExpressionSyntax baseExpression:
                    return this.AppendLine("SyntaxFactory.BaseExpression(")
                               .PushIndent()
                               .WriteArgument("token", baseExpression.Token)
                               //.WriteArgument("typeClause", baseExpression.TypeClause, closeArgumentList: true)
                               .PopIndent();
                //case BaseExpressionTypeClauseSyntax baseExpressionTypeClause:
                //    return this.AppendLine("SyntaxFactory.BaseExpressionTypeClause(")
                //               .PushIndent()
                //               .WriteArgument("openParenToken", baseExpressionTypeClause.OpenParenToken)
                //               .WriteArgument("baseType", baseExpressionTypeClause.BaseType)
                //               .WriteArgument("closeParenToken", baseExpressionTypeClause.CloseParenToken, closeArgumentList: true)
                //               .PopIndent();
                case BaseListSyntax baseList:
                    return this.AppendLine("SyntaxFactory.BaseList(")
                               .PushIndent()
                               .WriteArgument("colonToken", baseList.ColonToken)
                               .WriteArgument("types", baseList.Types, closeArgumentList: true)
                               .PopIndent();
                case BinaryExpressionSyntax binaryExpression:
                    return this.AppendLine("SyntaxFactory.BinaryExpression(")
                               .PushIndent()
                               .WriteArgument("kind", binaryExpression.Kind())
                               .WriteArgument("left", binaryExpression.Left)
                               .WriteArgument("operatorToken", binaryExpression.OperatorToken)
                               .WriteArgument("right", binaryExpression.Right, closeArgumentList: true)
                               .PopIndent();
                case BlockSyntax block:
                    return this.AppendLine("SyntaxFactory.Block(")
                               .PushIndent()
                               .WriteArgument("openBraceToken", block.OpenBraceToken)
                               .WriteArgument("statements", block.Statements)
                               .WriteArgument("closeBraceToken", block.CloseBraceToken, closeArgumentList: true)
                               .PopIndent();
                case BracketedArgumentListSyntax bracketedArgumentList:
                    return this.AppendLine("SyntaxFactory.BracketedArgumentList(")
                               .PushIndent()
                               .WriteArgument("openBracketToken", bracketedArgumentList.OpenBracketToken)
                               .WriteArgument("arguments", bracketedArgumentList.Arguments)
                               .WriteArgument("closeBracketToken", bracketedArgumentList.CloseBracketToken, closeArgumentList: true)
                               .PopIndent();
                case BracketedParameterListSyntax bracketedParameterList:
                    return this.AppendLine("SyntaxFactory.BracketedParameterList(")
                               .PushIndent()
                               .WriteArgument("openBracketToken", bracketedParameterList.OpenBracketToken)
                               .WriteArgument("parameters", bracketedParameterList.Parameters)
                               .WriteArgument("closeBracketToken", bracketedParameterList.CloseBracketToken, closeArgumentList: true)
                               .PopIndent();
                case BreakStatementSyntax breakStatement:
                    return this.AppendLine("SyntaxFactory.BreakStatement(")
                               .PushIndent()
                               .WriteArgument("breakKeyword", breakStatement.BreakKeyword)
                               .WriteArgument("semicolonToken", breakStatement.SemicolonToken, closeArgumentList: true)
                               .PopIndent();
                case CasePatternSwitchLabelSyntax casePatternSwitchLabel:
                    return this.AppendLine("SyntaxFactory.CasePatternSwitchLabel(")
                               .PushIndent()
                               .WriteArgument("keyword", casePatternSwitchLabel.Keyword)
                               .WriteArgument("pattern", casePatternSwitchLabel.Pattern)
                               .WriteArgument("whenClause", casePatternSwitchLabel.WhenClause)
                               .WriteArgument("colonToken", casePatternSwitchLabel.ColonToken, closeArgumentList: true)
                               .PopIndent();
                case CaseSwitchLabelSyntax caseSwitchLabel:
                    return this.AppendLine("SyntaxFactory.CaseSwitchLabel(")
                               .PushIndent()
                               .WriteArgument("keyword", caseSwitchLabel.Keyword)
                               .WriteArgument("value", caseSwitchLabel.Value)
                               .WriteArgument("colonToken", caseSwitchLabel.ColonToken, closeArgumentList: true)
                               .PopIndent();
                case CastExpressionSyntax castExpression:
                    return this.AppendLine("SyntaxFactory.CastExpression(")
                               .PushIndent()
                               .WriteArgument("openParenToken", castExpression.OpenParenToken)
                               .WriteArgument("type", castExpression.Type)
                               .WriteArgument("closeParenToken", castExpression.CloseParenToken)
                               .WriteArgument("expression", castExpression.Expression, closeArgumentList: true)
                               .PopIndent();
                case CatchClauseSyntax catchClause:
                    return this.AppendLine("SyntaxFactory.CatchClause(")
                               .PushIndent()
                               .WriteArgument("catchKeyword", catchClause.CatchKeyword)
                               .WriteArgument("declaration", catchClause.Declaration)
                               .WriteArgument("filter", catchClause.Filter)
                               .WriteArgument("block", catchClause.Block, closeArgumentList: true)
                               .PopIndent();
                case CatchDeclarationSyntax catchDeclaration:
                    return this.AppendLine("SyntaxFactory.CatchDeclaration(")
                               .PushIndent()
                               .WriteArgument("openParenToken", catchDeclaration.OpenParenToken)
                               .WriteArgument("type", catchDeclaration.Type)
                               .WriteArgument("identifier", catchDeclaration.Identifier)
                               .WriteArgument("closeParenToken", catchDeclaration.CloseParenToken, closeArgumentList: true)
                               .PopIndent();
                case CatchFilterClauseSyntax catchFilterClause:
                    return this.AppendLine("SyntaxFactory.CatchFilterClause(")
                               .PushIndent()
                               .WriteArgument("whenKeyword", catchFilterClause.WhenKeyword)
                               .WriteArgument("openParenToken", catchFilterClause.OpenParenToken)
                               .WriteArgument("filterExpression", catchFilterClause.FilterExpression)
                               .WriteArgument("closeParenToken", catchFilterClause.CloseParenToken, closeArgumentList: true)
                               .PopIndent();
                case CheckedExpressionSyntax checkedExpression:
                    return this.AppendLine("SyntaxFactory.CheckedExpression(")
                               .PushIndent()
                               .WriteArgument("kind", checkedExpression.Kind())
                               .WriteArgument("keyword", checkedExpression.Keyword)
                               .WriteArgument("openParenToken", checkedExpression.OpenParenToken)
                               .WriteArgument("expression", checkedExpression.Expression)
                               .WriteArgument("closeParenToken", checkedExpression.CloseParenToken, closeArgumentList: true)
                               .PopIndent();
                case CheckedStatementSyntax checkedStatement:
                    return this.AppendLine("SyntaxFactory.CheckedStatement(")
                               .PushIndent()
                               .WriteArgument("kind", checkedStatement.Kind())
                               .WriteArgument("keyword", checkedStatement.Keyword)
                               .WriteArgument("block", checkedStatement.Block, closeArgumentList: true)
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
                case ClassOrStructConstraintSyntax classOrStructConstraint:
                    return this.AppendLine("SyntaxFactory.ClassOrStructConstraint(")
                               .PushIndent()
                               .WriteArgument("kind", classOrStructConstraint.Kind())
                               .WriteArgument("classOrStructKeyword", classOrStructConstraint.ClassOrStructKeyword)
                               .WriteArgument("questionToken", classOrStructConstraint.QuestionToken, closeArgumentList: true)
                               .PopIndent();
                case CompilationUnitSyntax compilationUnit:
                    return this.AppendLine("SyntaxFactory.CompilationUnit(")
                               .PushIndent()
                               .WriteArgument("externs", compilationUnit.Externs)
                               .WriteArgument("usings", compilationUnit.Usings)
                               .WriteArgument("attributeLists", compilationUnit.AttributeLists)
                               .WriteArgument("members", compilationUnit.Members)
                               .WriteArgument("endOfFileToken", compilationUnit.EndOfFileToken, closeArgumentList: true)
                               .PopIndent();
                case ConditionalAccessExpressionSyntax conditionalAccessExpression:
                    return this.AppendLine("SyntaxFactory.ConditionalAccessExpression(")
                               .PushIndent()
                               .WriteArgument("expression", conditionalAccessExpression.Expression)
                               .WriteArgument("operatorToken", conditionalAccessExpression.OperatorToken)
                               .WriteArgument("whenNotNull", conditionalAccessExpression.WhenNotNull, closeArgumentList: true)
                               .PopIndent();
                case ConditionalExpressionSyntax conditionalExpression:
                    return this.AppendLine("SyntaxFactory.ConditionalExpression(")
                               .PushIndent()
                               .WriteArgument("condition", conditionalExpression.Condition)
                               .WriteArgument("questionToken", conditionalExpression.QuestionToken)
                               .WriteArgument("whenTrue", conditionalExpression.WhenTrue)
                               .WriteArgument("colonToken", conditionalExpression.ColonToken)
                               .WriteArgument("whenFalse", conditionalExpression.WhenFalse, closeArgumentList: true)
                               .PopIndent();
                case ConstantPatternSyntax constantPattern:
                    return this.AppendLine("SyntaxFactory.ConstantPattern(")
                               .PushIndent()
                               .WriteArgument("expression", constantPattern.Expression, closeArgumentList: true)
                               .PopIndent();
                case ConstructorConstraintSyntax constructorConstraint:
                    return this.AppendLine("SyntaxFactory.ConstructorConstraint(")
                               .PushIndent()
                               .WriteArgument("newKeyword", constructorConstraint.NewKeyword)
                               .WriteArgument("openParenToken", constructorConstraint.OpenParenToken)
                               .WriteArgument("closeParenToken", constructorConstraint.CloseParenToken, closeArgumentList: true)
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
                case ConstructorInitializerSyntax constructorInitializer:
                    return this.AppendLine("SyntaxFactory.ConstructorInitializer(")
                               .PushIndent()
                               .WriteArgument("kind", constructorInitializer.Kind())
                               .WriteArgument("colonToken", constructorInitializer.ColonToken)
                               .WriteArgument("thisOrBaseKeyword", constructorInitializer.ThisOrBaseKeyword)
                               .WriteArgument("argumentList", constructorInitializer.ArgumentList, closeArgumentList: true)
                               .PopIndent();
                case ContinueStatementSyntax continueStatement:
                    return this.AppendLine("SyntaxFactory.ContinueStatement(")
                               .PushIndent()
                               .WriteArgument("continueKeyword", continueStatement.ContinueKeyword)
                               .WriteArgument("semicolonToken", continueStatement.SemicolonToken, closeArgumentList: true)
                               .PopIndent();
                case ConversionOperatorDeclarationSyntax conversionOperatorDeclaration:
                    return this.AppendLine("SyntaxFactory.ConversionOperatorDeclaration(")
                               .PushIndent()
                               .WriteArgument("attributeLists", conversionOperatorDeclaration.AttributeLists)
                               .WriteArgument("modifiers", conversionOperatorDeclaration.Modifiers)
                               .WriteArgument("implicitOrExplicitKeyword", conversionOperatorDeclaration.ImplicitOrExplicitKeyword)
                               .WriteArgument("operatorKeyword", conversionOperatorDeclaration.OperatorKeyword)
                               .WriteArgument("type", conversionOperatorDeclaration.Type)
                               .WriteArgument("parameterList", conversionOperatorDeclaration.ParameterList)
                               .WriteArgument("body", conversionOperatorDeclaration.Body)
                               .WriteArgument("expressionBody", conversionOperatorDeclaration.ExpressionBody)
                               .WriteArgument("semicolonToken", conversionOperatorDeclaration.SemicolonToken, closeArgumentList: true)
                               .PopIndent();
                case ConversionOperatorMemberCrefSyntax conversionOperatorMemberCref:
                    return this.AppendLine("SyntaxFactory.ConversionOperatorMemberCref(")
                               .PushIndent()
                               .WriteArgument("implicitOrExplicitKeyword", conversionOperatorMemberCref.ImplicitOrExplicitKeyword)
                               .WriteArgument("operatorKeyword", conversionOperatorMemberCref.OperatorKeyword)
                               .WriteArgument("type", conversionOperatorMemberCref.Type)
                               .WriteArgument("parameters", conversionOperatorMemberCref.Parameters, closeArgumentList: true)
                               .PopIndent();
                case CrefBracketedParameterListSyntax crefBracketedParameterList:
                    return this.AppendLine("SyntaxFactory.CrefBracketedParameterList(")
                               .PushIndent()
                               .WriteArgument("openBracketToken", crefBracketedParameterList.OpenBracketToken)
                               .WriteArgument("parameters", crefBracketedParameterList.Parameters)
                               .WriteArgument("closeBracketToken", crefBracketedParameterList.CloseBracketToken, closeArgumentList: true)
                               .PopIndent();
                case CrefParameterSyntax crefParameter:
                    return this.AppendLine("SyntaxFactory.CrefParameter(")
                               .PushIndent()
                               .WriteArgument("refKindKeyword", crefParameter.RefKindKeyword)
                               .WriteArgument("type", crefParameter.Type, closeArgumentList: true)
                               .PopIndent();
                case CrefParameterListSyntax crefParameterList:
                    return this.AppendLine("SyntaxFactory.CrefParameterList(")
                               .PushIndent()
                               .WriteArgument("openParenToken", crefParameterList.OpenParenToken)
                               .WriteArgument("parameters", crefParameterList.Parameters)
                               .WriteArgument("closeParenToken", crefParameterList.CloseParenToken, closeArgumentList: true)
                               .PopIndent();
                case DeclarationExpressionSyntax declarationExpression:
                    return this.AppendLine("SyntaxFactory.DeclarationExpression(")
                               .PushIndent()
                               .WriteArgument("type", declarationExpression.Type)
                               .WriteArgument("designation", declarationExpression.Designation, closeArgumentList: true)
                               .PopIndent();
                case DeclarationPatternSyntax declarationPattern:
                    return this.AppendLine("SyntaxFactory.DeclarationPattern(")
                               .PushIndent()
                               .WriteArgument("type", declarationPattern.Type)
                               .WriteArgument("designation", declarationPattern.Designation, closeArgumentList: true)
                               .PopIndent();
                case DefaultExpressionSyntax defaultExpression:
                    return this.AppendLine("SyntaxFactory.DefaultExpression(")
                               .PushIndent()
                               .WriteArgument("keyword", defaultExpression.Keyword)
                               .WriteArgument("openParenToken", defaultExpression.OpenParenToken)
                               .WriteArgument("type", defaultExpression.Type)
                               .WriteArgument("closeParenToken", defaultExpression.CloseParenToken, closeArgumentList: true)
                               .PopIndent();
                case DefaultSwitchLabelSyntax defaultSwitchLabel:
                    return this.AppendLine("SyntaxFactory.DefaultSwitchLabel(")
                               .PushIndent()
                               .WriteArgument("keyword", defaultSwitchLabel.Keyword)
                               .WriteArgument("colonToken", defaultSwitchLabel.ColonToken, closeArgumentList: true)
                               .PopIndent();
                case DefineDirectiveTriviaSyntax defineDirectiveTrivia:
                    return this.AppendLine("SyntaxFactory.DefineDirectiveTrivia(")
                               .PushIndent()
                               .WriteArgument("hashToken", defineDirectiveTrivia.HashToken)
                               .WriteArgument("defineKeyword", defineDirectiveTrivia.DefineKeyword)
                               .WriteArgument("name", defineDirectiveTrivia.Name)
                               .WriteArgument("endOfDirectiveToken", defineDirectiveTrivia.EndOfDirectiveToken)
                               .WriteArgument("isActive", defineDirectiveTrivia.IsActive, closeArgumentList: true)
                               .PopIndent();
                case DelegateDeclarationSyntax delegateDeclaration:
                    return this.AppendLine("SyntaxFactory.DelegateDeclaration(")
                               .PushIndent()
                               .WriteArgument("attributeLists", delegateDeclaration.AttributeLists)
                               .WriteArgument("modifiers", delegateDeclaration.Modifiers)
                               .WriteArgument("delegateKeyword", delegateDeclaration.DelegateKeyword)
                               .WriteArgument("returnType", delegateDeclaration.ReturnType)
                               .WriteArgument("identifier", delegateDeclaration.Identifier)
                               .WriteArgument("typeParameterList", delegateDeclaration.TypeParameterList)
                               .WriteArgument("parameterList", delegateDeclaration.ParameterList)
                               .WriteArgument("constraintClauses", delegateDeclaration.ConstraintClauses)
                               .WriteArgument("semicolonToken", delegateDeclaration.SemicolonToken, closeArgumentList: true)
                               .PopIndent();
                case DestructorDeclarationSyntax destructorDeclaration:
                    return this.AppendLine("SyntaxFactory.DestructorDeclaration(")
                               .PushIndent()
                               .WriteArgument("attributeLists", destructorDeclaration.AttributeLists)
                               .WriteArgument("modifiers", destructorDeclaration.Modifiers)
                               .WriteArgument("tildeToken", destructorDeclaration.TildeToken)
                               .WriteArgument("identifier", destructorDeclaration.Identifier)
                               .WriteArgument("parameterList", destructorDeclaration.ParameterList)
                               .WriteArgument("body", destructorDeclaration.Body)
                               .WriteArgument("expressionBody", destructorDeclaration.ExpressionBody)
                               .WriteArgument("semicolonToken", destructorDeclaration.SemicolonToken, closeArgumentList: true)
                               .PopIndent();
                case DiscardDesignationSyntax discardDesignation:
                    return this.AppendLine("SyntaxFactory.DiscardDesignation(")
                               .PushIndent()
                               .WriteArgument("underscoreToken", discardDesignation.UnderscoreToken, closeArgumentList: true)
                               .PopIndent();
                case DiscardPatternSyntax discardPattern:
                    return this.AppendLine("SyntaxFactory.DiscardPattern(")
                               .PushIndent()
                               .WriteArgument("underscoreToken", discardPattern.UnderscoreToken, closeArgumentList: true)
                               .PopIndent();
                case DocumentationCommentTriviaSyntax documentationCommentTrivia:
                    return this.AppendLine("SyntaxFactory.DocumentationCommentTrivia(")
                               .PushIndent()
                               .WriteArgument("kind", documentationCommentTrivia.Kind())
                               .WriteArgument("content", documentationCommentTrivia.Content)
                               .WriteArgument("endOfComment", documentationCommentTrivia.EndOfComment, closeArgumentList: true)
                               .PopIndent();
                case DoStatementSyntax doStatement:
                    return this.AppendLine("SyntaxFactory.DoStatement(")
                               .PushIndent()
                               .WriteArgument("doKeyword", doStatement.DoKeyword)
                               .WriteArgument("statement", doStatement.Statement)
                               .WriteArgument("whileKeyword", doStatement.WhileKeyword)
                               .WriteArgument("openParenToken", doStatement.OpenParenToken)
                               .WriteArgument("condition", doStatement.Condition)
                               .WriteArgument("closeParenToken", doStatement.CloseParenToken)
                               .WriteArgument("semicolonToken", doStatement.SemicolonToken, closeArgumentList: true)
                               .PopIndent();
                case ElementAccessExpressionSyntax elementAccessExpression:
                    return this.AppendLine("SyntaxFactory.ElementAccessExpression(")
                               .PushIndent()
                               .WriteArgument("expression", elementAccessExpression.Expression)
                               .WriteArgument("argumentList", elementAccessExpression.ArgumentList, closeArgumentList: true)
                               .PopIndent();
                case ElementBindingExpressionSyntax elementBindingExpression:
                    return this.AppendLine("SyntaxFactory.ElementBindingExpression(")
                               .PushIndent()
                               .WriteArgument("argumentList", elementBindingExpression.ArgumentList, closeArgumentList: true)
                               .PopIndent();
                case ElifDirectiveTriviaSyntax elifDirectiveTrivia:
                    return this.AppendLine("SyntaxFactory.ElifDirectiveTrivia(")
                               .PushIndent()
                               .WriteArgument("hashToken", elifDirectiveTrivia.HashToken)
                               .WriteArgument("elifKeyword", elifDirectiveTrivia.ElifKeyword)
                               .WriteArgument("condition", elifDirectiveTrivia.Condition)
                               .WriteArgument("endOfDirectiveToken", elifDirectiveTrivia.EndOfDirectiveToken)
                               .WriteArgument("isActive", elifDirectiveTrivia.IsActive)
                               .WriteArgument("branchTaken", elifDirectiveTrivia.BranchTaken)
                               .WriteArgument("conditionValue", elifDirectiveTrivia.ConditionValue, closeArgumentList: true)
                               .PopIndent();
                case ElseClauseSyntax elseClause:
                    return this.AppendLine("SyntaxFactory.ElseClause(")
                               .PushIndent()
                               .WriteArgument("elseKeyword", elseClause.ElseKeyword)
                               .WriteArgument("statement", elseClause.Statement, closeArgumentList: true)
                               .PopIndent();
                case ElseDirectiveTriviaSyntax elseDirectiveTrivia:
                    return this.AppendLine("SyntaxFactory.ElseDirectiveTrivia(")
                               .PushIndent()
                               .WriteArgument("hashToken", elseDirectiveTrivia.HashToken)
                               .WriteArgument("elseKeyword", elseDirectiveTrivia.ElseKeyword)
                               .WriteArgument("endOfDirectiveToken", elseDirectiveTrivia.EndOfDirectiveToken)
                               .WriteArgument("isActive", elseDirectiveTrivia.IsActive)
                               .WriteArgument("branchTaken", elseDirectiveTrivia.BranchTaken, closeArgumentList: true)
                               .PopIndent();
                case EmptyStatementSyntax emptyStatement:
                    return this.AppendLine("SyntaxFactory.EmptyStatement(")
                               .PushIndent()
                               .WriteArgument("semicolonToken", emptyStatement.SemicolonToken, closeArgumentList: true)
                               .PopIndent();
                case EndIfDirectiveTriviaSyntax endIfDirectiveTrivia:
                    return this.AppendLine("SyntaxFactory.EndIfDirectiveTrivia(")
                               .PushIndent()
                               .WriteArgument("hashToken", endIfDirectiveTrivia.HashToken)
                               .WriteArgument("endIfKeyword", endIfDirectiveTrivia.EndIfKeyword)
                               .WriteArgument("endOfDirectiveToken", endIfDirectiveTrivia.EndOfDirectiveToken)
                               .WriteArgument("isActive", endIfDirectiveTrivia.IsActive, closeArgumentList: true)
                               .PopIndent();
                case EndRegionDirectiveTriviaSyntax endRegionDirectiveTrivia:
                    return this.AppendLine("SyntaxFactory.EndRegionDirectiveTrivia(")
                               .PushIndent()
                               .WriteArgument("hashToken", endRegionDirectiveTrivia.HashToken)
                               .WriteArgument("endRegionKeyword", endRegionDirectiveTrivia.EndRegionKeyword)
                               .WriteArgument("endOfDirectiveToken", endRegionDirectiveTrivia.EndOfDirectiveToken)
                               .WriteArgument("isActive", endRegionDirectiveTrivia.IsActive, closeArgumentList: true)
                               .PopIndent();
                case EnumDeclarationSyntax enumDeclaration:
                    return this.AppendLine("SyntaxFactory.EnumDeclaration(")
                               .PushIndent()
                               .WriteArgument("attributeLists", enumDeclaration.AttributeLists)
                               .WriteArgument("modifiers", enumDeclaration.Modifiers)
                               .WriteArgument("enumKeyword", enumDeclaration.EnumKeyword)
                               .WriteArgument("identifier", enumDeclaration.Identifier)
                               .WriteArgument("baseList", enumDeclaration.BaseList)
                               .WriteArgument("openBraceToken", enumDeclaration.OpenBraceToken)
                               .WriteArgument("members", enumDeclaration.Members)
                               .WriteArgument("closeBraceToken", enumDeclaration.CloseBraceToken)
                               .WriteArgument("semicolonToken", enumDeclaration.SemicolonToken, closeArgumentList: true)
                               .PopIndent();
                case EnumMemberDeclarationSyntax enumMemberDeclaration:
                    return this.AppendLine("SyntaxFactory.EnumMemberDeclaration(")
                               .PushIndent()
                               .WriteArgument("attributeLists", enumMemberDeclaration.AttributeLists)
                               .WriteArgument("identifier", enumMemberDeclaration.Identifier)
                               .WriteArgument("equalsValue", enumMemberDeclaration.EqualsValue, closeArgumentList: true)
                               .PopIndent();
                case EqualsValueClauseSyntax equalsValueClause:
                    return this.AppendLine("SyntaxFactory.EqualsValueClause(")
                               .PushIndent()
                               .WriteArgument("equalsToken", equalsValueClause.EqualsToken)
                               .WriteArgument("value", equalsValueClause.Value, closeArgumentList: true)
                               .PopIndent();
                case ErrorDirectiveTriviaSyntax errorDirectiveTrivia:
                    return this.AppendLine("SyntaxFactory.ErrorDirectiveTrivia(")
                               .PushIndent()
                               .WriteArgument("hashToken", errorDirectiveTrivia.HashToken)
                               .WriteArgument("errorKeyword", errorDirectiveTrivia.ErrorKeyword)
                               .WriteArgument("endOfDirectiveToken", errorDirectiveTrivia.EndOfDirectiveToken)
                               .WriteArgument("isActive", errorDirectiveTrivia.IsActive, closeArgumentList: true)
                               .PopIndent();
                case EventDeclarationSyntax eventDeclaration:
                    return this.AppendLine("SyntaxFactory.EventDeclaration(")
                               .PushIndent()
                               .WriteArgument("attributeLists", eventDeclaration.AttributeLists)
                               .WriteArgument("modifiers", eventDeclaration.Modifiers)
                               .WriteArgument("eventKeyword", eventDeclaration.EventKeyword)
                               .WriteArgument("type", eventDeclaration.Type)
                               .WriteArgument("explicitInterfaceSpecifier", eventDeclaration.ExplicitInterfaceSpecifier)
                               .WriteArgument("identifier", eventDeclaration.Identifier)
                               .WriteArgument("accessorList", eventDeclaration.AccessorList, closeArgumentList: true)
                               .PopIndent();
                case EventFieldDeclarationSyntax eventFieldDeclaration:
                    return this.AppendLine("SyntaxFactory.EventFieldDeclaration(")
                               .PushIndent()
                               .WriteArgument("attributeLists", eventFieldDeclaration.AttributeLists)
                               .WriteArgument("modifiers", eventFieldDeclaration.Modifiers)
                               .WriteArgument("eventKeyword", eventFieldDeclaration.EventKeyword)
                               .WriteArgument("declaration", eventFieldDeclaration.Declaration)
                               .WriteArgument("semicolonToken", eventFieldDeclaration.SemicolonToken, closeArgumentList: true)
                               .PopIndent();
                case ExplicitInterfaceSpecifierSyntax explicitInterfaceSpecifier:
                    return this.AppendLine("SyntaxFactory.ExplicitInterfaceSpecifier(")
                               .PushIndent()
                               .WriteArgument("name", explicitInterfaceSpecifier.Name)
                               .WriteArgument("dotToken", explicitInterfaceSpecifier.DotToken, closeArgumentList: true)
                               .PopIndent();
                case ExpressionStatementSyntax expressionStatement:
                    return this.AppendLine("SyntaxFactory.ExpressionStatement(")
                               .PushIndent()
                               .WriteArgument("expression", expressionStatement.Expression)
                               .WriteArgument("semicolonToken", expressionStatement.SemicolonToken, closeArgumentList: true)
                               .PopIndent();
                case ExternAliasDirectiveSyntax externAliasDirective:
                    return this.AppendLine("SyntaxFactory.ExternAliasDirective(")
                               .PushIndent()
                               .WriteArgument("externKeyword", externAliasDirective.ExternKeyword)
                               .WriteArgument("aliasKeyword", externAliasDirective.AliasKeyword)
                               .WriteArgument("identifier", externAliasDirective.Identifier)
                               .WriteArgument("semicolonToken", externAliasDirective.SemicolonToken, closeArgumentList: true)
                               .PopIndent();
                case FieldDeclarationSyntax fieldDeclaration:
                    return this.AppendLine("SyntaxFactory.FieldDeclaration(")
                               .PushIndent()
                               .WriteArgument("attributeLists", fieldDeclaration.AttributeLists)
                               .WriteArgument("modifiers", fieldDeclaration.Modifiers)
                               .WriteArgument("declaration", fieldDeclaration.Declaration)
                               .WriteArgument("semicolonToken", fieldDeclaration.SemicolonToken, closeArgumentList: true)
                               .PopIndent();
                case FinallyClauseSyntax finallyClause:
                    return this.AppendLine("SyntaxFactory.FinallyClause(")
                               .PushIndent()
                               .WriteArgument("finallyKeyword", finallyClause.FinallyKeyword)
                               .WriteArgument("block", finallyClause.Block, closeArgumentList: true)
                               .PopIndent();
                case FixedStatementSyntax fixedStatement:
                    return this.AppendLine("SyntaxFactory.FixedStatement(")
                               .PushIndent()
                               .WriteArgument("fixedKeyword", fixedStatement.FixedKeyword)
                               .WriteArgument("openParenToken", fixedStatement.OpenParenToken)
                               .WriteArgument("declaration", fixedStatement.Declaration)
                               .WriteArgument("closeParenToken", fixedStatement.CloseParenToken)
                               .WriteArgument("statement", fixedStatement.Statement, closeArgumentList: true)
                               .PopIndent();
                case ForEachStatementSyntax forEachStatement:
                    return this.AppendLine("SyntaxFactory.ForEachStatement(")
                               .PushIndent()
                               .WriteArgument("awaitKeyword", forEachStatement.AwaitKeyword)
                               .WriteArgument("forEachKeyword", forEachStatement.ForEachKeyword)
                               .WriteArgument("openParenToken", forEachStatement.OpenParenToken)
                               .WriteArgument("type", forEachStatement.Type)
                               .WriteArgument("identifier", forEachStatement.Identifier)
                               .WriteArgument("inKeyword", forEachStatement.InKeyword)
                               .WriteArgument("expression", forEachStatement.Expression)
                               .WriteArgument("closeParenToken", forEachStatement.CloseParenToken)
                               .WriteArgument("statement", forEachStatement.Statement, closeArgumentList: true)
                               .PopIndent();
                case ForEachVariableStatementSyntax forEachVariableStatement:
                    return this.AppendLine("SyntaxFactory.ForEachVariableStatement(")
                               .PushIndent()
                               .WriteArgument("awaitKeyword", forEachVariableStatement.AwaitKeyword)
                               .WriteArgument("forEachKeyword", forEachVariableStatement.ForEachKeyword)
                               .WriteArgument("openParenToken", forEachVariableStatement.OpenParenToken)
                               .WriteArgument("variable", forEachVariableStatement.Variable)
                               .WriteArgument("inKeyword", forEachVariableStatement.InKeyword)
                               .WriteArgument("expression", forEachVariableStatement.Expression)
                               .WriteArgument("closeParenToken", forEachVariableStatement.CloseParenToken)
                               .WriteArgument("statement", forEachVariableStatement.Statement, closeArgumentList: true)
                               .PopIndent();
                case ForStatementSyntax forStatement:
                    return this.AppendLine("SyntaxFactory.ForStatement(")
                               .PushIndent()
                               .WriteArgument("forKeyword", forStatement.ForKeyword)
                               .WriteArgument("openParenToken", forStatement.OpenParenToken)
                               .WriteArgument("declaration", forStatement.Declaration)
                               .WriteArgument("initializers", forStatement.Initializers)
                               .WriteArgument("firstSemicolonToken", forStatement.FirstSemicolonToken)
                               .WriteArgument("condition", forStatement.Condition)
                               .WriteArgument("secondSemicolonToken", forStatement.SecondSemicolonToken)
                               .WriteArgument("incrementors", forStatement.Incrementors)
                               .WriteArgument("closeParenToken", forStatement.CloseParenToken)
                               .WriteArgument("statement", forStatement.Statement, closeArgumentList: true)
                               .PopIndent();
                case FromClauseSyntax fromClause:
                    return this.AppendLine("SyntaxFactory.FromClause(")
                               .PushIndent()
                               .WriteArgument("fromKeyword", fromClause.FromKeyword)
                               .WriteArgument("type", fromClause.Type)
                               .WriteArgument("identifier", fromClause.Identifier)
                               .WriteArgument("inKeyword", fromClause.InKeyword)
                               .WriteArgument("expression", fromClause.Expression, closeArgumentList: true)
                               .PopIndent();
                case GenericNameSyntax genericName:
                    return this.AppendLine("SyntaxFactory.GenericName(")
                               .PushIndent()
                               .WriteArgument("identifier", genericName.Identifier)
                               .WriteArgument("typeArgumentList", genericName.TypeArgumentList, closeArgumentList: true)
                               .PopIndent();
                case GlobalStatementSyntax globalStatement:
                    return this.AppendLine("SyntaxFactory.GlobalStatement(")
                               .PushIndent()
                               .WriteArgument("statement", globalStatement.Statement, closeArgumentList: true)
                               .PopIndent();
                case GotoStatementSyntax gotoStatement:
                    return this.AppendLine("SyntaxFactory.GotoStatement(")
                               .PushIndent()
                               .WriteArgument("kind", gotoStatement.Kind())
                               .WriteArgument("gotoKeyword", gotoStatement.GotoKeyword)
                               .WriteArgument("caseOrDefaultKeyword", gotoStatement.CaseOrDefaultKeyword)
                               .WriteArgument("expression", gotoStatement.Expression)
                               .WriteArgument("semicolonToken", gotoStatement.SemicolonToken, closeArgumentList: true)
                               .PopIndent();
                case GroupClauseSyntax groupClause:
                    return this.AppendLine("SyntaxFactory.GroupClause(")
                               .PushIndent()
                               .WriteArgument("groupKeyword", groupClause.GroupKeyword)
                               .WriteArgument("groupExpression", groupClause.GroupExpression)
                               .WriteArgument("byKeyword", groupClause.ByKeyword)
                               .WriteArgument("byExpression", groupClause.ByExpression, closeArgumentList: true)
                               .PopIndent();
                case IdentifierNameSyntax identifierName:
                    return this.AppendLine("SyntaxFactory.IdentifierName(")
                               .PushIndent()
                               .WriteArgument("identifier", identifierName.Identifier, closeArgumentList: true)
                               .PopIndent();
                case IfDirectiveTriviaSyntax ifDirectiveTrivia:
                    return this.AppendLine("SyntaxFactory.IfDirectiveTrivia(")
                               .PushIndent()
                               .WriteArgument("hashToken", ifDirectiveTrivia.HashToken)
                               .WriteArgument("ifKeyword", ifDirectiveTrivia.IfKeyword)
                               .WriteArgument("condition", ifDirectiveTrivia.Condition)
                               .WriteArgument("endOfDirectiveToken", ifDirectiveTrivia.EndOfDirectiveToken)
                               .WriteArgument("isActive", ifDirectiveTrivia.IsActive)
                               .WriteArgument("branchTaken", ifDirectiveTrivia.BranchTaken)
                               .WriteArgument("conditionValue", ifDirectiveTrivia.ConditionValue, closeArgumentList: true)
                               .PopIndent();
                case IfStatementSyntax ifStatement:
                    return this.AppendLine("SyntaxFactory.IfStatement(")
                               .PushIndent()
                               .WriteArgument("ifKeyword", ifStatement.IfKeyword)
                               .WriteArgument("openParenToken", ifStatement.OpenParenToken)
                               .WriteArgument("condition", ifStatement.Condition)
                               .WriteArgument("closeParenToken", ifStatement.CloseParenToken)
                               .WriteArgument("statement", ifStatement.Statement)
                               .WriteArgument("else", ifStatement.Else, closeArgumentList: true)
                               .PopIndent();
                case ImplicitArrayCreationExpressionSyntax implicitArrayCreationExpression:
                    return this.AppendLine("SyntaxFactory.ImplicitArrayCreationExpression(")
                               .PushIndent()
                               .WriteArgument("newKeyword", implicitArrayCreationExpression.NewKeyword)
                               .WriteArgument("openBracketToken", implicitArrayCreationExpression.OpenBracketToken)
                               .WriteArgument("commas", implicitArrayCreationExpression.Commas)
                               .WriteArgument("closeBracketToken", implicitArrayCreationExpression.CloseBracketToken)
                               .WriteArgument("initializer", implicitArrayCreationExpression.Initializer, closeArgumentList: true)
                               .PopIndent();
                case ImplicitElementAccessSyntax implicitElementAccess:
                    return this.AppendLine("SyntaxFactory.ImplicitElementAccess(")
                               .PushIndent()
                               .WriteArgument("argumentList", implicitElementAccess.ArgumentList, closeArgumentList: true)
                               .PopIndent();
                case ImplicitStackAllocArrayCreationExpressionSyntax implicitStackAllocArrayCreationExpression:
                    return this.AppendLine("SyntaxFactory.ImplicitStackAllocArrayCreationExpression(")
                               .PushIndent()
                               .WriteArgument("stackAllocKeyword", implicitStackAllocArrayCreationExpression.StackAllocKeyword)
                               .WriteArgument("openBracketToken", implicitStackAllocArrayCreationExpression.OpenBracketToken)
                               .WriteArgument("closeBracketToken", implicitStackAllocArrayCreationExpression.CloseBracketToken)
                               .WriteArgument("initializer", implicitStackAllocArrayCreationExpression.Initializer, closeArgumentList: true)
                               .PopIndent();
                case IncompleteMemberSyntax incompleteMember:
                    return this.AppendLine("SyntaxFactory.IncompleteMember(")
                               .PushIndent()
                               .WriteArgument("attributeLists", incompleteMember.AttributeLists)
                               .WriteArgument("modifiers", incompleteMember.Modifiers)
                               .WriteArgument("type", incompleteMember.Type, closeArgumentList: true)
                               .PopIndent();
                case IndexerDeclarationSyntax indexerDeclaration:
                    return this.AppendLine("SyntaxFactory.IndexerDeclaration(")
                               .PushIndent()
                               .WriteArgument("attributeLists", indexerDeclaration.AttributeLists)
                               .WriteArgument("modifiers", indexerDeclaration.Modifiers)
                               .WriteArgument("type", indexerDeclaration.Type)
                               .WriteArgument("explicitInterfaceSpecifier", indexerDeclaration.ExplicitInterfaceSpecifier)
                               .WriteArgument("thisKeyword", indexerDeclaration.ThisKeyword)
                               .WriteArgument("parameterList", indexerDeclaration.ParameterList)
                               .WriteArgument("accessorList", indexerDeclaration.AccessorList)
                               .WriteArgument("expressionBody", indexerDeclaration.ExpressionBody)
                               .WriteArgument("semicolonToken", indexerDeclaration.SemicolonToken, closeArgumentList: true)
                               .PopIndent();
                case IndexerMemberCrefSyntax indexerMemberCref:
                    return this.AppendLine("SyntaxFactory.IndexerMemberCref(")
                               .PushIndent()
                               .WriteArgument("thisKeyword", indexerMemberCref.ThisKeyword)
                               .WriteArgument("parameters", indexerMemberCref.Parameters, closeArgumentList: true)
                               .PopIndent();
                case InitializerExpressionSyntax initializerExpression:
                    return this.AppendLine("SyntaxFactory.InitializerExpression(")
                               .PushIndent()
                               .WriteArgument("kind", initializerExpression.Kind())
                               .WriteArgument("openBraceToken", initializerExpression.OpenBraceToken)
                               .WriteArgument("expressions", initializerExpression.Expressions)
                               .WriteArgument("closeBraceToken", initializerExpression.CloseBraceToken, closeArgumentList: true)
                               .PopIndent();
                case InterfaceDeclarationSyntax interfaceDeclaration:
                    return this.AppendLine("SyntaxFactory.InterfaceDeclaration(")
                               .PushIndent()
                               .WriteArgument("attributeLists", interfaceDeclaration.AttributeLists)
                               .WriteArgument("modifiers", interfaceDeclaration.Modifiers)
                               .WriteArgument("keyword", interfaceDeclaration.Keyword)
                               .WriteArgument("identifier", interfaceDeclaration.Identifier)
                               .WriteArgument("typeParameterList", interfaceDeclaration.TypeParameterList)
                               .WriteArgument("baseList", interfaceDeclaration.BaseList)
                               .WriteArgument("constraintClauses", interfaceDeclaration.ConstraintClauses)
                               .WriteArgument("openBraceToken", interfaceDeclaration.OpenBraceToken)
                               .WriteArgument("members", interfaceDeclaration.Members)
                               .WriteArgument("closeBraceToken", interfaceDeclaration.CloseBraceToken)
                               .WriteArgument("semicolonToken", interfaceDeclaration.SemicolonToken, closeArgumentList: true)
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
                case InterpolationAlignmentClauseSyntax interpolationAlignmentClause:
                    return this.AppendLine("SyntaxFactory.InterpolationAlignmentClause(")
                               .PushIndent()
                               .WriteArgument("commaToken", interpolationAlignmentClause.CommaToken)
                               .WriteArgument("value", interpolationAlignmentClause.Value, closeArgumentList: true)
                               .PopIndent();
                case InterpolationFormatClauseSyntax interpolationFormatClause:
                    return this.AppendLine("SyntaxFactory.InterpolationFormatClause(")
                               .PushIndent()
                               .WriteArgument("colonToken", interpolationFormatClause.ColonToken)
                               .WriteArgument("formatStringToken", interpolationFormatClause.FormatStringToken, closeArgumentList: true)
                               .PopIndent();
                case InvocationExpressionSyntax invocationExpression:
                    return this.AppendLine("SyntaxFactory.InvocationExpression(")
                               .PushIndent()
                               .WriteArgument("expression", invocationExpression.Expression)
                               .WriteArgument("argumentList", invocationExpression.ArgumentList, closeArgumentList: true)
                               .PopIndent();
                case IsPatternExpressionSyntax isPatternExpression:
                    return this.AppendLine("SyntaxFactory.IsPatternExpression(")
                               .PushIndent()
                               .WriteArgument("expression", isPatternExpression.Expression)
                               .WriteArgument("isKeyword", isPatternExpression.IsKeyword)
                               .WriteArgument("pattern", isPatternExpression.Pattern, closeArgumentList: true)
                               .PopIndent();
                case JoinClauseSyntax joinClause:
                    return this.AppendLine("SyntaxFactory.JoinClause(")
                               .PushIndent()
                               .WriteArgument("joinKeyword", joinClause.JoinKeyword)
                               .WriteArgument("type", joinClause.Type)
                               .WriteArgument("identifier", joinClause.Identifier)
                               .WriteArgument("inKeyword", joinClause.InKeyword)
                               .WriteArgument("inExpression", joinClause.InExpression)
                               .WriteArgument("onKeyword", joinClause.OnKeyword)
                               .WriteArgument("leftExpression", joinClause.LeftExpression)
                               .WriteArgument("equalsKeyword", joinClause.EqualsKeyword)
                               .WriteArgument("rightExpression", joinClause.RightExpression)
                               .WriteArgument("into", joinClause.Into, closeArgumentList: true)
                               .PopIndent();
                case JoinIntoClauseSyntax joinIntoClause:
                    return this.AppendLine("SyntaxFactory.JoinIntoClause(")
                               .PushIndent()
                               .WriteArgument("intoKeyword", joinIntoClause.IntoKeyword)
                               .WriteArgument("identifier", joinIntoClause.Identifier, closeArgumentList: true)
                               .PopIndent();
                case LabeledStatementSyntax labeledStatement:
                    return this.AppendLine("SyntaxFactory.LabeledStatement(")
                               .PushIndent()
                               .WriteArgument("identifier", labeledStatement.Identifier)
                               .WriteArgument("colonToken", labeledStatement.ColonToken)
                               .WriteArgument("statement", labeledStatement.Statement, closeArgumentList: true)
                               .PopIndent();
                case LetClauseSyntax letClause:
                    return this.AppendLine("SyntaxFactory.LetClause(")
                               .PushIndent()
                               .WriteArgument("letKeyword", letClause.LetKeyword)
                               .WriteArgument("identifier", letClause.Identifier)
                               .WriteArgument("equalsToken", letClause.EqualsToken)
                               .WriteArgument("expression", letClause.Expression, closeArgumentList: true)
                               .PopIndent();
                case LineDirectiveTriviaSyntax lineDirectiveTrivia:
                    return this.AppendLine("SyntaxFactory.LineDirectiveTrivia(")
                               .PushIndent()
                               .WriteArgument("hashToken", lineDirectiveTrivia.HashToken)
                               .WriteArgument("lineKeyword", lineDirectiveTrivia.LineKeyword)
                               .WriteArgument("line", lineDirectiveTrivia.Line)
                               .WriteArgument("file", lineDirectiveTrivia.File)
                               .WriteArgument("endOfDirectiveToken", lineDirectiveTrivia.EndOfDirectiveToken)
                               .WriteArgument("isActive", lineDirectiveTrivia.IsActive, closeArgumentList: true)
                               .PopIndent();
                case LiteralExpressionSyntax literalExpression:
                    return this.AppendLine("SyntaxFactory.LiteralExpression(")
                               .PushIndent()
                               .WriteArgument("kind", literalExpression.Kind())
                               .WriteArgument("token", literalExpression.Token, closeArgumentList: true)
                               .PopIndent();
                case LoadDirectiveTriviaSyntax loadDirectiveTrivia:
                    return this.AppendLine("SyntaxFactory.LoadDirectiveTrivia(")
                               .PushIndent()
                               .WriteArgument("hashToken", loadDirectiveTrivia.HashToken)
                               .WriteArgument("loadKeyword", loadDirectiveTrivia.LoadKeyword)
                               .WriteArgument("file", loadDirectiveTrivia.File)
                               .WriteArgument("endOfDirectiveToken", loadDirectiveTrivia.EndOfDirectiveToken)
                               .WriteArgument("isActive", loadDirectiveTrivia.IsActive, closeArgumentList: true)
                               .PopIndent();
                case LocalDeclarationStatementSyntax localDeclarationStatement:
                    return this.AppendLine("SyntaxFactory.LocalDeclarationStatement(")
                               .PushIndent()
                               .WriteArgument("awaitKeyword", localDeclarationStatement.AwaitKeyword)
                               .WriteArgument("usingKeyword", localDeclarationStatement.UsingKeyword)
                               .WriteArgument("modifiers", localDeclarationStatement.Modifiers)
                               .WriteArgument("declaration", localDeclarationStatement.Declaration)
                               .WriteArgument("semicolonToken", localDeclarationStatement.SemicolonToken, closeArgumentList: true)
                               .PopIndent();
                case LocalFunctionStatementSyntax localFunctionStatement:
                    return this.AppendLine("SyntaxFactory.LocalFunctionStatement(")
                               .PushIndent()
                               .WriteArgument("modifiers", localFunctionStatement.Modifiers)
                               .WriteArgument("returnType", localFunctionStatement.ReturnType)
                               .WriteArgument("identifier", localFunctionStatement.Identifier)
                               .WriteArgument("typeParameterList", localFunctionStatement.TypeParameterList)
                               .WriteArgument("parameterList", localFunctionStatement.ParameterList)
                               .WriteArgument("constraintClauses", localFunctionStatement.ConstraintClauses)
                               .WriteArgument("body", localFunctionStatement.Body)
                               .WriteArgument("expressionBody", localFunctionStatement.ExpressionBody)
                               .WriteArgument("semicolonToken", localFunctionStatement.SemicolonToken, closeArgumentList: true)
                               .PopIndent();
                case LockStatementSyntax lockStatement:
                    return this.AppendLine("SyntaxFactory.LockStatement(")
                               .PushIndent()
                               .WriteArgument("lockKeyword", lockStatement.LockKeyword)
                               .WriteArgument("openParenToken", lockStatement.OpenParenToken)
                               .WriteArgument("expression", lockStatement.Expression)
                               .WriteArgument("closeParenToken", lockStatement.CloseParenToken)
                               .WriteArgument("statement", lockStatement.Statement, closeArgumentList: true)
                               .PopIndent();
                case MakeRefExpressionSyntax makeRefExpression:
                    return this.AppendLine("SyntaxFactory.MakeRefExpression(")
                               .PushIndent()
                               .WriteArgument("keyword", makeRefExpression.Keyword)
                               .WriteArgument("openParenToken", makeRefExpression.OpenParenToken)
                               .WriteArgument("expression", makeRefExpression.Expression)
                               .WriteArgument("closeParenToken", makeRefExpression.CloseParenToken, closeArgumentList: true)
                               .PopIndent();
                case MemberAccessExpressionSyntax memberAccessExpression:
                    return this.AppendLine("SyntaxFactory.MemberAccessExpression(")
                               .PushIndent()
                               .WriteArgument("kind", memberAccessExpression.Kind())
                               .WriteArgument("expression", memberAccessExpression.Expression)
                               .WriteArgument("operatorToken", memberAccessExpression.OperatorToken)
                               .WriteArgument("name", memberAccessExpression.Name, closeArgumentList: true)
                               .PopIndent();
                case MemberBindingExpressionSyntax memberBindingExpression:
                    return this.AppendLine("SyntaxFactory.MemberBindingExpression(")
                               .PushIndent()
                               .WriteArgument("operatorToken", memberBindingExpression.OperatorToken)
                               .WriteArgument("name", memberBindingExpression.Name, closeArgumentList: true)
                               .PopIndent();
                case MethodDeclarationSyntax methodDeclaration:
                    return this.AppendLine("SyntaxFactory.MethodDeclaration(")
                               .PushIndent()
                               .WriteArgument("attributeLists", methodDeclaration.AttributeLists)
                               .WriteArgument("modifiers", methodDeclaration.Modifiers)
                               .WriteArgument("returnType", methodDeclaration.ReturnType)
                               .WriteArgument("explicitInterfaceSpecifier", methodDeclaration.ExplicitInterfaceSpecifier)
                               .WriteArgument("identifier", methodDeclaration.Identifier)
                               .WriteArgument("typeParameterList", methodDeclaration.TypeParameterList)
                               .WriteArgument("parameterList", methodDeclaration.ParameterList)
                               .WriteArgument("constraintClauses", methodDeclaration.ConstraintClauses)
                               .WriteArgument("body", methodDeclaration.Body)
                               .WriteArgument("expressionBody", methodDeclaration.ExpressionBody)
                               .WriteArgument("semicolonToken", methodDeclaration.SemicolonToken, closeArgumentList: true)
                               .PopIndent();
                case NameColonSyntax nameColon:
                    return this.AppendLine("SyntaxFactory.NameColon(")
                               .PushIndent()
                               .WriteArgument("name", nameColon.Name)
                               .WriteArgument("colonToken", nameColon.ColonToken, closeArgumentList: true)
                               .PopIndent();
                case NameEqualsSyntax nameEquals:
                    return this.AppendLine("SyntaxFactory.NameEquals(")
                               .PushIndent()
                               .WriteArgument("name", nameEquals.Name)
                               .WriteArgument("equalsToken", nameEquals.EqualsToken, closeArgumentList: true)
                               .PopIndent();
                case NameMemberCrefSyntax nameMemberCref:
                    return this.AppendLine("SyntaxFactory.NameMemberCref(")
                               .PushIndent()
                               .WriteArgument("name", nameMemberCref.Name)
                               .WriteArgument("parameters", nameMemberCref.Parameters, closeArgumentList: true)
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
                case NullableDirectiveTriviaSyntax nullableDirectiveTrivia:
                    return this.AppendLine("SyntaxFactory.NullableDirectiveTrivia(")
                               .PushIndent()
                               .WriteArgument("hashToken", nullableDirectiveTrivia.HashToken)
                               .WriteArgument("nullableKeyword", nullableDirectiveTrivia.NullableKeyword)
                               .WriteArgument("settingToken", nullableDirectiveTrivia.SettingToken)
                               .WriteArgument("endOfDirectiveToken", nullableDirectiveTrivia.EndOfDirectiveToken)
                               .WriteArgument("isActive", nullableDirectiveTrivia.IsActive, closeArgumentList: true)
                               .PopIndent();
                case NullableTypeSyntax nullableType:
                    return this.AppendLine("SyntaxFactory.NullableType(")
                               .PushIndent()
                               .WriteArgument("elementType", nullableType.ElementType)
                               .WriteArgument("questionToken", nullableType.QuestionToken, closeArgumentList: true)
                               .PopIndent();
                case ObjectCreationExpressionSyntax objectCreationExpression:
                    return this.AppendLine("SyntaxFactory.ObjectCreationExpression(")
                               .PushIndent()
                               .WriteArgument("newKeyword", objectCreationExpression.NewKeyword)
                               .WriteArgument("type", objectCreationExpression.Type)
                               .WriteArgument("argumentList", objectCreationExpression.ArgumentList)
                               .WriteArgument("initializer", objectCreationExpression.Initializer, closeArgumentList: true)
                               .PopIndent();
                case OmittedArraySizeExpressionSyntax omittedArraySizeExpression:
                    return this.AppendLine("SyntaxFactory.OmittedArraySizeExpression(")
                               .PushIndent()
                               .WriteArgument("omittedArraySizeExpressionToken", omittedArraySizeExpression.OmittedArraySizeExpressionToken, closeArgumentList: true)
                               .PopIndent();
                case OmittedTypeArgumentSyntax omittedTypeArgument:
                    return this.AppendLine("SyntaxFactory.OmittedTypeArgument(")
                               .PushIndent()
                               .WriteArgument("omittedTypeArgumentToken", omittedTypeArgument.OmittedTypeArgumentToken, closeArgumentList: true)
                               .PopIndent();
                case OperatorDeclarationSyntax operatorDeclaration:
                    return this.AppendLine("SyntaxFactory.OperatorDeclaration(")
                               .PushIndent()
                               .WriteArgument("attributeLists", operatorDeclaration.AttributeLists)
                               .WriteArgument("modifiers", operatorDeclaration.Modifiers)
                               .WriteArgument("returnType", operatorDeclaration.ReturnType)
                               .WriteArgument("operatorKeyword", operatorDeclaration.OperatorKeyword)
                               .WriteArgument("operatorToken", operatorDeclaration.OperatorToken)
                               .WriteArgument("parameterList", operatorDeclaration.ParameterList)
                               .WriteArgument("body", operatorDeclaration.Body)
                               .WriteArgument("expressionBody", operatorDeclaration.ExpressionBody)
                               .WriteArgument("semicolonToken", operatorDeclaration.SemicolonToken, closeArgumentList: true)
                               .PopIndent();
                case OperatorMemberCrefSyntax operatorMemberCref:
                    return this.AppendLine("SyntaxFactory.OperatorMemberCref(")
                               .PushIndent()
                               .WriteArgument("operatorKeyword", operatorMemberCref.OperatorKeyword)
                               .WriteArgument("operatorToken", operatorMemberCref.OperatorToken)
                               .WriteArgument("parameters", operatorMemberCref.Parameters, closeArgumentList: true)
                               .PopIndent();
                case OrderByClauseSyntax orderByClause:
                    return this.AppendLine("SyntaxFactory.OrderByClause(")
                               .PushIndent()
                               .WriteArgument("orderByKeyword", orderByClause.OrderByKeyword)
                               .WriteArgument("orderings", orderByClause.Orderings, closeArgumentList: true)
                               .PopIndent();
                case OrderingSyntax ordering:
                    return this.AppendLine("SyntaxFactory.Ordering(")
                               .PushIndent()
                               .WriteArgument("kind", ordering.Kind())
                               .WriteArgument("expression", ordering.Expression)
                               .WriteArgument("ascendingOrDescendingKeyword", ordering.AscendingOrDescendingKeyword, closeArgumentList: true)
                               .PopIndent();
                case ParameterSyntax parameter:
                    return this.AppendLine("SyntaxFactory.Parameter(")
                               .PushIndent()
                               .WriteArgument("attributeLists", parameter.AttributeLists)
                               .WriteArgument("modifiers", parameter.Modifiers)
                               .WriteArgument("type", parameter.Type)
                               .WriteArgument("identifier", parameter.Identifier)
                               .WriteArgument("default", parameter.Default, closeArgumentList: true)
                               .PopIndent();
                case ParameterListSyntax parameterList:
                    return this.AppendLine("SyntaxFactory.ParameterList(")
                               .PushIndent()
                               .WriteArgument("openParenToken", parameterList.OpenParenToken)
                               .WriteArgument("parameters", parameterList.Parameters)
                               .WriteArgument("closeParenToken", parameterList.CloseParenToken, closeArgumentList: true)
                               .PopIndent();
                case ParenthesizedExpressionSyntax parenthesizedExpression:
                    return this.AppendLine("SyntaxFactory.ParenthesizedExpression(")
                               .PushIndent()
                               .WriteArgument("openParenToken", parenthesizedExpression.OpenParenToken)
                               .WriteArgument("expression", parenthesizedExpression.Expression)
                               .WriteArgument("closeParenToken", parenthesizedExpression.CloseParenToken, closeArgumentList: true)
                               .PopIndent();
                case ParenthesizedLambdaExpressionSyntax parenthesizedLambdaExpression:
                    return this.AppendLine("SyntaxFactory.ParenthesizedLambdaExpression(")
                               .PushIndent()
                               .WriteArgument("asyncKeyword", parenthesizedLambdaExpression.AsyncKeyword)
                               .WriteArgument("parameterList", parenthesizedLambdaExpression.ParameterList)
                               .WriteArgument("arrowToken", parenthesizedLambdaExpression.ArrowToken)
                               .WriteArgument("body", parenthesizedLambdaExpression.Body, closeArgumentList: true)
                               .PopIndent();
                case ParenthesizedVariableDesignationSyntax parenthesizedVariableDesignation:
                    return this.AppendLine("SyntaxFactory.ParenthesizedVariableDesignation(")
                               .PushIndent()
                               .WriteArgument("openParenToken", parenthesizedVariableDesignation.OpenParenToken)
                               .WriteArgument("variables", parenthesizedVariableDesignation.Variables)
                               .WriteArgument("closeParenToken", parenthesizedVariableDesignation.CloseParenToken, closeArgumentList: true)
                               .PopIndent();
                case PointerTypeSyntax pointerType:
                    return this.AppendLine("SyntaxFactory.PointerType(")
                               .PushIndent()
                               .WriteArgument("elementType", pointerType.ElementType)
                               .WriteArgument("asteriskToken", pointerType.AsteriskToken, closeArgumentList: true)
                               .PopIndent();
                case PositionalPatternClauseSyntax positionalPatternClause:
                    return this.AppendLine("SyntaxFactory.PositionalPatternClause(")
                               .PushIndent()
                               .WriteArgument("openParenToken", positionalPatternClause.OpenParenToken)
                               .WriteArgument("subpatterns", positionalPatternClause.Subpatterns)
                               .WriteArgument("closeParenToken", positionalPatternClause.CloseParenToken, closeArgumentList: true)
                               .PopIndent();
                case PostfixUnaryExpressionSyntax postfixUnaryExpression:
                    return this.AppendLine("SyntaxFactory.PostfixUnaryExpression(")
                               .PushIndent()
                               .WriteArgument("kind", postfixUnaryExpression.Kind())
                               .WriteArgument("operand", postfixUnaryExpression.Operand)
                               .WriteArgument("operatorToken", postfixUnaryExpression.OperatorToken, closeArgumentList: true)
                               .PopIndent();
                case PragmaChecksumDirectiveTriviaSyntax pragmaChecksumDirectiveTrivia:
                    return this.AppendLine("SyntaxFactory.PragmaChecksumDirectiveTrivia(")
                               .PushIndent()
                               .WriteArgument("hashToken", pragmaChecksumDirectiveTrivia.HashToken)
                               .WriteArgument("pragmaKeyword", pragmaChecksumDirectiveTrivia.PragmaKeyword)
                               .WriteArgument("checksumKeyword", pragmaChecksumDirectiveTrivia.ChecksumKeyword)
                               .WriteArgument("file", pragmaChecksumDirectiveTrivia.File)
                               .WriteArgument("guid", pragmaChecksumDirectiveTrivia.Guid)
                               .WriteArgument("bytes", pragmaChecksumDirectiveTrivia.Bytes)
                               .WriteArgument("endOfDirectiveToken", pragmaChecksumDirectiveTrivia.EndOfDirectiveToken)
                               .WriteArgument("isActive", pragmaChecksumDirectiveTrivia.IsActive, closeArgumentList: true)
                               .PopIndent();
                case PragmaWarningDirectiveTriviaSyntax pragmaWarningDirectiveTrivia:
                    return this.AppendLine("SyntaxFactory.PragmaWarningDirectiveTrivia(")
                               .PushIndent()
                               .WriteArgument("hashToken", pragmaWarningDirectiveTrivia.HashToken)
                               .WriteArgument("pragmaKeyword", pragmaWarningDirectiveTrivia.PragmaKeyword)
                               .WriteArgument("warningKeyword", pragmaWarningDirectiveTrivia.WarningKeyword)
                               .WriteArgument("disableOrRestoreKeyword", pragmaWarningDirectiveTrivia.DisableOrRestoreKeyword)
                               .WriteArgument("nullableKeyword", pragmaWarningDirectiveTrivia.NullableKeyword)
                               .WriteArgument("errorCodes", pragmaWarningDirectiveTrivia.ErrorCodes)
                               .WriteArgument("endOfDirectiveToken", pragmaWarningDirectiveTrivia.EndOfDirectiveToken)
                               .WriteArgument("isActive", pragmaWarningDirectiveTrivia.IsActive, closeArgumentList: true)
                               .PopIndent();
                case PredefinedTypeSyntax predefinedType:
                    return this.AppendLine("SyntaxFactory.PredefinedType(")
                               .PushIndent()
                               .WriteArgument("keyword", predefinedType.Keyword, closeArgumentList: true)
                               .PopIndent();
                case PrefixUnaryExpressionSyntax prefixUnaryExpression:
                    return this.AppendLine("SyntaxFactory.PrefixUnaryExpression(")
                               .PushIndent()
                               .WriteArgument("kind", prefixUnaryExpression.Kind())
                               .WriteArgument("operatorToken", prefixUnaryExpression.OperatorToken)
                               .WriteArgument("operand", prefixUnaryExpression.Operand, closeArgumentList: true)
                               .PopIndent();
                case PropertyDeclarationSyntax propertyDeclaration:
                    return this.AppendLine("SyntaxFactory.PropertyDeclaration(")
                               .PushIndent()
                               .WriteArgument("attributeLists", propertyDeclaration.AttributeLists)
                               .WriteArgument("modifiers", propertyDeclaration.Modifiers)
                               .WriteArgument("type", propertyDeclaration.Type)
                               .WriteArgument("explicitInterfaceSpecifier", propertyDeclaration.ExplicitInterfaceSpecifier)
                               .WriteArgument("identifier", propertyDeclaration.Identifier)
                               .WriteArgument("accessorList", propertyDeclaration.AccessorList)
                               .WriteArgument("expressionBody", propertyDeclaration.ExpressionBody)
                               .WriteArgument("initializer", propertyDeclaration.Initializer)
                               .WriteArgument("semicolonToken", propertyDeclaration.SemicolonToken, closeArgumentList: true)
                               .PopIndent();
                case PropertyPatternClauseSyntax propertyPatternClause:
                    return this.AppendLine("SyntaxFactory.PropertyPatternClause(")
                               .PushIndent()
                               .WriteArgument("openBraceToken", propertyPatternClause.OpenBraceToken)
                               .WriteArgument("subpatterns", propertyPatternClause.Subpatterns)
                               .WriteArgument("closeBraceToken", propertyPatternClause.CloseBraceToken, closeArgumentList: true)
                               .PopIndent();
                case QualifiedCrefSyntax qualifiedCref:
                    return this.AppendLine("SyntaxFactory.QualifiedCref(")
                               .PushIndent()
                               .WriteArgument("container", qualifiedCref.Container)
                               .WriteArgument("dotToken", qualifiedCref.DotToken)
                               .WriteArgument("member", qualifiedCref.Member, closeArgumentList: true)
                               .PopIndent();
                case QualifiedNameSyntax qualifiedName:
                    return this.AppendLine("SyntaxFactory.QualifiedName(")
                               .PushIndent()
                               .WriteArgument("left", qualifiedName.Left)
                               .WriteArgument("dotToken", qualifiedName.DotToken)
                               .WriteArgument("right", qualifiedName.Right, closeArgumentList: true)
                               .PopIndent();
                case QueryBodySyntax queryBody:
                    return this.AppendLine("SyntaxFactory.QueryBody(")
                               .PushIndent()
                               .WriteArgument("clauses", queryBody.Clauses)
                               .WriteArgument("selectOrGroup", queryBody.SelectOrGroup)
                               .WriteArgument("continuation", queryBody.Continuation, closeArgumentList: true)
                               .PopIndent();
                case QueryContinuationSyntax queryContinuation:
                    return this.AppendLine("SyntaxFactory.QueryContinuation(")
                               .PushIndent()
                               .WriteArgument("intoKeyword", queryContinuation.IntoKeyword)
                               .WriteArgument("identifier", queryContinuation.Identifier)
                               .WriteArgument("body", queryContinuation.Body, closeArgumentList: true)
                               .PopIndent();
                case QueryExpressionSyntax queryExpression:
                    return this.AppendLine("SyntaxFactory.QueryExpression(")
                               .PushIndent()
                               .WriteArgument("fromClause", queryExpression.FromClause)
                               .WriteArgument("body", queryExpression.Body, closeArgumentList: true)
                               .PopIndent();
                case RangeExpressionSyntax rangeExpression:
                    return this.AppendLine("SyntaxFactory.RangeExpression(")
                               .PushIndent()
                               .WriteArgument("leftOperand", rangeExpression.LeftOperand)
                               .WriteArgument("operatorToken", rangeExpression.OperatorToken)
                               .WriteArgument("rightOperand", rangeExpression.RightOperand, closeArgumentList: true)
                               .PopIndent();
                case RecursivePatternSyntax recursivePattern:
                    return this.AppendLine("SyntaxFactory.RecursivePattern(")
                               .PushIndent()
                               .WriteArgument("type", recursivePattern.Type)
                               .WriteArgument("positionalPatternClause", recursivePattern.PositionalPatternClause)
                               .WriteArgument("propertyPatternClause", recursivePattern.PropertyPatternClause)
                               .WriteArgument("designation", recursivePattern.Designation, closeArgumentList: true)
                               .PopIndent();
                case ReferenceDirectiveTriviaSyntax referenceDirectiveTrivia:
                    return this.AppendLine("SyntaxFactory.ReferenceDirectiveTrivia(")
                               .PushIndent()
                               .WriteArgument("hashToken", referenceDirectiveTrivia.HashToken)
                               .WriteArgument("referenceKeyword", referenceDirectiveTrivia.ReferenceKeyword)
                               .WriteArgument("file", referenceDirectiveTrivia.File)
                               .WriteArgument("endOfDirectiveToken", referenceDirectiveTrivia.EndOfDirectiveToken)
                               .WriteArgument("isActive", referenceDirectiveTrivia.IsActive, closeArgumentList: true)
                               .PopIndent();
                case RefExpressionSyntax refExpression:
                    return this.AppendLine("SyntaxFactory.RefExpression(")
                               .PushIndent()
                               .WriteArgument("refKeyword", refExpression.RefKeyword)
                               .WriteArgument("expression", refExpression.Expression, closeArgumentList: true)
                               .PopIndent();
                case RefTypeSyntax refType:
                    return this.AppendLine("SyntaxFactory.RefType(")
                               .PushIndent()
                               .WriteArgument("refKeyword", refType.RefKeyword)
                               .WriteArgument("readOnlyKeyword", refType.ReadOnlyKeyword)
                               .WriteArgument("type", refType.Type, closeArgumentList: true)
                               .PopIndent();
                case RefTypeExpressionSyntax refTypeExpression:
                    return this.AppendLine("SyntaxFactory.RefTypeExpression(")
                               .PushIndent()
                               .WriteArgument("keyword", refTypeExpression.Keyword)
                               .WriteArgument("openParenToken", refTypeExpression.OpenParenToken)
                               .WriteArgument("expression", refTypeExpression.Expression)
                               .WriteArgument("closeParenToken", refTypeExpression.CloseParenToken, closeArgumentList: true)
                               .PopIndent();
                case RefValueExpressionSyntax refValueExpression:
                    return this.AppendLine("SyntaxFactory.RefValueExpression(")
                               .PushIndent()
                               .WriteArgument("keyword", refValueExpression.Keyword)
                               .WriteArgument("openParenToken", refValueExpression.OpenParenToken)
                               .WriteArgument("expression", refValueExpression.Expression)
                               .WriteArgument("comma", refValueExpression.Comma)
                               .WriteArgument("type", refValueExpression.Type)
                               .WriteArgument("closeParenToken", refValueExpression.CloseParenToken, closeArgumentList: true)
                               .PopIndent();
                case RegionDirectiveTriviaSyntax regionDirectiveTrivia:
                    return this.AppendLine("SyntaxFactory.RegionDirectiveTrivia(")
                               .PushIndent()
                               .WriteArgument("hashToken", regionDirectiveTrivia.HashToken)
                               .WriteArgument("regionKeyword", regionDirectiveTrivia.RegionKeyword)
                               .WriteArgument("endOfDirectiveToken", regionDirectiveTrivia.EndOfDirectiveToken)
                               .WriteArgument("isActive", regionDirectiveTrivia.IsActive, closeArgumentList: true)
                               .PopIndent();
                case ReturnStatementSyntax returnStatement:
                    return this.AppendLine("SyntaxFactory.ReturnStatement(")
                               .PushIndent()
                               .WriteArgument("returnKeyword", returnStatement.ReturnKeyword)
                               .WriteArgument("expression", returnStatement.Expression)
                               .WriteArgument("semicolonToken", returnStatement.SemicolonToken, closeArgumentList: true)
                               .PopIndent();
                case SelectClauseSyntax selectClause:
                    return this.AppendLine("SyntaxFactory.SelectClause(")
                               .PushIndent()
                               .WriteArgument("selectKeyword", selectClause.SelectKeyword)
                               .WriteArgument("expression", selectClause.Expression, closeArgumentList: true)
                               .PopIndent();
                case ShebangDirectiveTriviaSyntax shebangDirectiveTrivia:
                    return this.AppendLine("SyntaxFactory.ShebangDirectiveTrivia(")
                               .PushIndent()
                               .WriteArgument("hashToken", shebangDirectiveTrivia.HashToken)
                               .WriteArgument("exclamationToken", shebangDirectiveTrivia.ExclamationToken)
                               .WriteArgument("endOfDirectiveToken", shebangDirectiveTrivia.EndOfDirectiveToken)
                               .WriteArgument("isActive", shebangDirectiveTrivia.IsActive, closeArgumentList: true)
                               .PopIndent();
                case SimpleBaseTypeSyntax simpleBaseType:
                    return this.AppendLine("SyntaxFactory.SimpleBaseType(")
                               .PushIndent()
                               .WriteArgument("type", simpleBaseType.Type, closeArgumentList: true)
                               .PopIndent();
                case SimpleLambdaExpressionSyntax simpleLambdaExpression:
                    return this.AppendLine("SyntaxFactory.SimpleLambdaExpression(")
                               .PushIndent()
                               .WriteArgument("asyncKeyword", simpleLambdaExpression.AsyncKeyword)
                               .WriteArgument("parameter", simpleLambdaExpression.Parameter)
                               .WriteArgument("arrowToken", simpleLambdaExpression.ArrowToken)
                               .WriteArgument("body", simpleLambdaExpression.Body, closeArgumentList: true)
                               .PopIndent();
                case SingleVariableDesignationSyntax singleVariableDesignation:
                    return this.AppendLine("SyntaxFactory.SingleVariableDesignation(")
                               .PushIndent()
                               .WriteArgument("identifier", singleVariableDesignation.Identifier, closeArgumentList: true)
                               .PopIndent();
                case SizeOfExpressionSyntax sizeOfExpression:
                    return this.AppendLine("SyntaxFactory.SizeOfExpression(")
                               .PushIndent()
                               .WriteArgument("keyword", sizeOfExpression.Keyword)
                               .WriteArgument("openParenToken", sizeOfExpression.OpenParenToken)
                               .WriteArgument("type", sizeOfExpression.Type)
                               .WriteArgument("closeParenToken", sizeOfExpression.CloseParenToken, closeArgumentList: true)
                               .PopIndent();
                case SkippedTokensTriviaSyntax skippedTokensTrivia:
                    return this.AppendLine("SyntaxFactory.SkippedTokensTrivia(")
                               .PushIndent()
                               .WriteArgument("tokens", skippedTokensTrivia.Tokens, closeArgumentList: true)
                               .PopIndent();
                case StackAllocArrayCreationExpressionSyntax stackAllocArrayCreationExpression:
                    return this.AppendLine("SyntaxFactory.StackAllocArrayCreationExpression(")
                               .PushIndent()
                               .WriteArgument("stackAllocKeyword", stackAllocArrayCreationExpression.StackAllocKeyword)
                               .WriteArgument("type", stackAllocArrayCreationExpression.Type)
                               .WriteArgument("initializer", stackAllocArrayCreationExpression.Initializer, closeArgumentList: true)
                               .PopIndent();
                case StructDeclarationSyntax structDeclaration:
                    return this.AppendLine("SyntaxFactory.StructDeclaration(")
                               .PushIndent()
                               .WriteArgument("attributeLists", structDeclaration.AttributeLists)
                               .WriteArgument("modifiers", structDeclaration.Modifiers)
                               .WriteArgument("keyword", structDeclaration.Keyword)
                               .WriteArgument("identifier", structDeclaration.Identifier)
                               .WriteArgument("typeParameterList", structDeclaration.TypeParameterList)
                               .WriteArgument("baseList", structDeclaration.BaseList)
                               .WriteArgument("constraintClauses", structDeclaration.ConstraintClauses)
                               .WriteArgument("openBraceToken", structDeclaration.OpenBraceToken)
                               .WriteArgument("members", structDeclaration.Members)
                               .WriteArgument("closeBraceToken", structDeclaration.CloseBraceToken)
                               .WriteArgument("semicolonToken", structDeclaration.SemicolonToken, closeArgumentList: true)
                               .PopIndent();
                case SubpatternSyntax subpattern:
                    return this.AppendLine("SyntaxFactory.Subpattern(")
                               .PushIndent()
                               .WriteArgument("nameColon", subpattern.NameColon)
                               .WriteArgument("pattern", subpattern.Pattern, closeArgumentList: true)
                               .PopIndent();
                case SwitchExpressionSyntax switchExpression:
                    return this.AppendLine("SyntaxFactory.SwitchExpression(")
                               .PushIndent()
                               .WriteArgument("governingExpression", switchExpression.GoverningExpression)
                               .WriteArgument("switchKeyword", switchExpression.SwitchKeyword)
                               .WriteArgument("openBraceToken", switchExpression.OpenBraceToken)
                               .WriteArgument("arms", switchExpression.Arms)
                               .WriteArgument("closeBraceToken", switchExpression.CloseBraceToken, closeArgumentList: true)
                               .PopIndent();
                case SwitchExpressionArmSyntax switchExpressionArm:
                    return this.AppendLine("SyntaxFactory.SwitchExpressionArm(")
                               .PushIndent()
                               .WriteArgument("pattern", switchExpressionArm.Pattern)
                               .WriteArgument("whenClause", switchExpressionArm.WhenClause)
                               .WriteArgument("equalsGreaterThanToken", switchExpressionArm.EqualsGreaterThanToken)
                               .WriteArgument("expression", switchExpressionArm.Expression, closeArgumentList: true)
                               .PopIndent();
                case SwitchSectionSyntax switchSection:
                    return this.AppendLine("SyntaxFactory.SwitchSection(")
                               .PushIndent()
                               .WriteArgument("labels", switchSection.Labels)
                               .WriteArgument("statements", switchSection.Statements, closeArgumentList: true)
                               .PopIndent();
                case SwitchStatementSyntax switchStatement:
                    return this.AppendLine("SyntaxFactory.SwitchStatement(")
                               .PushIndent()
                               .WriteArgument("switchKeyword", switchStatement.SwitchKeyword)
                               .WriteArgument("openParenToken", switchStatement.OpenParenToken)
                               .WriteArgument("expression", switchStatement.Expression)
                               .WriteArgument("closeParenToken", switchStatement.CloseParenToken)
                               .WriteArgument("openBraceToken", switchStatement.OpenBraceToken)
                               .WriteArgument("sections", switchStatement.Sections)
                               .WriteArgument("closeBraceToken", switchStatement.CloseBraceToken, closeArgumentList: true)
                               .PopIndent();
                case ThisExpressionSyntax thisExpression:
                    return this.AppendLine("SyntaxFactory.ThisExpression(")
                               .PushIndent()
                               .WriteArgument("token", thisExpression.Token, closeArgumentList: true)
                               .PopIndent();
                case ThrowExpressionSyntax throwExpression:
                    return this.AppendLine("SyntaxFactory.ThrowExpression(")
                               .PushIndent()
                               .WriteArgument("throwKeyword", throwExpression.ThrowKeyword)
                               .WriteArgument("expression", throwExpression.Expression, closeArgumentList: true)
                               .PopIndent();
                case ThrowStatementSyntax throwStatement:
                    return this.AppendLine("SyntaxFactory.ThrowStatement(")
                               .PushIndent()
                               .WriteArgument("throwKeyword", throwStatement.ThrowKeyword)
                               .WriteArgument("expression", throwStatement.Expression)
                               .WriteArgument("semicolonToken", throwStatement.SemicolonToken, closeArgumentList: true)
                               .PopIndent();
                case TryStatementSyntax tryStatement:
                    return this.AppendLine("SyntaxFactory.TryStatement(")
                               .PushIndent()
                               .WriteArgument("tryKeyword", tryStatement.TryKeyword)
                               .WriteArgument("block", tryStatement.Block)
                               .WriteArgument("catches", tryStatement.Catches)
                               .WriteArgument("finally", tryStatement.Finally, closeArgumentList: true)
                               .PopIndent();
                case TupleElementSyntax tupleElement:
                    return this.AppendLine("SyntaxFactory.TupleElement(")
                               .PushIndent()
                               .WriteArgument("type", tupleElement.Type)
                               .WriteArgument("identifier", tupleElement.Identifier, closeArgumentList: true)
                               .PopIndent();
                case TupleExpressionSyntax tupleExpression:
                    return this.AppendLine("SyntaxFactory.TupleExpression(")
                               .PushIndent()
                               .WriteArgument("openParenToken", tupleExpression.OpenParenToken)
                               .WriteArgument("arguments", tupleExpression.Arguments)
                               .WriteArgument("closeParenToken", tupleExpression.CloseParenToken, closeArgumentList: true)
                               .PopIndent();
                case TupleTypeSyntax tupleType:
                    return this.AppendLine("SyntaxFactory.TupleType(")
                               .PushIndent()
                               .WriteArgument("openParenToken", tupleType.OpenParenToken)
                               .WriteArgument("elements", tupleType.Elements)
                               .WriteArgument("closeParenToken", tupleType.CloseParenToken, closeArgumentList: true)
                               .PopIndent();
                case TypeArgumentListSyntax typeArgumentList:
                    return this.AppendLine("SyntaxFactory.TypeArgumentList(")
                               .PushIndent()
                               .WriteArgument("lessThanToken", typeArgumentList.LessThanToken)
                               .WriteArgument("arguments", typeArgumentList.Arguments)
                               .WriteArgument("greaterThanToken", typeArgumentList.GreaterThanToken, closeArgumentList: true)
                               .PopIndent();
                case TypeConstraintSyntax typeConstraint:
                    return this.AppendLine("SyntaxFactory.TypeConstraint(")
                               .PushIndent()
                               .WriteArgument("type", typeConstraint.Type, closeArgumentList: true)
                               .PopIndent();
                case TypeCrefSyntax typeCref:
                    return this.AppendLine("SyntaxFactory.TypeCref(")
                               .PushIndent()
                               .WriteArgument("type", typeCref.Type, closeArgumentList: true)
                               .PopIndent();
                case TypeDeclarationSyntax typeDeclaration:
                    return this.AppendLine("SyntaxFactory.TypeDeclaration(")
                               .PushIndent()
                               .WriteArgument("kind", typeDeclaration.Kind())
                               .WriteArgument("identifier", typeDeclaration.Identifier, closeArgumentList: true)
                               .PopIndent();
                case TypeOfExpressionSyntax typeOfExpression:
                    return this.AppendLine("SyntaxFactory.TypeOfExpression(")
                               .PushIndent()
                               .WriteArgument("keyword", typeOfExpression.Keyword)
                               .WriteArgument("openParenToken", typeOfExpression.OpenParenToken)
                               .WriteArgument("type", typeOfExpression.Type)
                               .WriteArgument("closeParenToken", typeOfExpression.CloseParenToken, closeArgumentList: true)
                               .PopIndent();
                case TypeParameterSyntax typeParameter:
                    return this.AppendLine("SyntaxFactory.TypeParameter(")
                               .PushIndent()
                               .WriteArgument("attributeLists", typeParameter.AttributeLists)
                               .WriteArgument("varianceKeyword", typeParameter.VarianceKeyword)
                               .WriteArgument("identifier", typeParameter.Identifier, closeArgumentList: true)
                               .PopIndent();
                case TypeParameterConstraintClauseSyntax typeParameterConstraintClause:
                    return this.AppendLine("SyntaxFactory.TypeParameterConstraintClause(")
                               .PushIndent()
                               .WriteArgument("whereKeyword", typeParameterConstraintClause.WhereKeyword)
                               .WriteArgument("name", typeParameterConstraintClause.Name)
                               .WriteArgument("colonToken", typeParameterConstraintClause.ColonToken)
                               .WriteArgument("constraints", typeParameterConstraintClause.Constraints, closeArgumentList: true)
                               .PopIndent();
                case TypeParameterListSyntax typeParameterList:
                    return this.AppendLine("SyntaxFactory.TypeParameterList(")
                               .PushIndent()
                               .WriteArgument("lessThanToken", typeParameterList.LessThanToken)
                               .WriteArgument("parameters", typeParameterList.Parameters)
                               .WriteArgument("greaterThanToken", typeParameterList.GreaterThanToken, closeArgumentList: true)
                               .PopIndent();
                case UndefDirectiveTriviaSyntax undefDirectiveTrivia:
                    return this.AppendLine("SyntaxFactory.UndefDirectiveTrivia(")
                               .PushIndent()
                               .WriteArgument("hashToken", undefDirectiveTrivia.HashToken)
                               .WriteArgument("undefKeyword", undefDirectiveTrivia.UndefKeyword)
                               .WriteArgument("name", undefDirectiveTrivia.Name)
                               .WriteArgument("endOfDirectiveToken", undefDirectiveTrivia.EndOfDirectiveToken)
                               .WriteArgument("isActive", undefDirectiveTrivia.IsActive, closeArgumentList: true)
                               .PopIndent();
                case UnsafeStatementSyntax unsafeStatement:
                    return this.AppendLine("SyntaxFactory.UnsafeStatement(")
                               .PushIndent()
                               .WriteArgument("unsafeKeyword", unsafeStatement.UnsafeKeyword)
                               .WriteArgument("block", unsafeStatement.Block, closeArgumentList: true)
                               .PopIndent();
                case UsingDirectiveSyntax usingDirective:
                    return this.AppendLine("SyntaxFactory.UsingDirective(")
                               .PushIndent()
                               .WriteArgument("usingKeyword", usingDirective.UsingKeyword)
                               .WriteArgument("staticKeyword", usingDirective.StaticKeyword)
                               .WriteArgument("alias", usingDirective.Alias)
                               .WriteArgument("name", usingDirective.Name)
                               .WriteArgument("semicolonToken", usingDirective.SemicolonToken, closeArgumentList: true)
                               .PopIndent();
                case UsingStatementSyntax usingStatement:
                    return this.AppendLine("SyntaxFactory.UsingStatement(")
                               .PushIndent()
                               .WriteArgument("awaitKeyword", usingStatement.AwaitKeyword)
                               .WriteArgument("usingKeyword", usingStatement.UsingKeyword)
                               .WriteArgument("openParenToken", usingStatement.OpenParenToken)
                               .WriteArgument("declaration", usingStatement.Declaration)
                               .WriteArgument("expression", usingStatement.Expression)
                               .WriteArgument("closeParenToken", usingStatement.CloseParenToken)
                               .WriteArgument("statement", usingStatement.Statement, closeArgumentList: true)
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
                case WarningDirectiveTriviaSyntax warningDirectiveTrivia:
                    return this.AppendLine("SyntaxFactory.WarningDirectiveTrivia(")
                               .PushIndent()
                               .WriteArgument("hashToken", warningDirectiveTrivia.HashToken)
                               .WriteArgument("warningKeyword", warningDirectiveTrivia.WarningKeyword)
                               .WriteArgument("endOfDirectiveToken", warningDirectiveTrivia.EndOfDirectiveToken)
                               .WriteArgument("isActive", warningDirectiveTrivia.IsActive, closeArgumentList: true)
                               .PopIndent();
                case VarPatternSyntax varPattern:
                    return this.AppendLine("SyntaxFactory.VarPattern(")
                               .PushIndent()
                               .WriteArgument("varKeyword", varPattern.VarKeyword)
                               .WriteArgument("designation", varPattern.Designation, closeArgumentList: true)
                               .PopIndent();
                case WhenClauseSyntax whenClause:
                    return this.AppendLine("SyntaxFactory.WhenClause(")
                               .PushIndent()
                               .WriteArgument("whenKeyword", whenClause.WhenKeyword)
                               .WriteArgument("condition", whenClause.Condition, closeArgumentList: true)
                               .PopIndent();
                case WhereClauseSyntax whereClause:
                    return this.AppendLine("SyntaxFactory.WhereClause(")
                               .PushIndent()
                               .WriteArgument("whereKeyword", whereClause.WhereKeyword)
                               .WriteArgument("condition", whereClause.Condition, closeArgumentList: true)
                               .PopIndent();
                case WhileStatementSyntax whileStatement:
                    return this.AppendLine("SyntaxFactory.WhileStatement(")
                               .PushIndent()
                               .WriteArgument("whileKeyword", whileStatement.WhileKeyword)
                               .WriteArgument("openParenToken", whileStatement.OpenParenToken)
                               .WriteArgument("condition", whileStatement.Condition)
                               .WriteArgument("closeParenToken", whileStatement.CloseParenToken)
                               .WriteArgument("statement", whileStatement.Statement, closeArgumentList: true)
                               .PopIndent();
                case XmlCDataSectionSyntax xmlCDataSection:
                    return this.AppendLine("SyntaxFactory.XmlCDataSection(")
                               .PushIndent()
                               .WriteArgument("startCDataToken", xmlCDataSection.StartCDataToken)
                               .WriteArgument("textTokens", xmlCDataSection.TextTokens)
                               .WriteArgument("endCDataToken", xmlCDataSection.EndCDataToken, closeArgumentList: true)
                               .PopIndent();
                case XmlCommentSyntax xmlComment:
                    return this.AppendLine("SyntaxFactory.XmlComment(")
                               .PushIndent()
                               .WriteArgument("lessThanExclamationMinusMinusToken", xmlComment.LessThanExclamationMinusMinusToken)
                               .WriteArgument("textTokens", xmlComment.TextTokens)
                               .WriteArgument("minusMinusGreaterThanToken", xmlComment.MinusMinusGreaterThanToken, closeArgumentList: true)
                               .PopIndent();
                case XmlCrefAttributeSyntax xmlCrefAttribute:
                    return this.AppendLine("SyntaxFactory.XmlCrefAttribute(")
                               .PushIndent()
                               .WriteArgument("name", xmlCrefAttribute.Name)
                               .WriteArgument("equalsToken", xmlCrefAttribute.EqualsToken)
                               .WriteArgument("startQuoteToken", xmlCrefAttribute.StartQuoteToken)
                               .WriteArgument("cref", xmlCrefAttribute.Cref)
                               .WriteArgument("endQuoteToken", xmlCrefAttribute.EndQuoteToken, closeArgumentList: true)
                               .PopIndent();
                case XmlElementSyntax xmlElement:
                    return this.AppendLine("SyntaxFactory.XmlElement(")
                               .PushIndent()
                               .WriteArgument("startTag", xmlElement.StartTag)
                               .WriteArgument("content", xmlElement.Content)
                               .WriteArgument("endTag", xmlElement.EndTag, closeArgumentList: true)
                               .PopIndent();
                case XmlElementEndTagSyntax xmlElementEndTag:
                    return this.AppendLine("SyntaxFactory.XmlElementEndTag(")
                               .PushIndent()
                               .WriteArgument("lessThanSlashToken", xmlElementEndTag.LessThanSlashToken)
                               .WriteArgument("name", xmlElementEndTag.Name)
                               .WriteArgument("greaterThanToken", xmlElementEndTag.GreaterThanToken, closeArgumentList: true)
                               .PopIndent();
                case XmlElementStartTagSyntax xmlElementStartTag:
                    return this.AppendLine("SyntaxFactory.XmlElementStartTag(")
                               .PushIndent()
                               .WriteArgument("lessThanToken", xmlElementStartTag.LessThanToken)
                               .WriteArgument("name", xmlElementStartTag.Name)
                               .WriteArgument("attributes", xmlElementStartTag.Attributes)
                               .WriteArgument("greaterThanToken", xmlElementStartTag.GreaterThanToken, closeArgumentList: true)
                               .PopIndent();
                case XmlEmptyElementSyntax xmlEmptyElement:
                    return this.AppendLine("SyntaxFactory.XmlEmptyElement(")
                               .PushIndent()
                               .WriteArgument("lessThanToken", xmlEmptyElement.LessThanToken)
                               .WriteArgument("name", xmlEmptyElement.Name)
                               .WriteArgument("attributes", xmlEmptyElement.Attributes)
                               .WriteArgument("slashGreaterThanToken", xmlEmptyElement.SlashGreaterThanToken, closeArgumentList: true)
                               .PopIndent();
                case XmlNameSyntax xmlName:
                    return this.AppendLine("SyntaxFactory.XmlName(")
                               .PushIndent()
                               .WriteArgument("prefix", xmlName.Prefix)
                               .WriteArgument("localName", xmlName.LocalName, closeArgumentList: true)
                               .PopIndent();
                case XmlNameAttributeSyntax xmlNameAttribute:
                    return this.AppendLine("SyntaxFactory.XmlNameAttribute(")
                               .PushIndent()
                               .WriteArgument("name", xmlNameAttribute.Name)
                               .WriteArgument("equalsToken", xmlNameAttribute.EqualsToken)
                               .WriteArgument("startQuoteToken", xmlNameAttribute.StartQuoteToken)
                               .WriteArgument("identifier", xmlNameAttribute.Identifier)
                               .WriteArgument("endQuoteToken", xmlNameAttribute.EndQuoteToken, closeArgumentList: true)
                               .PopIndent();
                case XmlPrefixSyntax xmlPrefix:
                    return this.AppendLine("SyntaxFactory.XmlPrefix(")
                               .PushIndent()
                               .WriteArgument("prefix", xmlPrefix.Prefix)
                               .WriteArgument("colonToken", xmlPrefix.ColonToken, closeArgumentList: true)
                               .PopIndent();
                case XmlProcessingInstructionSyntax xmlProcessingInstruction:
                    return this.AppendLine("SyntaxFactory.XmlProcessingInstruction(")
                               .PushIndent()
                               .WriteArgument("startProcessingInstructionToken", xmlProcessingInstruction.StartProcessingInstructionToken)
                               .WriteArgument("name", xmlProcessingInstruction.Name)
                               .WriteArgument("textTokens", xmlProcessingInstruction.TextTokens)
                               .WriteArgument("endProcessingInstructionToken", xmlProcessingInstruction.EndProcessingInstructionToken, closeArgumentList: true)
                               .PopIndent();
                case XmlTextSyntax xmlText:
                    return this.AppendLine("SyntaxFactory.XmlText(")
                               .PushIndent()
                               .WriteArgument("textTokens", xmlText.TextTokens, closeArgumentList: true)
                               .PopIndent();
                case XmlTextAttributeSyntax xmlTextAttribute:
                    return this.AppendLine("SyntaxFactory.XmlTextAttribute(")
                               .PushIndent()
                               .WriteArgument("name", xmlTextAttribute.Name)
                               .WriteArgument("equalsToken", xmlTextAttribute.EqualsToken)
                               .WriteArgument("startQuoteToken", xmlTextAttribute.StartQuoteToken)
                               .WriteArgument("textTokens", xmlTextAttribute.TextTokens)
                               .WriteArgument("endQuoteToken", xmlTextAttribute.EndQuoteToken, closeArgumentList: true)
                               .PopIndent();
                case YieldStatementSyntax yieldStatement:
                    return this.AppendLine("SyntaxFactory.YieldStatement(")
                               .PushIndent()
                               .WriteArgument("kind", yieldStatement.Kind())
                               .WriteArgument("yieldKeyword", yieldStatement.YieldKeyword)
                               .WriteArgument("returnOrBreakKeyword", yieldStatement.ReturnOrBreakKeyword)
                               .WriteArgument("expression", yieldStatement.Expression)
                               .WriteArgument("semicolonToken", yieldStatement.SemicolonToken, closeArgumentList: true)
                               .PopIndent();

                default:
#pragma warning disable GU0090 // Don't throw NotImplementedException.
                    throw new NotImplementedException($"{nameof(SyntaxFactoryWriter)}.{nameof(this.Serialize)}({nameof(SyntaxNode)}) does not handle {node.Kind()}");
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
                               .WriteArgument("text", token.Text, escape: true)
                               .WriteArgument("trailing", token.TrailingTrivia, closeArgumentList: true)
                               .PopIndent();
                case SyntaxKind.IdentifierToken:
                    if (token.IsContextualKeyword())
                    {
                        return this.AppendLine("SyntaxFactory.Identifier(")
                                   .PushIndent()
                                   .WriteArgument("leading", token.LeadingTrivia)
                                   .WriteArgument("contextualKind", token.Kind())
                                   .WriteArgument("text", token.Text, escape: true)
                                   .WriteArgument("valueText", token.ValueText, escape: true)
                                   .WriteArgument("trailing", token.TrailingTrivia, closeArgumentList: true)
                                   .PopIndent();
                    }

                    return this.AppendLine("SyntaxFactory.Identifier(")
                               .PushIndent()
                               .WriteArgument("leading", token.LeadingTrivia)
                               .WriteArgument("text", token.Text, escape: true)
                               .WriteArgument("trailing", token.TrailingTrivia, closeArgumentList: true)
                               .PopIndent();

                case SyntaxKind.CharacterLiteralToken:
                case SyntaxKind.NumericLiteralToken:
                case SyntaxKind.StringLiteralToken:
                    if (!token.HasLeadingTrivia &&
                        !token.HasTrailingTrivia)
                    {
                        return this.AppendLine("SyntaxFactory.Literal(")
                                   .PushIndent()
                                   .WriteArgument("text", token.Text, escape: true)
                                   .WriteArgument("value", token.ValueText, escape: true, closeArgumentList: true)
                                   .PopIndent();
                    }

                    return this.AppendLine("SyntaxFactory.Literal(")
                               .PushIndent()
                               .WriteArgument("leading", token.LeadingTrivia)
                               .WriteArgument("text", token.Text, escape: true)
                               .WriteArgument("value", token.ValueText, escape: true)
                               .WriteArgument("trailing", token.TrailingTrivia, closeArgumentList: true)
                               .PopIndent();
                case SyntaxKind.XmlTextLiteralToken when token.HasLeadingTrivia || token.HasTrailingTrivia:
                    if (!token.HasLeadingTrivia &&
                        !token.HasTrailingTrivia)
                    {
                        return this.AppendLine("SyntaxFactory.XmlTextLiteral(")
                                   .PushIndent()
                                   .WriteArgument("leading", token.LeadingTrivia)
                                   .WriteArgument("text", token.Text, escape: true)
                                   .WriteArgument("value", token.ValueText, escape: true)
                                   .WriteArgument("trailing", token.TrailingTrivia, closeArgumentList: true)
                                   .PopIndent();
                    }

                    return this.AppendLine("SyntaxFactory.XmlEntity(")
                               .PushIndent()
                               .WriteArgument("leading", token.LeadingTrivia)
                               .WriteArgument("text", token.Text, escape: true)
                               .WriteArgument("value", token.ValueText, escape: true)
                               .WriteArgument("trailing", token.TrailingTrivia, closeArgumentList: true)
                               .PopIndent();

                case SyntaxKind.XmlTextLiteralToken:
                    if (token.Text != token.ValueText)
                    {
                        return this.AppendLine("SyntaxFactory.XmlTextLiteral(")
                                   .PushIndent()
                                   .WriteArgument("text", token.Text, escape: true)
                                   .WriteArgument("value", token.ValueText, escape: true, closeArgumentList: true)
                                   .PopIndent();
                    }

                    return this.Append($"SyntaxFactory.XmlTextLiteral(\"{token.Value}\")");
                case SyntaxKind.XmlTextLiteralNewLineToken:
                    if (token.Text.Length == 1)
                    {
                        return this.AppendLine("SyntaxFactory.XmlTextNewLine(")
                                   .PushIndent()
                                   .WriteArgument("leading", token.LeadingTrivia)
                                   .WriteArgument("text", "\\n", escape: false)
                                   .WriteArgument("value", "\\n", escape: false)
                                   .WriteArgument("trailing", token.TrailingTrivia, closeArgumentList: true)
                                   .PopIndent();
                    }

                    return this.AppendLine("SyntaxFactory.XmlTextNewLine(")
                               .PushIndent()
                               .WriteArgument("leading", token.LeadingTrivia)
                               .WriteArgument("text", "\\r\\n", escape: false)
                               .WriteArgument("value", "\\r\\n", escape: false)
                               .WriteArgument("trailing", token.TrailingTrivia, closeArgumentList: true)
                               .PopIndent();

                default:
                    if (token.Text != token.ValueText)
                    {
                        return this.AppendLine("SyntaxFactory.Token(")
                                   .PushIndent()
                                   .WriteArgument("leading", token.LeadingTrivia)
                                   .WriteArgument("kind", token.Kind())
                                   .WriteArgument("text", token.Text, escape: true)
                                   .WriteArgument("valueText", token.ValueText, escape: true)
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
            if (trivia.HasStructure)
            {
                return this.AppendLine("SyntaxFactory.Trivia(")
                    .PushIndent()
                    .Write(trivia.GetStructure())
                    .PopIndent()
                    .Append(")");
            }

            switch (trivia.Kind())
            {
                case SyntaxKind.None:
                    return this.Append("default");
                case SyntaxKind.SingleLineCommentTrivia:
                case SyntaxKind.MultiLineCommentTrivia:
                    return this.Append($"SyntaxFactory.Comment(\"{trivia.ToString()}\")");
                case SyntaxKind.DisabledTextTrivia:
                    return this.Append($"SyntaxFactory.DisabledText(\"{trivia.ToString()}\")");
                case SyntaxKind.DocumentationCommentExteriorTrivia:
                    return this.Append($"SyntaxFactory.DocumentationCommentExterior(\"{trivia.ToString()}\")");
                case SyntaxKind.WhitespaceTrivia:
                    if (trivia.Span.Length == 1)
                    {
                        return this.Append("SyntaxFactory.Space");
                    }

                    return this.Append($"SyntaxFactory.Whitespace(\"{trivia.ToString()}\")");
                case SyntaxKind.EndOfLineTrivia:
                    if (trivia.Span.Length == 1)
                    {
                        return this.Append("SyntaxFactory.LineFeed");
                    }

                    return this.Append("SyntaxFactory.CarriageReturnLineFeed");
                default:
                    throw new NotImplementedException($"Not handling {trivia.Kind()} yet.");
            }
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
                case 1 when syntaxList.SeparatorCount == 0:
                    return this.AppendLine($"SyntaxFactory.SingletonSeparatedList<{typeof(T).Name}>(")
                               .PushIndent()
                               .Write(syntaxList[0])
                               .Append(")")
                               .CloseArgument(closeArgumentList)
                               .PopIndent();
                default:
                    this.AppendLine("SyntaxFactory.SeparatedList(")
                        .PushIndent()
                        .AppendLine($"new {typeof(T).Name}[]")
                        .AppendLine("{")
                        .PushIndent();
                    foreach (var node in syntaxList)
                    {
                        _ = this.Write(node)
                                .AppendLine(",");
                    }

                    this.PopIndent()
                        .AppendLine("},")
                        .AppendLine($"new SyntaxToken[]")
                        .AppendLine("{")
                        .PushIndent();

                    foreach (var token in syntaxList.GetSeparators())
                    {
                        _ = this.Write(token)
                                .AppendLine(",");
                    }

                    return this.PopIndent()
                               .Append("})")
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
                        if (i < tokenList.Count - 1)
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
                        if (i < triviaList.Count - 1)
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
                    switch (trivia.Kind())
                    {
                        case SyntaxKind.None:
                            result = "default";
                            return true;
                        case SyntaxKind.WhitespaceTrivia:
                            result = trivia.Span.Length == 1
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

        private SyntaxFactoryWriter WriteArgument(string parameter, string text, bool escape, bool closeArgumentList = false)
        {
            this.Append(parameter)
                      .Append(": ")
                      .Append("\"");
            foreach (var c in text)
            {
                if (escape &&
                    (c == '\\' ||
                     c == '"'))
                {
                    this.writer.Append('\\');
                }

                this.writer.Append(c);
            }

            return this.Append("\"")
                      .CloseArgument(closeArgumentList);
        }

        private SyntaxFactoryWriter WriteArgument(string parameter, bool value, bool closeArgumentList = false)
        {
            return this.Append(parameter)
                       .Append(": ")
                       .Append("SyntaxKind.")
                       .Append(value ? "true" : "false")
                       .CloseArgument(closeArgumentList);
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
