namespace Gu.Roslyn.Asserts;

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Diagnostics;

public static partial class RoslynAssert
{
    // This has the same overloads as RoslynAssert.Diagnostics but here it complains?

    /// <summary>
    /// Verifies that <paramref name="code"/> produces the expected diagnostics
    /// and that these are suppressed by <paramref name="suppressor"/>.
    /// </summary>
    /// <param name="suppressor">The <see cref="DiagnosticSuppressor"/> to check <paramref name="code"/> with.</param>
    /// <param name="code">The code to analyze with <paramref name="suppressor"/>. Indicate diagnostic position with ↓ (alt + 25).</param>
    public static void Suppressed(DiagnosticSuppressor suppressor, params string[] code)
    {
        if (suppressor is null)
        {
            throw new ArgumentNullException(nameof(suppressor));
        }

        Suppressed(
            suppressor,
            DiagnosticsAndSources.FromMarkup(suppressor, code),
            Settings.Default);
    }

    /// <summary>
    /// Verifies that <paramref name="code"/> produces the expected diagnostics
    /// and that these are suppressed by <paramref name="suppressor"/>.
    /// </summary>
    /// <param name="suppressor">The <see cref="DiagnosticSuppressor"/> to check <paramref name="code"/> with.</param>
    /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="suppressor"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
    /// <param name="code">The code to analyze with <paramref name="suppressor"/>. Indicate diagnostic position with ↓ (alt + 25).</param>
    public static void Suppressed(DiagnosticSuppressor suppressor, ExpectedDiagnostic expectedDiagnostic, params string[] code)
    {
        if (suppressor is null)
        {
            throw new ArgumentNullException(nameof(suppressor));
        }

        if (expectedDiagnostic is null)
        {
            throw new ArgumentNullException(nameof(expectedDiagnostic));
        }

        Suppressed(
            suppressor,
            DiagnosticsAndSources.Create(expectedDiagnostic, code),
            Settings.Default);
    }

    /// <summary>
    /// Verifies that <paramref name="code"/> produces the expected diagnostics
    /// and that these are suppressed by <paramref name="suppressor"/>.
    /// </summary>
    /// <param name="suppressor">The <see cref="DiagnosticSuppressor"/> to check <paramref name="code"/> with.</param>
    /// <param name="expectedDiagnostics">The expected diagnostics.</param>
    /// <param name="code">The code to analyze with <paramref name="suppressor"/>. Indicate diagnostic position with ↓ (alt + 25).</param>
    public static void Suppressed(DiagnosticSuppressor suppressor, IReadOnlyList<ExpectedDiagnostic> expectedDiagnostics, params string[] code)
    {
        if (suppressor is null)
        {
            throw new ArgumentNullException(nameof(suppressor));
        }

        if (expectedDiagnostics is null)
        {
            throw new ArgumentNullException(nameof(expectedDiagnostics));
        }

        Suppressed(
            suppressor,
            new DiagnosticsAndSources(expectedDiagnostics, code),
            Settings.Default);
    }

    /// <summary>
    /// Verifies that <paramref name="code"/> produces the expected diagnostics
    /// and that these are suppressed by <paramref name="suppressor"/>.
    /// </summary>
    /// <param name="suppressor">The <see cref="DiagnosticSuppressor"/> to check <paramref name="code"/> with.</param>
    /// <param name="code">The code to analyze with <paramref name="suppressor"/>. Indicate diagnostic position with ↓ (alt + 25).</param>
    /// <param name="settings">The <see cref="Settings"/>.</param>
    public static void Suppressed(
        DiagnosticSuppressor suppressor,
        string code,
        Settings? settings = null)
    {
        if (suppressor is null)
        {
            throw new ArgumentNullException(nameof(suppressor));
        }

        if (code is null)
        {
            throw new ArgumentNullException(nameof(code));
        }

        Suppressed(
            suppressor,
            DiagnosticsAndSources.FromMarkup(suppressor, code),
            settings);
    }

