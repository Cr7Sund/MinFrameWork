using System.Collections.Generic;

namespace Cr7Sund.Editor.NodeGraph
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

        [NodeParams]
        public int nodeInfoInt = 24;
        [NodeParams]
        public string nodeInfoStr = "hello";
        [OutPort]
        public List<int> bList;
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