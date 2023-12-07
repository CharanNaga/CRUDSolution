namespace CRUDTests
{
    public class UnitTest1
    {
        [Fact] //Unit Tests are decorated with this attribute. Follows clean seperation of concerns so any file can be unit testable.
        public void Test1()
        {
            //Three Steps:
            //Arrange - Declaration of Variables, Creating Object to call the methods and collecting inputs
            MyMath myMath = new MyMath();
            int input1 = 10, input2 = 5;
            int expected = 15;

            //Act - Calling the method to perform test.
            int actual = myMath.Add(input1, input2);

            //Assert - Comparing expected value with actual value. If both are same, then Testcase pass.
            Assert.Equal(expected, actual);

            //For each possible input, we will create unit test.
            //To See Test outputs, View-> Test Explorer -> Run all tests for available test.
        }
    }
}