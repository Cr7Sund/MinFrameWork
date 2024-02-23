using System;

namespace Cr7Sund
{
    [Serializable]
    public struct AssetKey : IAssetKey
    {
        public string FullName;

        public string Key { get => FullName; }


        public AssetKey(string key)
        {
            FullName = key;
        }


        public bool IsValid()
        {
            return string.IsNullOrEmpty(Key);
        }
    }
}