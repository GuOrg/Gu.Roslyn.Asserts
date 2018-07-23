namespace Gu.Roslyn.Asserts.Tests
{
    using Microsoft.CodeAnalysis.Text;
    using NUnit.Framework;

    public partial class CodeReaderTests
    {
        public class FindLinePositions
        {
            [Test]
            public void OneErrorInClass()
            {
                var code = @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynSandbox
{
    ↓class CodeReaderTests
    {
    }
}";
                CollectionAssert.AreEqual(new[] { new LinePosition(8, 4) }, CodeReader.FindLinePositions(code));
            }

            [Test]
            public void TwoErrorsInClass()
            {
                var code = @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RoslynSandbox
{
    ↓class ↓CodeReaderTests
    {
    }
}";
                CollectionAssert.AreEqual(new[] { new LinePosition(8, 4), new LinePosition(8, 10) }, CodeReader.FindLinePositions(code));
            }
        }
    }
}
