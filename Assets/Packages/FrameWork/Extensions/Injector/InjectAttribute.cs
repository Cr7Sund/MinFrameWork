using System;
[AttributeUsage(AttributeTargets.Field)]
public sealed class Inject : Attribute
{
    // See the attribute guidelines at
    //  http://go.microsoft.com/fwlink/?LinkId=85236

    // This is a positional argument
    public Inject(object name)
    {
        Name = name;
    }
    public Inject()
    {
    }

    public object Name
    {
        get;
    }
}




//Tag [PostConstruct] to perform post-injection construction actions
[AttributeUsage(AttributeTargets.Method)]
public class PostConstruct : Attribute
{
    public PostConstruct() { }

    public PostConstruct(int p)
    {
        priority = p;
    }

    public int priority { get; set; }
}
