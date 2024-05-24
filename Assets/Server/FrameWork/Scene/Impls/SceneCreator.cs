
using Cr7Sund.FrameWork.Util;

namespace Cr7Sund.Server.Scene.Impl
{
    public static class SceneCreator
    {
        public static async PromiseTask<SceneNode> Create(SceneKey key)
        {
            SceneBuilder builder = key.Create();

            if (builder == null)
            {
                throw new MyException($"SceneCreator::Create SceneBuilder is null, Key: {key}");
            }

            await SceneDirector.Construct(builder, key);
            return builder.GetProduct();
        }
    }
}