namespace Cr7Sund.Framework.Tests
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
