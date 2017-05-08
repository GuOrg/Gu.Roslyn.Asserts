namespace Gu.Roslyn.Asserts.Tests
{
    using Microsoft.CodeAnalysis.Text;
    using NUnit.Framework;

    public partial class CodeReaderTests
    {
        public class FindDiagnosticsPositions
        {
            [Test]
            public void OneErrorInClass()
            {
                var code = @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gu.Roslyn.Asserts.Tests
{
    ↓class CodeReaderTests
    {
    }
}";
                CollectionAssert.AreEqual(new[] { new LinePosition(9, 5) }, CodeReader.FindDiagnosticsPositions(code));
            }

            [Test]
            public void TwoErrorsInClass()
            {
                var code = @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gu.Roslyn.Asserts.Tests
{
    ↓class ↓CodeReaderTests
    {
    }
}";
                CollectionAssert.AreEqual(new[] { new LinePosition(9, 5), new LinePosition(9, 11) }, CodeReader.FindDiagnosticsPositions(code));
            }
        }
    }
}