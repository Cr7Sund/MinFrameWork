namespace Cr7Sund.PackageTest.IOC
{
    public class PostConstructSimple
    {

        public static int PostConstructCount { get; set; }

        [PostConstruct]
        public void MultiplyBy2()
        {
            PostConstructCount++;
        }
    }
}
