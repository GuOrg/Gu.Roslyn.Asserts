// ReSharper disable AssignNullToNotNullAttribute
// ReSharper disable PossibleNullReferenceException
namespace Gu.Roslyn.Asserts.Tests
{
    using System.IO;
    using System.Reflection;
    using NUnit.Framework;

    public static partial class CodeFactoryTests
    {
        public static class TryFindFileInParentDirectory
        {
            private static readonly FileInfo ExecutingAssemblyDll = new(Assembly.GetExecutingAssembly().Location);

            [Test]
            public static void TryFindProjectFileInParentDirectory()
            {
                var directory = ExecutingAssemblyDll.Directory;
                var projectFileName = Path.GetFileNameWithoutExtension(ExecutingAssemblyDll.FullName) + ".csproj";
                Assert.AreEqual(true, CodeFactory.TryFindFileInParentDirectory(directory, projectFileName, out var projectFile));
                Assert.AreEqual(projectFileName, projectFile!.Name);
            }

            [Test]
            public static void TryFindSolutionFileInParentDirectory()
            {
                var directory = ExecutingAssemblyDll.Directory;
                Assert.AreEqual(true, CodeFactory.TryFindFileInParentDirectory(directory, "Gu.Roslyn.Asserts.sln", out var projectFile));
                Assert.AreEqual("Gu.Roslyn.Asserts.sln", projectFile!.Name);
            }
        }
    }
}
