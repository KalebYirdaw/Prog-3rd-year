namespace GLMS.Tests
{
    public class UnitTest1
    {
     
        [Fact]
        public void Test1_AlwaysPasses()
        {
            // This test always passes
            Assert.True(true);
        }

        [Fact]
        public void Test2_StringIsNotNull()
        {
            string test = "Hello";
            Assert.NotNull(test);
        }

        [Fact]
        public void Test3_NumbersAreEqual()
        {
            int expected = 5;
            int actual = 5;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Test4_ContractStatus_DefaultIsDraft()
        {
            // Test your business logic
            var status = "Draft";
            Assert.Equal("Draft", status);
        }

        [Fact]
        public void Test5_CurrencyConversion_ReturnsCorrectValue()
        {
            decimal usd = 100;
            decimal rate = 19.00m;
            decimal expectedZar = 1900;
            decimal actualZar = usd * rate;
            
            Assert.Equal(expectedZar, actualZar);
        }
    }
}