namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
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
#if NET461
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
                               "Microsoft.CodeAnalysis.dll",
                               "System.Diagnostics.Debug.dll",
                               "System.Collections.dll",
                               "System.Collections.Immutable.dll",
                               "System.Threading.Tasks.dll",
                               "System.IO.dll",
                               "System.Text.Encoding.dll",
                               "System.Reflection.Metadata.dll",
                               "System.Reflection.dll",
                               "System.Globalization.dll",
                               "System.Threading.dll",
                               "System.Collections.Concurrent.dll",
                               "System.Linq.dll",
                               "System.Runtime.Extensions.dll",
                               "System.Xml.XDocument.dll",
                               "System.Runtime.InteropServices.dll",
                               "System.IO.FileSystem.dll",
                               "System.Reflection.Primitives.dll",
                               "System.Runtime.Numerics.dll",
                               "System.Diagnostics.Tools.dll",
                               "System.Resources.ResourceManager.dll",
                               "System.IO.FileSystem.Primitives.dll",
                               "System.Xml.ReaderWriter.dll",
                               "System.Security.Cryptography.Primitives.dll",
                               "System.Reflection.Extensions.dll",
                               "System.Text.Encoding.CodePages.dll",
                               "System.Text.Encoding.Extensions.dll",
                               "System.Linq.Expressions.dll",
                               "System.Threading.Tasks.Parallel.dll",
                           };
#else
            var expected = new[]
                           {
                                "Microsoft.CodeAnalysis.CSharp.dll",
                                "System.Runtime.dll",
                                "System.Private.CoreLib.dll",
                                "System.Private.Uri.dll",
                                "Microsoft.CodeAnalysis.dll",
                                "System.Diagnostics.Debug.dll",
                                "System.Collections.dll",
                                "System.Collections.Immutable.dll",
                                "System.Resources.ResourceManager.dll",
                                "System.Diagnostics.Tools.dll",
                                "System.Linq.dll",
                                "System.Runtime.Extensions.dll",
                                "System.Security.Principal.dll",
                                "System.Threading.dll",
                                "System.Threading.Tasks.dll",
                                "System.IO.dll",
                                "System.Text.Encoding.dll",
                                "System.Reflection.Metadata.dll",
                                "System.Runtime.InteropServices.dll",
                                "System.IO.Compression.dll",
                                "System.Buffers.dll",
                                "System.IO.MemoryMappedFiles.dll",
                                "System.IO.FileSystem.dll",
                                "System.Threading.Overlapped.dll",
                                "System.Text.Encoding.Extensions.dll",
                                "System.Reflection.dll",
                                "System.Security.Cryptography.Algorithms.dll",
                                "System.Security.Cryptography.Encoding.dll",
                                "System.Security.Cryptography.Primitives.dll",
                                "System.Collections.Concurrent.dll",
                                "System.Diagnostics.Tracing.dll",
                                "System.Globalization.dll",
                                "System.Xml.XDocument.dll",
                                "System.Private.Xml.Linq.dll",
                                "System.Private.Xml.dll",
                                "System.Diagnostics.TraceSource.dll",
                                "System.Collections.NonGeneric.dll",
                                "System.Collections.Specialized.dll",
                                "System.Text.RegularExpressions.dll",
                                "System.Net.Primitives.dll",
                                "Microsoft.Win32.Primitives.dll",
                                "System.Net.Requests.dll",
                                "System.Net.WebHeaderCollection.dll",
                                "System.Net.Http.dll",
                                "System.Security.Cryptography.X509Certificates.dll",
                                "System.Security.Cryptography.Cng.dll",
                                "System.Runtime.Numerics.dll",
                                "System.Security.Cryptography.Csp.dll",
                                "System.Threading.Thread.dll",
                                "System.Diagnostics.DiagnosticSource.dll",
                                "System.Net.ServicePoint.dll",
                                "System.Net.Security.dll",
                                "System.Security.Principal.Windows.dll",
                                "System.Security.Claims.dll",
                                "System.Threading.ThreadPool.dll",
                                "System.Net.Sockets.dll",
                                "System.Net.NameResolution.dll",
                                "System.Threading.Tasks.Extensions.dll",
                                "System.Reflection.Emit.dll",
                                "System.Reflection.Emit.ILGeneration.dll",
                                "System.Reflection.Primitives.dll",
                                "System.Reflection.Emit.Lightweight.dll",
                                "System.ObjectModel.dll",
                                "System.Console.dll",
                                "System.ValueTuple.dll",
                                "System.IO.FileSystem.Primitives.dll",
                                "System.Xml.ReaderWriter.dll",
                                "System.Xml.XPath.XDocument.dll",
                                "System.Reflection.Extensions.dll",
                                "System.Text.Encoding.CodePages.dll",
                                "System.Linq.Expressions.dll",
                                "System.Threading.Tasks.Parallel.dll",
                           };
#endif
            var actual = AnalyzerAssert.MetadataReferences
                                       .Select(x => Path.GetFileName(x.Display))
                                       .ToArray();
            Dump(actual);
            CollectionAssert.AreEqual(expected, actual);
        }

        [Conditional("DEBUG")]
        private static void Dump(IEnumerable<string> references)
        {
            foreach (var reference in references)
            {
                Console.WriteLine($"                                \"{reference}\",");
            }
        }
    }
}