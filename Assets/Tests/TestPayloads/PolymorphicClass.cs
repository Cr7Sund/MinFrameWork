namespace Cr7Sund.PackageTest.IOC
{
    public class PolymorphicClass : ISimpleInterface, IAnotherSimpleInterface
    {

        #region IAnotherSimpleInterface implementation
        public string stringValue { get; set; }
        #endregion

        #region ISimpleInterface implementation
        public int intValue { get; set; }
        #endregion
    }
}
