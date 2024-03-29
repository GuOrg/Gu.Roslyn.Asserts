﻿namespace Gu.Roslyn.Asserts;

using System;

/// <summary>
/// Specifies what diagnostics to allow in the code generated by a code fix when calling RoslynAssert.CodeFix and RoslynAssert.FixAll.
/// </summary>
[AttributeUsage(AttributeTargets.Assembly, AllowMultiple = false)]
[Obsolete("Use " + nameof(SuppressWarningsAttribute))]
public sealed class AllowedDiagnosticsAttribute : Attribute
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AllowedDiagnosticsAttribute"/> class.
    /// </summary>
    /// <param name="allowedDiagnostics">The diagnostics to allow in the code generated by a code fix when calling RoslynAssert.CodeFix and RoslynAssert.FixAll.</param>
    public AllowedDiagnosticsAttribute(AllowedDiagnostics allowedDiagnostics)
    {
        this.AllowedDiagnostics = allowedDiagnostics;
    }

    /// <summary>
    /// Gets diagnostics to allow in the code generated by a code fix when calling RoslynAssert.CodeFix and RoslynAssert.FixAll.
    /// </summary>
    public AllowedDiagnostics AllowedDiagnostics { get; }
}
