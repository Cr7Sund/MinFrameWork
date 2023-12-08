namespace Cr7Sund.Framework.Tests
{
    public class HasNamedInjections
    {
        [Inject(SomeEnum.ONE)]
        public InjectableSuperClass injectionOne;

        [Inject(SomeEnum.TWO)]
        public InjectableSuperClass injectionTwo;
    }
}
