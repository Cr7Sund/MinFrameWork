using System;

namespace Cr7Sund
{
    [Serializable]
    public class AssetKey : IAssetKey
    {
        public string FullName;

        public string Key { get => FullName; }
        public int Priority
        {
            get;
        }


        public AssetKey(string key)
        {
            FullName = key;
        }


        public bool IsValid()
        {
            return string.IsNullOrEmpty(Key);
        }

        public override bool Equals(object obj)
        {
            if (obj is IAssetKey assetKey)
            {
                return Key.Equals(assetKey.Key);
            }

            return false;
        }

        public override int GetHashCode()
        {
            return Key.GetHashCode();
        }
    }
}