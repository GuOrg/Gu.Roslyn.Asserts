namespace Gu.Roslyn.Asserts.Tests.TestHelpers.Suppressors;

using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

[DiagnosticAnalyzer(LanguageNames.CSharp)]
public class AllowUnassignedMagicMembers : DiagnosticSuppressor
{
    public const string MagicFieldName = "Magic";

    public static readonly SuppressionDescriptor FieldNameIsMagic = new(
        id: nameof(AllowUnassignedMagicMembers),
        suppressedDiagnosticId: "CS8618",
        justification: "Field is called " + MagicFieldName);

    public static readonly SuppressionDescriptor PropertyNameIsMagic = new(
        id: nameof(AllowUnassignedMagicMembers),
        suppressedDiagnosticId: "CS8618",
        justification: "Property is called " + MagicFieldName);

    public override ImmutableArray<SuppressionDescriptor> SupportedSuppressions { get; } =
        ImmutableArray.Create(FieldNameIsMagic, PropertyNameIsMagic);

    public override void ReportSuppressions(SuppressionAnalysisContext context)
    {
        foreach (var diagnostic in context.ReportedDiagnostics)
        {
            var sourceTree = diagnostic.Location.SourceTree;

            if (sourceTree is null)
            {
                continue;
            }

            var node = sourceTree.GetRoot(context.CancellationToken)
                                 .FindNode(diagnostic.Location.SourceSpan);

            if (node.ToString().Contains(MagicFieldName))
            {
                if (node is PropertyDeclarationSyntax)
                {
                    context.ReportSuppression(Suppression.Create(PropertyNameIsMagic, diagnostic));
                }
                else if (node is VariableDeclaratorSyntax &&
                    node.Parent is VariableDeclarationSyntax &&
                    node.Parent.Parent is FieldDeclarationSyntax)
                {
                    context.ReportSuppression(Suppression.Create(FieldNameIsMagic, diagnostic));
                }
            }
        }
    }
}
