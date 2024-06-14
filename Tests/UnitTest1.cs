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
            //TestRelishIqController controller = new TestRelishIqController();
            //int result = controller.GetMiddlePoint(997, 749);
            string[] result = "https://api.myservice.com/foo?baz=1".Split("/");
            foreach (string s in result)
            {
                Console.WriteLine(s);
            }
        }
    }
}