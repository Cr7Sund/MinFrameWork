using System;
using System.Collections;
using System.Collections.Generic;
using Cr7Sund.AssetLoader.Impl;
using Cr7Sund.Server.Impl;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Cr7Sund.PackageTest.IOC
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
            _sceneLoader.LoadSceneAsync(SampleSceneKeys.SampleSceneKeyOne, LoadSceneMode.Additive, false);
        }

        [Test]
        public void TestUnLoadScene()
        {
            _sceneLoader.LoadSceneAsync(SampleSceneKeys.SampleSceneKeyOne, LoadSceneMode.Additive, false);
            _sceneLoader.UnloadScene(SampleSceneKeys.SampleSceneKeyOne);
        }


        [Test]
        public void TestLoadSceneAdditive()
        {
            _sceneLoader.LoadSceneAsync(SampleSceneKeys.SampleSceneKeyOne, LoadSceneMode.Single, false);
            _sceneLoader.LoadSceneAsync(SampleSceneKeys.SampleSceneKeyTwo
                , UnityEngine.SceneManagement.LoadSceneMode.Additive, false);
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
