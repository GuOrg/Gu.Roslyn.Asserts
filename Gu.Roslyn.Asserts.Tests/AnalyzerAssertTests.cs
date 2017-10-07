namespace Gu.Roslyn.Asserts.Tests
{
    using System.IO;
    using System.Linq;
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.CSharp;
    using NUnit.Framework;

    public class AnalyzerAssertTests
    {
        [Test]
        public void ResetMetadataReferences()
        {
            AnalyzerAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
            AnalyzerAssert.ResetMetadataReferences();
            CollectionAssert.IsEmpty(AnalyzerAssert.MetadataReferences);
        }

        [Test]
        public void AddTransitiveMetadataReferences()
        {
            AnalyzerAssert.MetadataReferences.Clear();
            AnalyzerAssert.AddTransitiveMetadataReferences(typeof(CSharpCompilationOptions).Assembly);
            var expected = new[]
                           {
                               "Microsoft.CodeAnalysis.CSharp.dll",
                               "System.Runtime.dll",
                               "netstandard.dll",
                               "mscorlib.dll",
                               "System.Core.dll",
                               "System.dll",
                               "System.Configuration.dll",
                               "System.Xml.dll",
                               "System.Data.SqlXml.dll",
                               "System.Security.dll",
                               "System.Numerics.dll",
                               "System.Data.dll",
                               "System.Transactions.dll",
                               "System.EnterpriseServices.dll",
                               "System.DirectoryServices.dll",
                               "System.Runtime.Remoting.dll",
                               "System.Web.dll",
                               "System.Drawing.dll",
                               "System.Web.RegularExpressions.dll",
                               "System.Design.dll",
                               "System.Windows.Forms.dll",
                               "Accessibility.dll",
                               "System.Deployment.dll",
                               "System.Runtime.Serialization.Formatters.Soap.dll",
                               "System.Data.OracleClient.dll",
                               "System.Drawing.Design.dll",
                               "System.Web.ApplicationServices.dll",
                               "System.ComponentModel.DataAnnotations.dll",
                               "System.DirectoryServices.Protocols.dll",
                               "System.ServiceProcess.dll",
                               "System.Configuration.Install.dll",
                               "System.Runtime.Serialization.dll",
                               "System.ServiceModel.Internals.dll",
                               "SMDiagnostics.dll",
                               "System.Web.Services.dll",
                               "Microsoft.Build.Utilities.v4.0.dll",
                               "Microsoft.Build.Framework.dll",
                               "System.Xaml.dll",
                               "Microsoft.Build.Tasks.v4.0.dll",
                               "System.Runtime.Caching.dll",
                               "System.Data.Common.dll",
                               "System.Diagnostics.StackTrace.dll",
                               "System.Diagnostics.Tracing.dll",
                               "System.Globalization.Extensions.dll",
                               "System.IO.Compression.dll",
                               "System.IO.Compression.FileSystem.dll",
                               "System.ComponentModel.Composition.dll",
                               "System.Net.Http.dll",
                               "System.Net.Sockets.dll",
                               "System.ValueTuple.dll",
                               "System.Runtime.InteropServices.RuntimeInformation.dll",
                               "System.Runtime.Serialization.Xml.dll",
                               "System.Runtime.Serialization.Primitives.dll",
                               "System.Security.Cryptography.Algorithms.dll",
                               "System.Security.SecureString.dll",
                               "System.Threading.Overlapped.dll",
                               "System.Xml.Linq.dll",
                               "System.Xml.XPath.XDocument.dll",
                               "System.Diagnostics.Debug.dll",
                               "Microsoft.CodeAnalysis.dll",
                               "System.Runtime.InteropServices.dll",
                               "System.Reflection.Metadata.dll",
                               "System.Collections.Immutable.dll",
                               "System.IO.dll",
                               "System.Collections.dll",
                               "System.Text.Encoding.dll",
                               "System.Threading.Tasks.dll",
                               "System.Reflection.Primitives.dll",
                               "System.Reflection.dll",
                               "System.Globalization.dll",
                               "System.Runtime.Extensions.dll",
                               "System.Runtime.Numerics.dll",
                               "System.Diagnostics.Tools.dll",
                               "System.Resources.ResourceManager.dll",
                               "System.Linq.dll",
                               "System.Collections.Concurrent.dll",
                               "System.Xml.ReaderWriter.dll",
                               "System.Xml.XDocument.dll",
                               "System.Dynamic.Runtime.dll",
                               "System.Threading.dll",
                               "System.Runtime.Serialization.Json.dll",
                               "System.Text.Encoding.Extensions.dll",
                               "System.Linq.Expressions.dll",
                               "System.Threading.Tasks.Parallel.dll",
                           };

            var actual = AnalyzerAssert.MetadataReferences
                                       .Select(x => Path.GetFileName(x.Display))
                                       .ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }
    }
}