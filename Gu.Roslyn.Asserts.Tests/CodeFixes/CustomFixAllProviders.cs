namespace Gu.Roslyn.Asserts.Tests.CodeFixes
{
    using Microsoft.CodeAnalysis.CodeFixes;

    internal static class CustomFixAllProviders
    {
        /// <summary>
        /// Gets the default batch fix all provider.
        /// This provider batches all the individual diagnostic fixes across the scope of fix all action,
        /// computes fixes in parallel and then merges all the non-conflicting fixes into a single fix all code action.
        /// This fixer supports fixes for the following fix all scopes:
        /// <see cref="FixAllScope.Document"/>, <see cref="FixAllScope.Project"/> and <see cref="FixAllScope.Solution"/>.
        /// </summary>
        /// <remarks>
        /// The batch fix all provider only batches operations (i.e. <see cref="Microsoft.CodeAnalysis.CodeActions.CodeActionOperation"/>) of type
        /// <see cref="Microsoft.CodeAnalysis.CodeActions.ApplyChangesOperation"/> present within the individual diagnostic fixes. Other types of
        /// operations present within these fixes are ignored.
        /// </remarks>
        /// <value>
        /// The default batch fix all provider.
        /// </value>
        public static FixAllProvider BatchFixer => CustomBatchFixAllProvider.Instance;
    }
}