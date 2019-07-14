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

    public static partial class RoslynAssertTests
    {
        [Test]
        public static void ResetMetadataReferences()
        {
            RoslynAssert.MetadataReferences.Add(MetadataReference.CreateFromFile(typeof(int).Assembly.Location));
            RoslynAssert.ResetMetadataReferences();
            CollectionAssert.IsEmpty(RoslynAssert.MetadataReferences);
        }

        [Test]
        public static void AddTransitiveMetadataReferences()
        {
            RoslynAssert.MetadataReferences.Clear();
            RoslynAssert.AddTransitiveMetadataReferences(typeof(CSharpCompilationOptions).Assembly);
#if NET472
            var expected = new[]
                           {
                                "Accessibility.dll",
                                "Microsoft.Build.Framework.dll",
                                "Microsoft.Build.Tasks.v4.0.dll",
                                "Microsoft.Build.Utilities.v4.0.dll",
                                "Microsoft.CodeAnalysis.CSharp.dll",
                                "Microsoft.CodeAnalysis.dll",
                                "mscorlib.dll",
                                "netstandard.dll",
                                "SMDiagnostics.dll",
                                "System.Buffers.dll",
                                "System.Collections.Immutable.dll",
                                "System.ComponentModel.Composition.dll",
                                "System.ComponentModel.DataAnnotations.dll",
                                "System.Configuration.dll",
                                "System.Configuration.Install.dll",
                                "System.Core.dll",
                                "System.Data.Common.dll",
                                "System.Data.dll",
                                "System.Data.OracleClient.dll",
                                "System.Data.SqlXml.dll",
                                "System.Deployment.dll",
                                "System.Design.dll",
                                "System.Diagnostics.StackTrace.dll",
                                "System.Diagnostics.Tracing.dll",
                                "System.DirectoryServices.dll",
                                "System.DirectoryServices.Protocols.dll",
                                "System.dll",
                                "System.Drawing.Design.dll",
                                "System.Drawing.dll",
                                "System.EnterpriseServices.dll",
                                "System.Globalization.Extensions.dll",
                                "System.IO.Compression.dll",
                                "System.IO.Compression.FileSystem.dll",
                                "System.Memory.dll",
                                "System.Net.Http.dll",
                                "System.Net.Sockets.dll",
                                "System.Numerics.dll",
                                "System.Numerics.Vectors.dll",
                                "System.Reflection.Metadata.dll",
                                "System.Runtime.Caching.dll",
                                "System.Runtime.CompilerServices.Unsafe.dll",
                                "System.Runtime.dll",
                                "System.Runtime.InteropServices.RuntimeInformation.dll",
                                "System.Runtime.Remoting.dll",
                                "System.Runtime.Serialization.dll",
                                "System.Runtime.Serialization.Formatters.Soap.dll",
                                "System.Runtime.Serialization.Primitives.dll",
                                "System.Runtime.Serialization.Xml.dll",
                                "System.Security.Cryptography.Algorithms.dll",
                                "System.Security.dll",
                                "System.Security.SecureString.dll",
                                "System.ServiceModel.Internals.dll",
                                "System.ServiceProcess.dll",
                                "System.Text.Encoding.CodePages.dll",
                                "System.Threading.Overlapped.dll",
                                "System.Threading.Tasks.Extensions.dll",
                                "System.Transactions.dll",
                                "System.ValueTuple.dll",
                                "System.Web.ApplicationServices.dll",
                                "System.Web.dll",
                                "System.Web.RegularExpressions.dll",
                                "System.Web.Services.dll",
                                "System.Windows.Forms.dll",
                                "System.Xaml.dll",
                                "System.Xml.dll",
                                "System.Xml.Linq.dll",
                                "System.Xml.XPath.XDocument.dll",
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
            var actual = RoslynAssert.MetadataReferences
                                       .Select(x => Path.GetFileName(x.Display))
                                       .OrderBy(x => x)
                                       .ToArray();
            //// Dump(actual);
            CollectionAssert.AreEqual(expected, actual);
        }

        [Conditional("DEBUG")]
#pragma warning disable IDE0051 // Remove unused private members
        private static void Dump(IEnumerable<string> references)
#pragma warning restore IDE0051 // Remove unused private members
        {
            foreach (var reference in references.OrderBy(x => x))
            {
                Console.WriteLine($"                                \"{reference}\",");
            }
        }
    }
}
