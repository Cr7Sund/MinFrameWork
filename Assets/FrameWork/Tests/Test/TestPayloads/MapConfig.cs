using System.Diagnostics;

namespace Cr7Sund.Framework.Tests
{
    public interface IMapConfig
    { }

    public class MapConfig : IMapConfig
    { }

    public interface IMap
    {
    }

    public class Map : IMap
    {
        public Map(IMapConfig config)
        { }
    }


}