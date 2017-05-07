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

namespace Gu.Roslyn.Asserts.Tests
{
    class CodeReaderTests
    {
    }
}";
            Assert.AreEqual("Gu.Roslyn.Asserts.Tests", CodeReader.Namespace(code));
        }
    }
}
