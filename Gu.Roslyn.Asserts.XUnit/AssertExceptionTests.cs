namespace Gu.Roslyn.Asserts.XUnit
{
    using Xunit;

    public class AssertExceptionTests
    {
        [Fact]
        public void CreateReturnsXunitException()
        {
            var exception = AssertException.Create("abc");
            Assert.IsType<Xunit.Sdk.XunitException>(exception);
            Assert.Equal("abc", exception.Message);
        }
    }
}
