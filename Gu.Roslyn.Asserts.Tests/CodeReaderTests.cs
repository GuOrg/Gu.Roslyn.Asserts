namespace Gu.Roslyn.Asserts.Tests;

using Microsoft.CodeAnalysis.Text;
using NUnit.Framework;

public static partial class CodeReaderTests
{
    [Test]
    public static void NamespaceFromClass()
    {
        var code = @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N
{
    class CodeReaderTests
    {
    }
}";
        Assert.AreEqual("N", CodeReader.Namespace(code));
    }

    [TestCase(0, 0, "↓using System;")]
    [TestCase(0, 6, "using ↓System;")]
    [TestCase(1, 0, "↓using System.Collections.Generic;")]
    [TestCase(11, 0, "↓}")]
    [TestCase(11, 1, "}↓")]
    public static void GetLineWithErrorIndicated(int line, int character, string expected)
    {
        var code = @"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace N
{
    class CodeReaderTests
    {
    }
}";
        Assert.AreEqual(expected, CodeReader.GetLineWithErrorIndicated(code, new LinePosition(line, character)));
    }
}
