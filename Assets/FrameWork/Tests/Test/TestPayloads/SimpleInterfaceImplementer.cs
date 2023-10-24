using System;

namespace Cr7Sund.Framework.Tests
{
    public class SimpleInterfaceImplementer : ISimpleInterface
    {
        public int intValue { get; set; }

        public SimpleInterfaceImplementer()
        { }
    }

    public class SimpleInterfaceImplementerTwo : ISimpleInterface
    {
        [Inject]
        public int _intValue;
        public int intValue { get; set; }

        public SimpleInterfaceImplementerTwo()
        { }
    }
}

