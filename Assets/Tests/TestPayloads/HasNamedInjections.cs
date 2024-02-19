namespace Cr7Sund.PackageTest.IOC
{
    public class HasNamedInjections
    {
        [Inject(SomeEnum.ONE)]
        public InjectableSuperClass injectionOne;

        [Inject(SomeEnum.TWO)]
        public InjectableSuperClass injectionTwo;
    }
}
