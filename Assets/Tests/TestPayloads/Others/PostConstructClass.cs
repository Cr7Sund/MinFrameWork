namespace Cr7Sund.PackageTest.IOC
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
