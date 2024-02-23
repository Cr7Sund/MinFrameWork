using System;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Cr7Sund.Server.Scene.Apis
{
    public interface ISceneContainer : IDisposable
    {
        public string SceneName { get; set; }
        public GameObject CreateInstance(string name, params Type[] components);
        public T CreateInstanceWithComponent<T>(string name) where T : Object;
        public GameObject LoadInstance(IAssetKey assetKey, string name);
        void ReturnInstance(string name);
    }
}