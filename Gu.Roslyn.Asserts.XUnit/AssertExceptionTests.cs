namespace Gu.Roslyn.Asserts.XUnit
{
    using Xunit;

    public class AssertExceptionTests
    {
        [Fact]
        public void CreateReturnsXunitException()
        {
            var exception = AssertException.Create("abc");
#pragma warning disable GU0011 // Don't ignore the return value.
            Assert.IsType<Xunit.Sdk.XunitException>(exception);
#pragma warning restore GU0011 // Don't ignore the return value.
            Assert.Equal("abc", exception.Message);
        }
    }
}
