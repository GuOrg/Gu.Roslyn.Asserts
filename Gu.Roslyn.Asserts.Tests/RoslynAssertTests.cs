namespace Gu.Roslyn.Asserts.Tests
{
    using System;
    using System.Collections.Generic;
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
#elif NETCOREAPP2_0
            var expected = new[]
                           {
                                "Microsoft.CodeAnalysis.CSharp.dll",
                                "Microsoft.CodeAnalysis.dll",
                                "Microsoft.Win32.Primitives.dll",
                                "Microsoft.Win32.Registry.dll",
                                "netstandard.dll",
                                "System.Buffers.dll",
                                "System.Collections.Concurrent.dll",
                                "System.Collections.dll",
                                "System.Collections.Immutable.dll",
                                "System.Collections.NonGeneric.dll",
                                "System.Collections.Specialized.dll",
                                "System.ComponentModel.dll",
                                "System.ComponentModel.EventBasedAsync.dll",
                                "System.ComponentModel.Primitives.dll",
                                "System.ComponentModel.TypeConverter.dll",
                                "System.Console.dll",
                                "System.Data.Common.dll",
                                "System.Diagnostics.Contracts.dll",
                                "System.Diagnostics.Debug.dll",
                                "System.Diagnostics.DiagnosticSource.dll",
                                "System.Diagnostics.FileVersionInfo.dll",
                                "System.Diagnostics.Process.dll",
                                "System.Diagnostics.StackTrace.dll",
                                "System.Diagnostics.TextWriterTraceListener.dll",
                                "System.Diagnostics.Tools.dll",
                                "System.Diagnostics.TraceSource.dll",
                                "System.Diagnostics.Tracing.dll",
                                "System.Drawing.Primitives.dll",
                                "System.IO.Compression.dll",
                                "System.IO.Compression.ZipFile.dll",
                                "System.IO.FileSystem.AccessControl.dll",
                                "System.IO.FileSystem.dll",
                                "System.IO.FileSystem.DriveInfo.dll",
                                "System.IO.FileSystem.Watcher.dll",
                                "System.IO.IsolatedStorage.dll",
                                "System.IO.MemoryMappedFiles.dll",
                                "System.IO.Pipes.dll",
                                "System.Linq.dll",
                                "System.Linq.Expressions.dll",
                                "System.Linq.Parallel.dll",
                                "System.Linq.Queryable.dll",
                                "System.Memory.dll",
                                "System.Net.Http.dll",
                                "System.Net.HttpListener.dll",
                                "System.Net.Mail.dll",
                                "System.Net.NameResolution.dll",
                                "System.Net.NetworkInformation.dll",
                                "System.Net.Ping.dll",
                                "System.Net.Primitives.dll",
                                "System.Net.Requests.dll",
                                "System.Net.Security.dll",
                                "System.Net.ServicePoint.dll",
                                "System.Net.Sockets.dll",
                                "System.Net.WebClient.dll",
                                "System.Net.WebHeaderCollection.dll",
                                "System.Net.WebProxy.dll",
                                "System.Net.WebSockets.Client.dll",
                                "System.Net.WebSockets.dll",
                                "System.Numerics.Vectors.dll",
                                "System.ObjectModel.dll",
                                "System.Private.CoreLib.dll",
                                "System.Private.DataContractSerialization.dll",
                                "System.Private.Uri.dll",
                                "System.Private.Xml.dll",
                                "System.Private.Xml.Linq.dll",
                                "System.Reflection.Emit.dll",
                                "System.Reflection.Emit.ILGeneration.dll",
                                "System.Reflection.Emit.Lightweight.dll",
                                "System.Reflection.Metadata.dll",
                                "System.Reflection.Primitives.dll",
                                "System.Resources.ResourceManager.dll",
                                "System.Resources.Writer.dll",
                                "System.Runtime.CompilerServices.Unsafe.dll",
                                "System.Runtime.CompilerServices.VisualC.dll",
                                "System.Runtime.dll",
                                "System.Runtime.Extensions.dll",
                                "System.Runtime.InteropServices.dll",
                                "System.Runtime.InteropServices.RuntimeInformation.dll",
                                "System.Runtime.Numerics.dll",
                                "System.Runtime.Serialization.Formatters.dll",
                                "System.Runtime.Serialization.Json.dll",
                                "System.Runtime.Serialization.Primitives.dll",
                                "System.Runtime.Serialization.Xml.dll",
                                "System.Security.AccessControl.dll",
                                "System.Security.Claims.dll",
                                "System.Security.Cryptography.Algorithms.dll",
                                "System.Security.Cryptography.Cng.dll",
                                "System.Security.Cryptography.Csp.dll",
                                "System.Security.Cryptography.Encoding.dll",
                                "System.Security.Cryptography.Primitives.dll",
                                "System.Security.Cryptography.X509Certificates.dll",
                                "System.Security.Principal.dll",
                                "System.Security.Principal.Windows.dll",
                                "System.Text.Encoding.CodePages.dll",
                                "System.Text.Encoding.Extensions.dll",
                                "System.Text.RegularExpressions.dll",
                                "System.Threading.dll",
                                "System.Threading.Overlapped.dll",
                                "System.Threading.Tasks.dll",
                                "System.Threading.Tasks.Extensions.dll",
                                "System.Threading.Tasks.Parallel.dll",
                                "System.Threading.Thread.dll",
                                "System.Threading.ThreadPool.dll",
                                "System.Threading.Timer.dll",
                                "System.Transactions.Local.dll",
                                "System.Web.HttpUtility.dll",
                                "System.Xml.ReaderWriter.dll",
                                "System.Xml.XDocument.dll",
                                "System.Xml.XmlSerializer.dll",
                                "System.Xml.XPath.dll",
                                "System.Xml.XPath.XDocument.dll",
                           };
#else
            Assert.Inconclusive("Not handling this framework.");
#endif
            var actual = RoslynAssert.MetadataReferences
                                       .Select(x => Path.GetFileName(x.Display))
                                       .OrderBy(x => x)
                                       .ToArray();
            CollectionAssert.AreEqual(expected, actual);
        }

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
