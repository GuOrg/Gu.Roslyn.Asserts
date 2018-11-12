namespace Gu.Roslyn.Asserts.Tests.MetadataReferences
{
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
            var expected = new[] { "mscorlib.dll" };
            var type = typeof(object);
            CollectionAssert.AreEqual(expected, Gu.Roslyn.Asserts.MetadataReferences.Transitive(type).Select(x => Path.GetFileName(x.Display)));
            CollectionAssert.AreEqual(expected, Gu.Roslyn.Asserts.MetadataReferences.Transitive(type.Assembly).Select(x => Path.GetFileName(x.Display)));
        }

        [Test]
        public void TransitiveMscorlibFullNames()
        {
            var expected = new[] { "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\mscorlib.dll" };
            var type = typeof(object);
            CollectionAssert.AreEqual(expected, Gu.Roslyn.Asserts.MetadataReferences.Transitive(type).Select(x => x.Display).ToArray());
            CollectionAssert.AreEqual(expected, Gu.Roslyn.Asserts.MetadataReferences.Transitive(type.Assembly).Select(x => x.Display).ToArray());
        }

        [Test]
        public void TransitiveSystemCore()
        {
            var expected = new[]
                           {
                               "System.Core.dll",
                               "mscorlib.dll",
                               "System.dll",
                               "System.Configuration.dll",
                               "System.Xml.dll",
                               "System.Data.SqlXml.dll",
                               "System.Security.dll",
                               "System.Numerics.dll",
                           };
            var type = typeof(Enumerable);
            CollectionAssert.AreEqual(expected, Gu.Roslyn.Asserts.MetadataReferences.Transitive(type).Select(x => Path.GetFileName(x.Display)));
            CollectionAssert.AreEqual(expected, Gu.Roslyn.Asserts.MetadataReferences.Transitive(type.Assembly).Select(x => Path.GetFileName(x.Display)));
        }

        [Test]
        public void TransitiveGeneric()
        {
            var expected = new[]
            {
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\mscorlib.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Xml.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Configuration.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Security.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Core.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Numerics.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Data.SqlXml.dll",
            };
            var type = typeof(List<XmlReader>);
            CollectionAssert.AreEqual(expected, Gu.Roslyn.Asserts.MetadataReferences.Transitive(type).Select(x => x.Display));
        }

        [Test]
        public void TransitiveSystemCoreFullNames()
        {
            var expected = new[]
            {
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Core.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\mscorlib.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Configuration.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Xml.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Data.SqlXml.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Security.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Numerics.dll",
            };
            var type = typeof(Enumerable);
            CollectionAssert.AreEqual(expected, Gu.Roslyn.Asserts.MetadataReferences.Transitive(type).Select(x => x.Display));
            CollectionAssert.AreEqual(expected, Gu.Roslyn.Asserts.MetadataReferences.Transitive(type.Assembly).Select(x => x.Display));
        }

        [Test]
        public void TransitiveCSharpCompilation()
        {
            var expected = new[]
            {
                "Microsoft.CodeAnalysis.CSharp.dll",
                "System.Runtime.dll",
                "mscorlib.dll",
                "System.Core.dll",
                "System.dll",
                "System.Configuration.dll",
                "System.Xml.dll",
                "System.Data.SqlXml.dll",
                "System.Security.dll",
                "System.Numerics.dll",
                "System.ComponentModel.Composition.dll",
                "Microsoft.CodeAnalysis.dll",
                "System.Diagnostics.Debug.dll",
                "System.Collections.Immutable.dll",
                "netstandard.dll",
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
                "System.Runtime.Caching.dll",
                "Microsoft.Build.Tasks.v4.0.dll",
                "System.Data.Common.dll",
                "System.Diagnostics.StackTrace.dll",
                "System.Diagnostics.Tracing.dll",
                "System.Globalization.Extensions.dll",
                "System.IO.Compression.dll",
                "System.IO.Compression.FileSystem.dll",
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
                "System.Collections.dll",
                "System.Reflection.Metadata.dll",
                "System.Globalization.dll",
                "System.Reflection.dll",
                "System.IO.dll",
                "System.IO.FileSystem.dll",
                "System.Runtime.Extensions.dll",
                "System.Collections.Concurrent.dll",
                "System.Text.Encoding.dll",
                "System.Linq.dll",
                "System.Threading.Tasks.dll",
                "System.Threading.dll",
                "System.Xml.XDocument.dll",
                "System.Security.Cryptography.Primitives.dll",
                "System.Runtime.InteropServices.dll",
                "System.Reflection.Primitives.dll",
                "System.Diagnostics.Tools.dll",
                "System.Resources.ResourceManager.dll",
                "System.IO.FileSystem.Primitives.dll",
                "System.Xml.ReaderWriter.dll",
                "System.Runtime.Numerics.dll",
                "System.Threading.Tasks.Extensions.dll",
                "System.Runtime.CompilerServices.Unsafe.dll",
                "System.Reflection.Extensions.dll",
                "System.Text.Encoding.CodePages.dll",
                "System.Text.Encoding.Extensions.dll",
                "System.Linq.Expressions.dll",
                "System.Threading.Tasks.Parallel.dll",
            };
            var type = typeof(CSharpCompilation);
            CollectionAssert.AreEqual(expected, Gu.Roslyn.Asserts.MetadataReferences.Transitive(type).Select(x => Path.GetFileName(x.Display)));
            CollectionAssert.AreEqual(expected, Gu.Roslyn.Asserts.MetadataReferences.Transitive(type.Assembly).Select(x => Path.GetFileName(x.Display)));
        }

        [Test]
        public void ManyTransitiveFullNames()
        {
            var expected = new[]
            {

                "Microsoft.CodeAnalysis.CSharp.dll",
                "System.Runtime.dll",
                "mscorlib.dll",
                "System.Core.dll",
                "System.dll",
                "System.Configuration.dll",
                "System.Xml.dll",
                "System.Data.SqlXml.dll",
                "System.Security.dll",
                "System.Numerics.dll",
                "System.ComponentModel.Composition.dll",
                "Microsoft.CodeAnalysis.dll",
                "System.Diagnostics.Debug.dll",
                "System.Collections.Immutable.dll",
                "netstandard.dll",
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
                "System.Runtime.Caching.dll",
                "Microsoft.Build.Tasks.v4.0.dll",
                "System.Data.Common.dll",
                "System.Diagnostics.StackTrace.dll",
                "System.Diagnostics.Tracing.dll",
                "System.Globalization.Extensions.dll",
                "System.IO.Compression.dll",
                "System.IO.Compression.FileSystem.dll",
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
                "System.Collections.dll",
                "System.Reflection.Metadata.dll",
                "System.Globalization.dll",
                "System.Reflection.dll",
                "System.IO.dll",
                "System.IO.FileSystem.dll",
                "System.Runtime.Extensions.dll",
                "System.Collections.Concurrent.dll",
                "System.Text.Encoding.dll",
                "System.Linq.dll",
                "System.Threading.Tasks.dll",
                "System.Threading.dll",
                "System.Xml.XDocument.dll",
                "System.Security.Cryptography.Primitives.dll",
                "System.Runtime.InteropServices.dll",
                "System.Reflection.Primitives.dll",
                "System.Diagnostics.Tools.dll",
                "System.Resources.ResourceManager.dll",
                "System.IO.FileSystem.Primitives.dll",
                "System.Xml.ReaderWriter.dll",
                "System.Runtime.Numerics.dll",
                "System.Threading.Tasks.Extensions.dll",
                "System.Runtime.CompilerServices.Unsafe.dll",
                "System.Reflection.Extensions.dll",
                "System.Text.Encoding.CodePages.dll",
                "System.Text.Encoding.Extensions.dll",
                "System.Linq.Expressions.dll",
                "System.Threading.Tasks.Parallel.dll",
                "Microsoft.CodeAnalysis.Workspaces.dll",
                "System.Diagnostics.Contracts.dll",
                "System.Composition.AttributedModel.dll",
                "System.Composition.Runtime.dll",
                "System.Text.RegularExpressions.dll",
                "System.Threading.Thread.dll",
                "System.Composition.TypedParts.dll",
                "System.Composition.Hosting.dll",
                "System.ObjectModel.dll",
                "System.Linq.Parallel.dll",
            };
            var metadataReferences = Gu.Roslyn.Asserts.MetadataReferences.Transitive(
                typeof(CSharpCompilation),
                typeof(Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider),
                typeof(System.Runtime.CompilerServices.InternalsVisibleToAttribute));
            CollectionAssert.AreEqual(expected, metadataReferences.Select(x => Path.GetFileName(x.Display)));
        }

        // ReSharper disable once UnusedMember.Local
        private static void Dump(IEnumerable<MetadataReference> references, bool fullname)
        {
            foreach (var metadataReference in references)
            {
                if (fullname)
                {
                    Console.WriteLine($"\"{metadataReference.Display.Replace("\\", "\\\\")}\",");
                }
                else
                {
                    Console.WriteLine($"\"{Path.GetFileName(metadataReference.Display)}\",");
                }
            }
        }
    }
}
