namespace Gu.Roslyn.Asserts.Tests.MetadataReferences;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using NUnit.Framework;

public class MetadataReferencesTests
{
    [Test]
    public void TransitiveMscorlib()
    {
        var expected = new[] { "System.Private.CoreLib.dll" };
        var type = typeof(object);
        CollectionAssert.AreEqual(expected, Gu.Roslyn.Asserts.MetadataReferences.Transitive(type).Select(x => Path.GetFileName(x.Display)));
        CollectionAssert.AreEqual(expected, Gu.Roslyn.Asserts.MetadataReferences.Transitive(type.Assembly).Select(x => Path.GetFileName(x.Display)));
    }

    [Explicit("Depends on what is installed.")]
    [Test]
    public void TransitiveMscorlibFullNames()
    {
        Assert.Inconclusive("VS does not understand [Explicit]");
        var expected = new[] { "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Private.CoreLib.dll" };
        var type = typeof(object);
        CollectionAssert.AreEqual(expected, Gu.Roslyn.Asserts.MetadataReferences.Transitive(type).Select(x => x.Display).ToArray());
        CollectionAssert.AreEqual(expected, Gu.Roslyn.Asserts.MetadataReferences.Transitive(type.Assembly).Select(x => x.Display).ToArray());
    }

    [Test]
    public void TransitiveSystemCore()
    {
        var expected = new[]
        {
            "System.Linq.dll",
            "System.Runtime.dll",
            "System.Private.CoreLib.dll",
            "System.Private.Uri.dll",
            "System.Collections.dll",
            "System.Memory.dll",
        };
        var type = typeof(Enumerable);
        CollectionAssert.AreEqual(expected, Gu.Roslyn.Asserts.MetadataReferences.Transitive(type).Select(x => Path.GetFileName(x.Display)));
        CollectionAssert.AreEqual(expected, Gu.Roslyn.Asserts.MetadataReferences.Transitive(type.Assembly).Select(x => Path.GetFileName(x.Display)));
    }

