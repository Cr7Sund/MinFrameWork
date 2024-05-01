namespace Cr7Sund.PackageTest.IOC
{
    public class SimpleInterfaceImplementer : ISimpleInterface
    {

        public int intValue { get; set; }
    }

    public class SimpleInterfaceImplementerTwo : ISimpleInterface
    {
        [Inject]
        public int _intValue;

        public int intValue { get; set; }
    }
}
