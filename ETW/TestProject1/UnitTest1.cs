using ETW;
using NUnit.Framework;

namespace TestProject1
{
    [TestFixture]
    public class Tests
    {
        private WriteLoadPattern wlp;
        [SetUp]
        public void Setup()
        {

            wlp = new WriteLoadPattern();


        }

        [Test]
        public void Test1()
        {
            var res = wlp.TestVarient();
            Assert.AreEqual(3, res);
        }
    }
}