    [Explicit("Depends on what is installed.")]
    [Test]
    public void TransitiveGenericFullNames()
    {
        Assert.Inconclusive("VS does not understand [Explicit]");
        var expected = new[]
        {
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Private.CoreLib.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Private.Xml.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Runtime.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Private.Uri.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Resources.ResourceManager.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Runtime.InteropServices.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Runtime.Extensions.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Security.Principal.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Collections.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Diagnostics.Debug.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Threading.Tasks.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Diagnostics.TraceSource.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Threading.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Collections.NonGeneric.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Collections.Specialized.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.IO.FileSystem.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Threading.Overlapped.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Text.Encoding.Extensions.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Buffers.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Text.RegularExpressions.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Net.Primitives.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Diagnostics.Tracing.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\Microsoft.Win32.Primitives.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Net.Requests.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Net.WebHeaderCollection.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Net.Http.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Security.Cryptography.X509Certificates.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Security.Cryptography.Cng.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Security.Cryptography.Encoding.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Security.Cryptography.Primitives.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Collections.Concurrent.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Linq.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Diagnostics.Tools.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Security.Cryptography.Algorithms.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Runtime.Numerics.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Security.Cryptography.Csp.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Threading.Thread.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Diagnostics.DiagnosticSource.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.IO.Compression.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Net.ServicePoint.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Net.Security.dll",
            "C:\\Users\\ds2346\\.nuget\\packages\\system.security.principal.windows\\4.5.0\\runtimes\\win\\lib\\netcoreapp2.0\\System.Security.Principal.Windows.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Security.Claims.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Threading.ThreadPool.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Net.Sockets.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Net.NameResolution.dll",
            "C:\\Users\\ds2346\\.nuget\\packages\\system.threading.tasks.extensions\\4.5.2\\lib\\netstandard2.0\\System.Threading.Tasks.Extensions.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\netstandard.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.IO.MemoryMappedFiles.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.IO.Pipes.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Diagnostics.Process.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.ComponentModel.Primitives.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.ComponentModel.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Diagnostics.FileVersionInfo.dll",
            "C:\\Users\\ds2346\\.nuget\\packages\\microsoft.win32.registry\\4.5.0\\runtimes\\win\\lib\\netstandard2.0\\Microsoft.Win32.Registry.dll",
            "C:\\Users\\ds2346\\.nuget\\packages\\system.memory\\4.5.3\\lib\\netstandard2.0\\System.Memory.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Numerics.Vectors.dll",
            "C:\\Users\\ds2346\\.nuget\\packages\\system.runtime.compilerservices.unsafe\\4.5.2\\lib\\netcoreapp2.0\\System.Runtime.CompilerServices.Unsafe.dll",
            "C:\\Users\\ds2346\\.nuget\\packages\\system.security.accesscontrol\\4.5.0\\runtimes\\win\\lib\\netcoreapp2.0\\System.Security.AccessControl.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.ComponentModel.TypeConverter.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Drawing.Primitives.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Threading.Timer.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.ObjectModel.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Runtime.Serialization.Formatters.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Resources.Writer.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.ComponentModel.EventBasedAsync.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Console.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Data.Common.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Xml.ReaderWriter.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Xml.XmlSerializer.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Transactions.Local.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Linq.Expressions.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Reflection.Emit.ILGeneration.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Reflection.Emit.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Reflection.Primitives.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Reflection.Emit.Lightweight.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Diagnostics.Contracts.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Diagnostics.TextWriterTraceListener.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Diagnostics.StackTrace.dll",
            "C:\\Users\\ds2346\\.nuget\\packages\\system.reflection.metadata\\1.6.0\\lib\\netstandard2.0\\System.Reflection.Metadata.dll",
            "C:\\Users\\ds2346\\.nuget\\packages\\system.collections.immutable\\1.5.0\\lib\\netstandard2.0\\System.Collections.Immutable.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.IO.Compression.ZipFile.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.IO.FileSystem.DriveInfo.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.IO.FileSystem.Watcher.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.IO.IsolatedStorage.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.IO.FileSystem.AccessControl.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Linq.Queryable.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Linq.Parallel.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Net.HttpListener.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Net.WebSockets.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Net.WebClient.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Net.WebProxy.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Net.NetworkInformation.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Net.Mail.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Net.Ping.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Net.WebSockets.Client.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Runtime.CompilerServices.VisualC.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Runtime.InteropServices.RuntimeInformation.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Runtime.Serialization.Primitives.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Runtime.Serialization.Xml.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Private.DataContractSerialization.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Xml.XDocument.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Private.Xml.Linq.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Runtime.Serialization.Json.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Threading.Tasks.Parallel.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Web.HttpUtility.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Xml.XPath.XDocument.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Xml.XPath.dll",
        };
        var type = typeof(List<XmlReader>);
        CollectionAssert.AreEqual(expected, Gu.Roslyn.Asserts.MetadataReferences.Transitive(type).Select(x => x.Display));
    }

    [Explicit("Depends on what is installed.")]
    [Test]
    public void TransitiveSystemCoreFullNames()
    {
        Assert.Inconclusive("VS does not understand [Explicit]");
        var expected = new[]
        {
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Linq.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Runtime.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Private.CoreLib.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Private.Uri.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Resources.ResourceManager.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Collections.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Diagnostics.Debug.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Diagnostics.Tools.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Runtime.Extensions.dll",
            "C:\\Program Files\\dotnet\\shared\\Microsoft.NETCore.App\\2.0.9\\System.Security.Principal.dll",
        };
        var type = typeof(Enumerable);
        CollectionAssert.AreEqual(expected, Gu.Roslyn.Asserts.MetadataReferences.Transitive(type).Select(x => x.Display));
        CollectionAssert.AreEqual(expected, Gu.Roslyn.Asserts.MetadataReferences.Transitive(type.Assembly).Select(x => x.Display));
    }

