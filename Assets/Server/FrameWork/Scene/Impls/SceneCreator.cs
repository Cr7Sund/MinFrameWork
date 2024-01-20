
using Cr7Sund.Framework.Util;
using Cr7Sund.Server.Apis;

namespace Cr7Sund.Server.Impl
{
    public static class SceneCreator
    {
        public static SceneNode Create(SceneKey key)
        {
            SceneBuilder builder = key.Create();

            if (builder == null)
            {
                throw new MyException($"SceneCreator::Create SceneBuilder is null, Key: {key}");
            }

            SceneDirector.Construct(builder);
            builder.SetSceneKey(key);
            return builder.GetProduct();
        }
    }
}