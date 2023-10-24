using System;
namespace Cr7Sund.Framework.Tests
{
    public class ConstructorInjectsClassToBeInjected
    {
        [Inject]
        public ClassToBeInjected injected;
        public static int Value = 0;
        public ConstructorInjectsClassToBeInjected()
        {
            Value++;
        }
    }
}