    [Explicit("fragile")]
    [Test]
    public void TransitiveCSharpCompilation()
    {
        var expected = new[]
        {
            "Microsoft.CodeAnalysis.CSharp.dll",
            "System.Runtime.dll",
            "System.Private.CoreLib.dll",
            "System.Private.Uri.dll",
            "Microsoft.CodeAnalysis.dll",
            "System.Resources.ResourceManager.dll",
            "System.Runtime.Extensions.dll",
            "System.Diagnostics.Debug.dll",
            "System.Collections.Immutable.dll",
            "System.Runtime.InteropServices.dll",
            "System.Linq.dll",
            "System.Collections.dll",
            "System.Memory.dll",
            "System.Threading.dll",
            "System.Runtime.CompilerServices.Unsafe.dll",
            "System.Reflection.Metadata.dll",
            "System.IO.Compression.dll",
            "System.Text.Encoding.Extensions.dll",
            "System.IO.MemoryMappedFiles.dll",
            "System.Threading.Thread.dll",
            "System.Collections.Concurrent.dll",
            "System.Threading.ThreadPool.dll",
            "System.Security.Cryptography.Algorithms.dll",
            "System.Security.Cryptography.Encoding.dll",
            "System.Security.Cryptography.Primitives.dll",
            "System.Formats.Asn1.dll",
            "System.Runtime.Numerics.dll",
            "System.Threading.Tasks.Parallel.dll",
            "System.Diagnostics.Tracing.dll",
            "System.Threading.Tasks.dll",
            "System.Xml.XDocument.dll",
            "System.Private.Xml.Linq.dll",
            "System.Private.Xml.dll",
            "System.Diagnostics.TraceSource.dll",
            "System.Collections.NonGeneric.dll",
            "System.Collections.Specialized.dll",
            "System.ComponentModel.Primitives.dll",
            "System.ComponentModel.dll",
            "System.ObjectModel.dll",
            "System.Text.RegularExpressions.dll",
            "System.Reflection.Emit.ILGeneration.dll",
            "System.Reflection.Emit.Lightweight.dll",
            "System.Reflection.Primitives.dll",
            "System.Net.Primitives.dll",
            "Microsoft.Win32.Primitives.dll",
            "System.Net.Http.dll",
            "System.Net.Security.dll",
            "System.Security.Cryptography.X509Certificates.dll",
            "System.Security.Cryptography.Cng.dll",
            "System.Security.Cryptography.Csp.dll",
            "System.Security.Principal.Windows.dll",
            "System.Security.Claims.dll",
            "System.Net.Sockets.dll",
            "System.Threading.Overlapped.dll",
            "System.Net.NameResolution.dll",
            "System.Net.Quic.dll",
            "System.Threading.Channels.dll",
            "System.IO.Compression.Brotli.dll",
            "System.Net.NetworkInformation.dll",
            "System.Diagnostics.DiagnosticSource.dll",
            "System.Reflection.Emit.dll",
            "System.Linq.Expressions.dll",
            "System.IO.FileSystem.dll",
            "System.Xml.ReaderWriter.dll",
            "System.Runtime.Loader.dll",
            "System.Runtime.Serialization.Primitives.dll",
            "System.Diagnostics.StackTrace.dll",
            "System.Console.dll",
            "System.Xml.XPath.XDocument.dll",
            "System.Text.Encoding.CodePages.dll",
        };
        var type = typeof(CSharpCompilation);
        CollectionAssert.AreEqual(expected, Gu.Roslyn.Asserts.MetadataReferences.Transitive(type).Select(x => Path.GetFileName(x.Display)));
        CollectionAssert.AreEqual(expected, Gu.Roslyn.Asserts.MetadataReferences.Transitive(type.Assembly).Select(x => Path.GetFileName(x.Display)));
    }

