namespace Cr7Sund.PackageTest.IOC
{
    public interface IMapConfig
    {
    }

    public class MapConfig : IMapConfig
    {
    }

    public interface IMap
    {
    }

    public class Map : IMap
    {
        public Map(IMapConfig config)
        {
        }
    }


}
