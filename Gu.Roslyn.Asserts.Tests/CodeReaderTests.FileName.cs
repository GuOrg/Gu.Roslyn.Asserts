namespace Gu.Roslyn.Asserts.Tests
{
    using NUnit.Framework;

    public partial class CodeReaderTests
    {
        public class FileName
        {
            [TestCase("CodeReaderTests", "CodeReaderTests.cs")]
            [TestCase("CodeReaderTests<T>", "CodeReaderTests{T}.cs")]
            [TestCase("CodeReaderTests<T1, T2>", "CodeReaderTests{T1,T2}.cs")]
            public void FromClass(string className, string expected)
            {
                var code = @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gu.Roslyn.Asserts.Tests
{
    class CodeReaderTests
    {
    }
}";
                code = code.AssertReplace("CodeReaderTests", className);
                Assert.AreEqual(expected, CodeReader.FileName(code));
            }

            [TestCase("IFoo", "IFoo.cs")]
            [TestCase("IFoo<T>", "IFoo{T}.cs")]
            [TestCase("IFoo<T1, T2>", "IFoo{T1,T2}.cs")]
            public void FromInterface(string className, string expected)
            {
                var code = @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gu.Roslyn.Asserts.Tests
{
    interface IFoo
    {
    }
}";
                code = code.AssertReplace("IFoo", className);
                Assert.AreEqual(expected, CodeReader.FileName(code));
            }

            [Test]
            public void FromEnum()
            {
                var code = @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gu.Roslyn.Asserts.Tests
{
    enum Foo
    {
    }
}";
                Assert.AreEqual("Foo.cs", CodeReader.FileName(code));
            }

            [Test]
            public void FromAssemblyInfo()
            {
                var code = @"using System.Reflection;
using System.Runtime.InteropServices;

[assembly: AssemblyTitle(""Gu.Roslyn.Asserts.Tests"")]
[assembly: AssemblyDescription("""")]
[assembly: AssemblyConfiguration("""")]
[assembly: AssemblyCompany("""")]
[assembly: AssemblyProduct(""Gu.Roslyn.Asserts.Tests"")]
[assembly: AssemblyCopyright(""Copyright ©  2017"")]
[assembly: AssemblyTrademark("""")]
[assembly: AssemblyCulture("""")]

[assembly: ComVisible(false)]

[assembly: Guid(""34e6a491-5742-40e1-b1bf-f210d43dee1b"")]

// [assembly: AssemblyVersion(""1.0.*"")]
[assembly: AssemblyVersion(""1.0.0.0"")]
[assembly: AssemblyFileVersion(""1.0.0.0"")]
";

                Assert.AreEqual("AssemblyInfo.cs", CodeReader.FileName(code));
            }
        }
    }
}