    /// <summary>
    /// Verifies that <paramref name="code"/> produces the expected diagnostics
    /// and that these are suppressed by <paramref name="suppressor"/>.
    /// </summary>
    /// <param name="suppressor">The <see cref="DiagnosticSuppressor"/> to check <paramref name="code"/> with.</param>
    /// <param name="code">The code to analyze with <paramref name="suppressor"/>. Indicate diagnostic position with ↓ (alt + 25).</param>
    /// <param name="settings">The <see cref="Settings"/>.</param>
    public static void Suppressed(
        DiagnosticSuppressor suppressor,
        IReadOnlyList<string> code,
        Settings? settings = null)
    {
        if (suppressor is null)
        {
            throw new ArgumentNullException(nameof(suppressor));
        }

        if (code is null)
        {
            throw new ArgumentNullException(nameof(code));
        }

        Suppressed(
            suppressor,
            DiagnosticsAndSources.FromMarkup(suppressor, code),
            settings);
    }

    /// <summary>
    /// Verifies that <paramref name="code"/> produces the expected diagnostics
    /// and that these are suppressed by <paramref name="suppressor"/>.
    /// </summary>
    /// <param name="suppressor">The <see cref="DiagnosticSuppressor"/> to check <paramref name="code"/> with.</param>
    /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="suppressor"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
    /// <param name="code">The code to analyze with <paramref name="suppressor"/>. Indicate diagnostic position with ↓ (alt + 25).</param>
    /// <param name="settings">The <see cref="Settings"/>.</param>
    public static void Suppressed(
        DiagnosticSuppressor suppressor,
        ExpectedDiagnostic expectedDiagnostic,
        string code,
        Settings? settings = null)
    {
        if (suppressor is null)
        {
            throw new ArgumentNullException(nameof(suppressor));
        }

        if (expectedDiagnostic is null)
        {
            throw new ArgumentNullException(nameof(expectedDiagnostic));
        }

        if (code is null)
        {
            throw new ArgumentNullException(nameof(code));
        }

        Suppressed(
            suppressor,
            DiagnosticsAndSources.Create(expectedDiagnostic, code),
            settings);
    }

    /// <summary>
    /// Verifies that <paramref name="code"/> produces the expected diagnostics
    /// and that these are suppressed by <paramref name="suppressor"/>.
    /// </summary>
    /// <param name="suppressor">The <see cref="DiagnosticSuppressor"/> to check <paramref name="code"/> with.</param>
    /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="suppressor"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
    /// <param name="code">The code to analyze with <paramref name="suppressor"/>. Indicate diagnostic position with ↓ (alt + 25).</param>
    /// <param name="settings">The <see cref="Settings"/>.</param>
    public static void Suppressed(
        DiagnosticSuppressor suppressor,
        ExpectedDiagnostic expectedDiagnostic,
        IReadOnlyList<string> code,
        Settings? settings = null)
    {
        if (suppressor is null)
        {
            throw new ArgumentNullException(nameof(suppressor));
        }

        if (expectedDiagnostic is null)
        {
            throw new ArgumentNullException(nameof(expectedDiagnostic));
        }

        if (code is null)
        {
            throw new ArgumentNullException(nameof(code));
        }

        Suppressed(
            suppressor,
            DiagnosticsAndSources.Create(expectedDiagnostic, code),
            settings);
    }

    /// <summary>
    /// Verifies that <paramref name="diagnosticsAndSources"/> produces the expected diagnostics
    /// and that these are suppressed by <paramref name="suppressor"/>.
    /// </summary>
    /// <param name="suppressor">The <see cref="DiagnosticSuppressor"/> to check <paramref name="diagnosticsAndSources"/> with.</param>
    /// <param name="diagnosticsAndSources">The code to analyze with <paramref name="suppressor"/>. Indicate diagnostic position with ↓ (alt + 25).</param>
    /// <param name="settings">The <see cref="Settings"/>.</param>
    public static void Suppressed(
        DiagnosticSuppressor suppressor,
        DiagnosticsAndSources diagnosticsAndSources,
        Settings? settings = null)
    {
        SuppressedOrNot(suppressor, diagnosticsAndSources, settings, x => !x.IsSuppressed);
    }

