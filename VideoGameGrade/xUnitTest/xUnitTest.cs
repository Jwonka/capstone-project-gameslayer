namespace testGameSlayer
{
    public class xUnitTest
    {
        [Fact]
        public void thisTestShouldPass()
        {
            Assert.False(false, "This should pass");
        }
        [Fact]
        public void thistestShouldFail()
        {
            Assert.False(true, "This should fail");
        }
        [Fact]
        public void testAnIntegerIs42()
        {
            /*
             Arrange
                Setup test enviroment
            */
            int expected = 42;

            /*
             Act
                Perform the act to be tested
                Gather the actual answer
            */
            int actual = 6 * 7;

            /*
             Assert
                check that expected is actual
             */
            Assert.Equal(expected, actual);
        }
        [Fact(Skip = "This should be skipped")]
        public void thisTestShouldBeSkipped()
        {
            Assert.False(false, "This should not appear");
        }
    }
}