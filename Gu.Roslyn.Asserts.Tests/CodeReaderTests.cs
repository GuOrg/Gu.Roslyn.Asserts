namespace Gu.Roslyn.Asserts.Tests
{
    using NUnit.Framework;

    public partial class CodeReaderTests
    {
        [Test]
        public void NamespaceFromClass()
        {
            var code = @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynSandbox
{
    class CodeReaderTests
    {
    }
}";
            Assert.AreEqual("RoslynSandbox", CodeReader.Namespace(code));
        }
    }
}
