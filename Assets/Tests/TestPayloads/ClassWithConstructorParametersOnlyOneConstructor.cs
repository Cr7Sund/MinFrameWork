namespace Cr7Sund.PackageTest.IOC
{
    public class ClassWithConstructorParametersOnlyOneConstructor
    {

        public ClassWithConstructorParametersOnlyOneConstructor()
        {
            stringVal = "defaultValue";
        }

        public ClassWithConstructorParametersOnlyOneConstructor(string value)
        {
            stringVal = value;

        }
        public string stringVal
        {
            get;

        }
    }
}
