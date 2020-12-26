using System;
using Gu.Roslyn.Asserts;
using Gu.Roslyn.Asserts.Analyzers.Tests.RenameObsoleteFixTests;

[assembly: CLSCompliant(false)]
[assembly: TransitiveMetadataReferences(typeof(CodeFix))]
[assembly: SuppressWarnings("CS1701", "CS1702", "CS0281")]
