namespace Gu.Roslyn.Asserts.Tests.MetadataReferences
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
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
        public void TransitiveCSharpCompilationFullNames()
        {
            var expected = new[]
            {
                "C:\\Git\\Gu.Roslyn.Asserts\\Gu.Roslyn.Asserts.Tests\\bin\\Debug\\net472\\Microsoft.CodeAnalysis.CSharp.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Runtime.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\mscorlib.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Core.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Configuration.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Xml.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Data.SqlXml.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Security.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Numerics.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.ComponentModel.Composition.dll",
                "C:\\Git\\Gu.Roslyn.Asserts\\Gu.Roslyn.Asserts.Tests\\bin\\Debug\\net472\\Microsoft.CodeAnalysis.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Diagnostics.Debug.dll",
                "C:\\Git\\Gu.Roslyn.Asserts\\Gu.Roslyn.Asserts.Tests\\bin\\Debug\\net472\\System.Collections.Immutable.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\netstandard.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Data.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Transactions.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.EnterpriseServices.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.DirectoryServices.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Runtime.Remoting.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Web.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Drawing.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Web.RegularExpressions.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Design.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Windows.Forms.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Accessibility.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Deployment.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Runtime.Serialization.Formatters.Soap.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Data.OracleClient.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Drawing.Design.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Web.ApplicationServices.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.ComponentModel.DataAnnotations.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.DirectoryServices.Protocols.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.ServiceProcess.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Configuration.Install.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Runtime.Serialization.dll",
                "C:\\WINDOWS\\Microsoft.Net\\assembly\\GAC_MSIL\\System.ServiceModel.Internals\\v4.0_4.0.0.0__31bf3856ad364e35\\System.ServiceModel.Internals.dll",
                "C:\\WINDOWS\\Microsoft.Net\\assembly\\GAC_MSIL\\SMDiagnostics\\v4.0_4.0.0.0__b77a5c561934e089\\SMDiagnostics.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Web.Services.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Microsoft.Build.Utilities.v4.0.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Microsoft.Build.Framework.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Xaml.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Runtime.Caching.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Microsoft.Build.Tasks.v4.0.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Data.Common.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Diagnostics.StackTrace.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Diagnostics.Tracing.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Globalization.Extensions.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.IO.Compression.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.IO.Compression.FileSystem.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Net.Http.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Net.Sockets.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.ValueTuple.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Runtime.InteropServices.RuntimeInformation.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Runtime.Serialization.Xml.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Runtime.Serialization.Primitives.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Security.Cryptography.Algorithms.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Security.SecureString.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Threading.Overlapped.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Xml.Linq.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Xml.XPath.XDocument.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Collections.dll",
                "C:\\Git\\Gu.Roslyn.Asserts\\Gu.Roslyn.Asserts.Tests\\bin\\Debug\\net472\\System.Reflection.Metadata.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Globalization.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Reflection.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.IO.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.IO.FileSystem.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Runtime.Extensions.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Collections.Concurrent.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Text.Encoding.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Linq.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Threading.Tasks.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Threading.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Xml.XDocument.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Security.Cryptography.Primitives.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Runtime.InteropServices.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Reflection.Primitives.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Diagnostics.Tools.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Resources.ResourceManager.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.IO.FileSystem.Primitives.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Xml.ReaderWriter.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Runtime.Numerics.dll",
                "C:\\Git\\Gu.Roslyn.Asserts\\Gu.Roslyn.Asserts.Tests\\bin\\Debug\\net472\\System.Threading.Tasks.Extensions.dll",
                "C:\\Git\\Gu.Roslyn.Asserts\\Gu.Roslyn.Asserts.Tests\\bin\\Debug\\net472\\System.Runtime.CompilerServices.Unsafe.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Reflection.Extensions.dll",
                "C:\\Git\\Gu.Roslyn.Asserts\\Gu.Roslyn.Asserts.Tests\\bin\\Debug\\net472\\System.Text.Encoding.CodePages.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Text.Encoding.Extensions.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Linq.Expressions.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Threading.Tasks.Parallel.dll",
            };
            var type = typeof(CSharpCompilation);
            CollectionAssert.AreEqual(expected, Gu.Roslyn.Asserts.MetadataReferences.Transitive(type).Select(x => x.Display));
            CollectionAssert.AreEqual(expected, Gu.Roslyn.Asserts.MetadataReferences.Transitive(type.Assembly).Select(x => x.Display));
        }

        [Test]
        public void ManyTransitiveFullNames()
        {
            var expected = new[]
            {
                "C:\\Git\\Gu.Roslyn.Asserts\\Gu.Roslyn.Asserts.Tests\\bin\\Debug\\net472\\Microsoft.CodeAnalysis.CSharp.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Runtime.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\mscorlib.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Core.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Configuration.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Xml.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Data.SqlXml.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Security.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Numerics.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.ComponentModel.Composition.dll",
                "C:\\Git\\Gu.Roslyn.Asserts\\Gu.Roslyn.Asserts.Tests\\bin\\Debug\\net472\\Microsoft.CodeAnalysis.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Diagnostics.Debug.dll",
                "C:\\Git\\Gu.Roslyn.Asserts\\Gu.Roslyn.Asserts.Tests\\bin\\Debug\\net472\\System.Collections.Immutable.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\netstandard.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Data.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Transactions.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.EnterpriseServices.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.DirectoryServices.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Runtime.Remoting.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Web.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Drawing.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Web.RegularExpressions.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Design.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Windows.Forms.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Accessibility.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Deployment.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Runtime.Serialization.Formatters.Soap.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Data.OracleClient.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Drawing.Design.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Web.ApplicationServices.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.ComponentModel.DataAnnotations.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.DirectoryServices.Protocols.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.ServiceProcess.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Configuration.Install.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Runtime.Serialization.dll",
                "C:\\WINDOWS\\Microsoft.Net\\assembly\\GAC_MSIL\\System.ServiceModel.Internals\\v4.0_4.0.0.0__31bf3856ad364e35\\System.ServiceModel.Internals.dll",
                "C:\\WINDOWS\\Microsoft.Net\\assembly\\GAC_MSIL\\SMDiagnostics\\v4.0_4.0.0.0__b77a5c561934e089\\SMDiagnostics.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Web.Services.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Microsoft.Build.Utilities.v4.0.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Microsoft.Build.Framework.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Xaml.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Runtime.Caching.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Microsoft.Build.Tasks.v4.0.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Data.Common.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Diagnostics.StackTrace.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Diagnostics.Tracing.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Globalization.Extensions.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.IO.Compression.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.IO.Compression.FileSystem.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Net.Http.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Net.Sockets.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.ValueTuple.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Runtime.InteropServices.RuntimeInformation.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Runtime.Serialization.Xml.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Runtime.Serialization.Primitives.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Security.Cryptography.Algorithms.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Security.SecureString.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Threading.Overlapped.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\System.Xml.Linq.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Xml.XPath.XDocument.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Collections.dll",
                "C:\\Git\\Gu.Roslyn.Asserts\\Gu.Roslyn.Asserts.Tests\\bin\\Debug\\net472\\System.Reflection.Metadata.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Globalization.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Reflection.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.IO.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.IO.FileSystem.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Runtime.Extensions.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Collections.Concurrent.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Text.Encoding.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Linq.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Threading.Tasks.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Threading.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Xml.XDocument.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Security.Cryptography.Primitives.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Runtime.InteropServices.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Reflection.Primitives.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Diagnostics.Tools.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Resources.ResourceManager.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.IO.FileSystem.Primitives.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Xml.ReaderWriter.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Runtime.Numerics.dll",
                "C:\\Git\\Gu.Roslyn.Asserts\\Gu.Roslyn.Asserts.Tests\\bin\\Debug\\net472\\System.Threading.Tasks.Extensions.dll",
                "C:\\Git\\Gu.Roslyn.Asserts\\Gu.Roslyn.Asserts.Tests\\bin\\Debug\\net472\\System.Runtime.CompilerServices.Unsafe.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Reflection.Extensions.dll",
                "C:\\Git\\Gu.Roslyn.Asserts\\Gu.Roslyn.Asserts.Tests\\bin\\Debug\\net472\\System.Text.Encoding.CodePages.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Text.Encoding.Extensions.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Linq.Expressions.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Threading.Tasks.Parallel.dll",
                "C:\\Git\\Gu.Roslyn.Asserts\\Gu.Roslyn.Asserts.Tests\\bin\\Debug\\net472\\Microsoft.CodeAnalysis.Workspaces.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Diagnostics.Contracts.dll",
                "C:\\Git\\Gu.Roslyn.Asserts\\Gu.Roslyn.Asserts.Tests\\bin\\Debug\\net472\\System.Composition.AttributedModel.dll",
                "C:\\Git\\Gu.Roslyn.Asserts\\Gu.Roslyn.Asserts.Tests\\bin\\Debug\\net472\\System.Composition.Runtime.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Text.RegularExpressions.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Threading.Thread.dll",
                "C:\\Git\\Gu.Roslyn.Asserts\\Gu.Roslyn.Asserts.Tests\\bin\\Debug\\net472\\System.Composition.TypedParts.dll",
                "C:\\Git\\Gu.Roslyn.Asserts\\Gu.Roslyn.Asserts.Tests\\bin\\Debug\\net472\\System.Composition.Hosting.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.ObjectModel.dll",
                "C:\\Program Files (x86)\\Reference Assemblies\\Microsoft\\Framework\\.NETFramework\\v4.7.2\\Facades\\System.Linq.Parallel.dll",
            };
            IEnumerable<MetadataReference> metadataReferences = Gu.Roslyn.Asserts.MetadataReferences.Transitive(
                typeof(Microsoft.CodeAnalysis.CSharp.CSharpCompilation),
                typeof(Microsoft.CodeAnalysis.CodeFixes.CodeFixProvider),
                typeof(System.Runtime.CompilerServices.InternalsVisibleToAttribute));
            CollectionAssert.AreEqual(expected, metadataReferences.Select(x => x.Display));
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
