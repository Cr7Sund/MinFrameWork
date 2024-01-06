using Cr7Sund.Selector.Impl;
using NUnit.Framework;

namespace Cr7Sund.Server.Tests
{
    public class TestGameLogic
    {
        private GameLogic _gameLogic;

        [SetUp]
        public void SetUp()
        {
            _gameLogic = new SampleGameLogic();
        }

        [Test]
        public void TestRun()
        {
            _gameLogic.Init();
            _gameLogic.Start();
        }
    }
}