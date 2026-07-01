using System.Net;
using Xunit;

namespace GLMS.Tests.IntegrationTests
{
    public class SimpleApiTests
    {
        [Fact]
        public void Test1_AlwaysPasses()
        {
            Assert.True(true);
        }

        [Fact]
        public void Test2_StringNotNull()
        {
            string test = "API Test";
            Assert.NotNull(test);
        }

        [Fact]
        public void Test3_NumbersEqual()
        {
            Assert.Equal(5, 5);
        }
    }
}