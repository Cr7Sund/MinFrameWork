namespace Cr7Sund.Framework.Tests
{
    public class PostConstructClass
    {
        [Inject]
        public float floatVal;

        [PostConstruct]
        public void MultiplyBy2()
        {
            floatVal *= 2f;
        }
    }
}
