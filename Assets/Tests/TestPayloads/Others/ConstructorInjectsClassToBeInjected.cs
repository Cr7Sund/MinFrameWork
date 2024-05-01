namespace Cr7Sund.PackageTest.IOC
{
    public class ConstructorInjectsClassToBeInjected
    {
        public static int Value;
        [Inject]
        public ClassToBeInjected injected;
        public ConstructorInjectsClassToBeInjected()
        {
            Value++;
        }
    }
}
