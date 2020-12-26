using System;
using Gu.Roslyn.Asserts;

[assembly: CLSCompliant(false)]

[assembly: MetadataReference(typeof(object), new[] { "global", "mscorlib" })]
[assembly: MetadataReference(typeof(System.Diagnostics.Debug), new[] { "global", "System" })]
[assembly: TransitiveMetadataReferences(typeof(Microsoft.CodeAnalysis.CSharp.CSharpCompilation))]
[assembly: TransitiveMetadataReferences(typeof(Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider))]
[assembly: TransitiveMetadataReferences(typeof(System.Windows.Window))]
[assembly: MetadataReferences(
    typeof(System.Linq.Enumerable),
    typeof(System.Net.WebClient),
    typeof(System.Drawing.Bitmap),
    typeof(System.Data.Common.DbConnection),
    typeof(System.Xml.Serialization.XmlSerializer),
    typeof(System.Runtime.Serialization.DataContractSerializer),
    typeof(NUnit.Framework.Assert))]
