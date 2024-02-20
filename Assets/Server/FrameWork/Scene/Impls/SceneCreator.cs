
using Cr7Sund.FrameWork.Util;

namespace Cr7Sund.Server.Scene.Impl
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