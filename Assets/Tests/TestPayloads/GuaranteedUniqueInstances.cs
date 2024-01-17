namespace Cr7Sund.Framework.Tests
{
    public class GuaranteedUniqueInstances
    {

        private static int counter;

        public GuaranteedUniqueInstances()
        {
            uid = ++counter;
        }
        public int uid { get; set; }

        public static void Reset()
        {
            counter = 0;
        }
    }
}
