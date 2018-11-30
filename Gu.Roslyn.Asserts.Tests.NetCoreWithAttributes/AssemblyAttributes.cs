using Gu.Roslyn.Asserts;

[assembly: TransitiveMetadataReferences(
    typeof(Gu.Roslyn.Asserts.Tests.NetCoreWithAttributes.AnalyzerAssertTests),
    typeof(Microsoft.EntityFrameworkCore.DbContext),
    typeof(Microsoft.AspNetCore.Mvc.Controller))]
