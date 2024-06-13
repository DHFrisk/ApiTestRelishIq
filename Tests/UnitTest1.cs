using ApiTestRelishIq.Controllers;
namespace Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            TestRelishIqController controller = new TestRelishIqController();
            int result = controller.GetMiddlePoint(997, 749);
            Console.WriteLine(result);
        }
    }
}