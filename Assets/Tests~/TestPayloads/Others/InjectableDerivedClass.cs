using System;
namespace Cr7Sund.PackageTest.IOC
{
    public class InjectableDerivedClass : InjectableSuperClass
    {
        [Inject]
        public ClassToBeInjected injected;

        [PostConstruct]
        public void postConstruct1()
        {
            System.Console.Write("Calling post construct 1\n");
        }



        public void notAPostConstruct()
        {
            System.Console.Write("notAPostConstruct :: SHOULD NOT CALL THIS!");
        }
    }

    public class InjectableDerivedClassOne : InjectableSuperClass
    {
        [Inject]
        public ClassToBeInjected injected;
    }
    public class InjectableDerivedClassTwo : InjectableSuperClass
    {
        [Inject]
        public ClassToBeInjected injected;
    }

    public class InjectableDerivedClassThree : InjectableSuperClass
    {
        [Inject]
        public ClassToBeInjected injected;
    }
}
