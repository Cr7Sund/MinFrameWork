namespace Cr7Sund.PackageTest.IOC
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
