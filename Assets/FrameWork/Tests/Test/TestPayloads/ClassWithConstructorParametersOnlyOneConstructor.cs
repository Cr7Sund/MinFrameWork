namespace Cr7Sund.Framework.Tests
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
