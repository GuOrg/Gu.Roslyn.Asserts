namespace Gu.Roslyn.Asserts.Tests
{
    using Microsoft.CodeAnalysis.Text;
    using NUnit.Framework;

    public static partial class CodeReaderTests
    {
        public static class FindLinePositions
        {
            [Test]
            public static void OneErrorInClass()
            {
                var code = @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N
{
    ↓class CodeReaderTests
    {
    }
}";
                CollectionAssert.AreEqual(new[] { new LinePosition(8, 4) }, CodeReader.FindLinePositions(code));
            }

            [Test]
            public static void TwoErrorsInClass()
            {
                var code = @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N
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
