using System;
using Cr7Sund.Framework.Tests;
using Cr7Sund.Server.Impl;
using NUnit.Framework;
using UnityEngine.TestTools.Constraints;
using Is = NUnit.Framework.Is;

namespace Cr7Sund.Server.Tests
{
    public class TestSceneNode
    {
        private SceneBuilder _gameLogic;

        [SetUp]
        public void SetUp()
        {
            _gameLogic = new SampleSceneBuilder();
        }

        [Test]
        public void NoGCConstructor()
        {
            SampleSceneBuilder builder = new SampleSceneBuilder();
            Assert.That(() =>
            {
                builder.SubClassInitialization();
            }, Is.Not.AllocatingGCMemory());
        }


    }
}