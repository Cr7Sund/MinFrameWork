using System;

[AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
public sealed class Inject : Attribute
{
    // See the attribute guidelines at
    //  http://go.microsoft.com/fwlink/?LinkId=85236
    readonly object name;

    // This is a positional argument
    public Inject(object name)
    {
        this.name = name;
    }
    public Inject()
    {
    }

    public object Name
    {
        get { return name; }
    }
}




//Tag [PostConstruct] to perform post-injection construction actions
[AttributeUsage(AttributeTargets.Method,
        AllowMultiple = false,
        Inherited = true)]
public class PostConstruct : Attribute
{
    public PostConstruct() { }

    public PostConstruct(int p)
    {
        priority = p;
    }

    public int priority { get; set; }
}

