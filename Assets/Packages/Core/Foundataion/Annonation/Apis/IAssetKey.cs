namespace Cr7Sund
{
    public interface IAssetKey
    {
        public string Key { get; }

        string ToString();

        int Priority { get; }
    }

}
