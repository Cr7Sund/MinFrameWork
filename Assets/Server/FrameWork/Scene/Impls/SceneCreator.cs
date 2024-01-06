
using Cr7Sund.Framework.Util;

namespace Cr7Sund.Server.Impl
{
    public static class SceneCreator
    {
        public static SceneNode Create(SceneKey key)
        {
            SceneBuilder builder = key.SceneBuilder;

            if (builder == null)
            {
                throw new MyException($"SceneCreator::Create SceneBuilder is null, Key: {key}");
            }

            builder.SetSceneKey(key);
            SceneDirector.Construct(builder);
            return builder.GetProduct();
        }
    }
}