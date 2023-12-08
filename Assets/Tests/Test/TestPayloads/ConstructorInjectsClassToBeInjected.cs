namespace Cr7Sund.Framework.Tests
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
