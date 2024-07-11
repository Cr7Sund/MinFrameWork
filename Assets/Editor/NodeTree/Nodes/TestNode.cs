using System.Collections.Generic;

namespace Cr7Sund.Editor.NodeTree
{
    [Node]
    public class TestNode1
    {
        [InPort]
        public int a0;
        [InPort]
        public int a1;
        [OutPort]
        public int b0;
        [OutPort]
        public double b1;
    }

    [Node]
    public class TestNode2
    {
        [InPort]
        public double a0;
        [InPort]
        public int a1;
        [OutPort]
        public int b0;
        [OutPort]
        public List<int> b1;
    }
}