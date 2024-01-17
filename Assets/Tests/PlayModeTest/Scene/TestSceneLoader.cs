using System;
using System.Collections;
using System.Collections.Generic;
using Cr7Sund.AssetLoader.Impl;
using Cr7Sund.Server.Impl;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace Cr7Sund.Framework.Tests
{
    public class TestSceneLoader : MonoBehaviour
    {
        private ISceneLoader _sceneLoader;

        [SetUp]
        public void SetUp()
        {
            _sceneLoader = AssetLoaderFactory.CreateSceneLoader();
        }

        [Test]
        public void TestLoadScene()
        {
            _sceneLoader.LoadSceneAsync(SampleSceneKeys.SampleSceneKeyOne);
        }

        [Test]
        public void TestUnLoadScene()
        {
            _sceneLoader.LoadSceneAsync(SampleSceneKeys.SampleSceneKeyOne);
            _sceneLoader.UnloadScene(SampleSceneKeys.SampleSceneKeyOne);
        }


        [Test]
        public void TestLoadSceneAdditive()
        {
            _sceneLoader.LoadSceneAsync(SampleSceneKeys.SampleSceneKeyOne);
            _sceneLoader.LoadSceneAsync(SampleSceneKeys.SampleSceneKeyTwo
                , UnityEngine.SceneManagement.LoadSceneMode.Additive);
        }
        
        [Test]
        public void TestActiveSceneAsync()
        {
            SetUp();

            _sceneLoader.LoadSceneAsync(SampleSceneKeys.SampleSceneKeyTwo
                , UnityEngine.SceneManagement.LoadSceneMode.Single, false);

            _sceneLoader.ActiveSceneAsync(SampleSceneKeys.SampleSceneKeyTwo);
        }
    }
}
