namespace Cr7Sund.LifeTime
{
    public class ContainerFactory
    {
        public static IAssetContainer Create(string sceneName, bool isUnique = false)
        {
            if (isUnique)
            {
                return new DefaultUniqueInstanceContainer(sceneName);
            }
            return new DefaultUniqueInstanceContainer(sceneName) ;
        }

        public static void Release(IAssetContainer container)
        {

        }
    }
}
