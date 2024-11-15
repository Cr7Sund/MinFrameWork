namespace Cr7Sund.Editor.NodeGraph
{
    [System.AttributeUsage(System.AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    sealed class OutPortAttribute : System.Attribute
    {
        // See the attribute guidelines at
        //  http://go.microsoft.com/fwlink/?LinkId=85236
        readonly string displayName;

        // This is a positional argument
        public OutPortAttribute(string name = null)
        {
            this.displayName = name;
        }

        public string DisplayName
        {
            get { return displayName; }
        }

    }
}