    /// <summary>
    /// Verifies that any diagnostics in <paramref name="code"/> are not suppressed when analyzed with <paramref name="suppressor"/>.
    /// </summary>
    /// <param name="suppressor">The <see cref="DiagnosticSuppressor"/> to check <paramref name="code"/> with.</param>
    /// <param name="code">The code to analyze using <paramref name="suppressor"/>. Analyzing the code is not expected to suppress any errors or warnings.</param>
    /// <param name="settings">The <see cref="Settings"/>.</param>
    public static void NotSuppressed(DiagnosticSuppressor suppressor, string code, Settings? settings = null)
    {
        if (suppressor is null)
        {
            throw new ArgumentNullException(nameof(suppressor));
        }

        if (code is null)
        {
            throw new ArgumentNullException(nameof(code));
        }

        SuppressedOrNot(
            suppressor,
            DiagnosticsAndSources.FromMarkup(suppressor, code),
            settings,
            d => d.IsSuppressed);
    }

    /// <summary>
    /// Verifies that <paramref name="code"/> produces the expected diagnostics
    /// and that these are suppressed by <paramref name="suppressor"/>.
    /// </summary>
    /// <param name="suppressor">The <see cref="DiagnosticSuppressor"/> to check <paramref name="code"/> with.</param>
    /// <param name="expectedDiagnostic">The <see cref="ExpectedDiagnostic"/> with information about the expected <see cref="Diagnostic"/>. If <paramref name="suppressor"/> supports more than one <see cref="DiagnosticDescriptor.Id"/> this must be provided.</param>
    /// <param name="code">The code to analyze using <paramref name="suppressor"/>. Analyzing the code is not expected to suppress any errors or warnings.</param>
    /// <param name="settings">The <see cref="Settings"/>.</param>
    public static void NotSuppressed(DiagnosticSuppressor suppressor, ExpectedDiagnostic expectedDiagnostic, string code, Settings? settings = null)
    {
        if (suppressor is null)
        {
            throw new ArgumentNullException(nameof(suppressor));
        }

        if (expectedDiagnostic is null)
        {
            throw new ArgumentNullException(nameof(expectedDiagnostic));
        }

        SuppressedOrNot(
            suppressor,
            DiagnosticsAndSources.Create(expectedDiagnostic, code),
            settings,
            d => d.IsSuppressed);
    }

    /// <summary>
    /// Verifies that <paramref name="diagnosticsAndSources"/> produces the expected diagnostics
    /// and that these are (not) suppressed by <paramref name="suppressor"/>.
    /// </summary>
    /// <param name="suppressor">The <see cref="DiagnosticSuppressor"/> to check <paramref name="diagnosticsAndSources"/> with.</param>
    /// <param name="diagnosticsAndSources">The code to analyze with <paramref name="suppressor"/>. Indicate diagnostic position with ↓ (alt + 25).</param>
    /// <param name="settings">The <see cref="Settings"/>.</param>
    /// <param name="filter">Filter to apply to final <see cref="Diagnostic"/>.</param>
    private static void SuppressedOrNot(
        DiagnosticSuppressor suppressor,
        DiagnosticsAndSources diagnosticsAndSources,
        Settings? settings,
        Func<Diagnostic, bool> filter)
    {
        if (suppressor is null)
        {
            throw new ArgumentNullException(nameof(suppressor));
        }

        if (diagnosticsAndSources is null)
        {
            throw new ArgumentNullException(nameof(diagnosticsAndSources));
        }

        settings ??= Settings.Default;
        VerifySuppressorSupportsDiagnostics(suppressor, diagnosticsAndSources.ExpectedDiagnostics);
        var sln = CodeFactory.CreateSolution(
            suppressor,
            diagnosticsAndSources,
            settings);

        // Verify that the errors exist when we don't have the suppressor
        var diagnostics = Analyze.GetAllDiagnostics(sln);
        VerifyDiagnostics(diagnosticsAndSources, diagnostics, diagnostics);

        // Now see if the suppressor actually suppresses the errors.
        diagnostics = Analyze.GetAllDiagnostics(suppressor, sln);

        // All errors should have been gone.
        NoDiagnostics(diagnostics.Where(filter));
    }
}
