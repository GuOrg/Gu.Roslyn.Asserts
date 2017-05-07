namespace Gu.Roslyn.Asserts
{
    using System;
    using System.Collections.Generic;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;

    public class AnalyzerAssert
    {
        public static readonly List<MetadataReference> References = new List<MetadataReference>();

        public static void HappyPath<TAnalyzer>(string code)
            where TAnalyzer : DiagnosticAnalyzer, new()
        {

        }

        public static void HappyPath(Type analyzerType, string code)
        {

        }

        public static void HappyPath(DiagnosticAnalyzer analyzer, string code)
        {

        }
    }
}
