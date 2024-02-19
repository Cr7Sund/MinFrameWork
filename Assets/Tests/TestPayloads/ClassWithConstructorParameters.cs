namespace Cr7Sund.PackageTest.IOC
{
    public class ClassWithConstructorParameters : ISimpleInterface
    {

        //Two constructors. One is tagged to be the constructor used during injection
        public ClassWithConstructorParameters()
        {
            intValue = 42;
            stringValue = "Liberator";
        }

        public ClassWithConstructorParameters(int intValue, string stringValue)
        {
            this.intValue = intValue;
            this.stringValue = stringValue;
        }

        public string stringValue
        {
            get;
            set;
        }

        public int intValue
        {
            get;
            set;
        }

        public void DeConstruct()
        {
            intValue = 0;
        }
    }
}