    [Explicit("fragile")]
    [Test]
    public void ManyTransitive()
    {
        var expected = new[]
        {
            "Microsoft.CodeAnalysis.CSharp.dll",
            "System.Runtime.dll",
            "System.Private.CoreLib.dll",
            "System.Private.Uri.dll",
            "Microsoft.CodeAnalysis.dll",
            "System.Resources.ResourceManager.dll",
            "System.Runtime.Extensions.dll",
            "System.Diagnostics.Debug.dll",
            "System.Collections.Immutable.dll",
            "System.Runtime.InteropServices.dll",
            "System.Linq.dll",
            "System.Collections.dll",
            "System.Memory.dll",
            "System.Threading.dll",
            "System.Runtime.CompilerServices.Unsafe.dll",
            "System.Reflection.Metadata.dll",
            "System.IO.Compression.dll",
            "System.Text.Encoding.Extensions.dll",
            "System.IO.MemoryMappedFiles.dll",
            "System.Threading.Thread.dll",
            "System.Collections.Concurrent.dll",
            "System.Threading.ThreadPool.dll",
            "System.Security.Cryptography.Algorithms.dll",
            "System.Security.Cryptography.Encoding.dll",
            "System.Security.Cryptography.Primitives.dll",
            "System.Formats.Asn1.dll",
            "System.Runtime.Numerics.dll",
            "System.Threading.Tasks.Parallel.dll",
            "System.Diagnostics.Tracing.dll",
            "System.Threading.Tasks.dll",
            "System.Xml.XDocument.dll",
            "System.Private.Xml.Linq.dll",
            "System.Private.Xml.dll",
            "System.Diagnostics.TraceSource.dll",
            "System.Collections.NonGeneric.dll",
            "System.Collections.Specialized.dll",
            "System.ComponentModel.Primitives.dll",
            "System.ComponentModel.dll",
            "System.ObjectModel.dll",
            "System.Text.RegularExpressions.dll",
            "System.Reflection.Emit.ILGeneration.dll",
            "System.Reflection.Emit.Lightweight.dll",
            "System.Reflection.Primitives.dll",
            "System.Net.Primitives.dll",
            "Microsoft.Win32.Primitives.dll",
            "System.Net.Http.dll",
            "System.Net.Security.dll",
            "System.Security.Cryptography.X509Certificates.dll",
            "System.Security.Cryptography.Cng.dll",
            "System.Security.Cryptography.Csp.dll",
            "System.Security.Principal.Windows.dll",
            "System.Security.Claims.dll",
            "System.Net.Sockets.dll",
            "System.Threading.Overlapped.dll",
            "System.Net.NameResolution.dll",
            "System.Net.Quic.dll",
            "System.Threading.Channels.dll",
            "System.IO.Compression.Brotli.dll",
            "System.Net.NetworkInformation.dll",
            "System.Diagnostics.DiagnosticSource.dll",
            "System.Reflection.Emit.dll",
            "System.Linq.Expressions.dll",
            "System.IO.FileSystem.dll",
            "System.Xml.ReaderWriter.dll",
            "System.Runtime.Loader.dll",
            "System.Runtime.Serialization.Primitives.dll",
            "System.Diagnostics.StackTrace.dll",
            "System.Console.dll",
            "System.Xml.XPath.XDocument.dll",
            "System.Text.Encoding.CodePages.dll",
            "Microsoft.CodeAnalysis.Workspaces.dll",
            "System.Diagnostics.Contracts.dll",
            "System.Composition.AttributedModel.dll",
            "System.Reflection.dll",
            "System.Xml.XPath.dll",
            "System.IO.Pipelines.dll",
            "System.Buffers.dll",
            "System.Diagnostics.FileVersionInfo.dll",
            "System.Composition.Runtime.dll",
            "System.Diagnostics.Tools.dll",
            "System.Globalization.dll",
            "System.Runtime.InteropServices.RuntimeInformation.dll",
            "Humanizer.dll",
            "System.Reflection.Extensions.dll",
            "System.Composition.TypedParts.dll",
            "System.Composition.Hosting.dll",
        };
        var metadataReferences = Gu.Roslyn.Asserts.MetadataReferences.Transitive(
            typeof(CSharpCompilation),
            typeof(Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider),
            typeof(System.Runtime.CompilerServices.InternalsVisibleToAttribute));
        CollectionAssert.AreEqual(expected, metadataReferences.Select(x => Path.GetFileName(x.Display)));
    }

#pragma warning disable IDE0051 // Remove unused private members
    [Obsolete("Temp use only.")]
    private static void Dump(IEnumerable<MetadataReference> references, bool fullname)
#pragma warning restore IDE0051 // Remove unused private members
    {
        foreach (var metadataReference in references)
        {
            if (fullname)
            {
                Console.WriteLine($"                \"{metadataReference.Display.Replace("\\", "\\\\")}\",");
            }
            else
            {
                Console.WriteLine($"                \"{Path.GetFileName(metadataReference.Display)}\",");
            }
        }
    }
}
