using System;
using Cr7Sund.Collection.Generic;
using NUnit.Framework;

namespace Cr7Sund.CollectionTest
{
    public class TestUnsafeUnOrderList
    {
        [Test]
        public void AddLast()
        {
            var list = new UnsafeUnOrderList<int>();
            list.AddLast(1);
            list.AddLast(2);
            list.AddLast(3);
            Assert.AreEqual(3, list.Count);
        }

        [Test]
        public void RemoveItems()
        {
            var list = new UnsafeUnOrderList<int>();
            list.AddLast(1);
            list.AddLast(2);
            list.Remove(2);

            Assert.AreEqual(1, list.Count);
        }

        [Test]
        public void RemoveNoExistItemsException()
        {
            var list = new UnsafeUnOrderList<int>();
            list.AddLast(1);
            list.AddLast(2);
            list.Remove(2);

            TestDelegate handler = () => list.Remove(3);
            Assert.Throws<IndexOutOfRangeException>(handler);
        }


        [Test]
        public void Clear()
        {
            var list = new UnsafeUnOrderList<int>();
            list.AddLast(1);
            list.AddLast(2);
            list.Clear();

            Assert.AreEqual(0, list.Count);
        }

        [Test]
        public void EnumerateEmpty()
        {
            var list = new UnsafeUnOrderList<int>();

            foreach (var item in list) { }
        }

        [Test]
        public void Enumerate()
        {
            var list = new UnsafeUnOrderList<int>();
            list.AddLast(1);
            list.AddLast(3);
            list.AddLast(5);
            list.AddLast(10);
            list.Remove(5);
            list.Remove(1);

            var result = 0;
            foreach (var item in list)
            {
                result += item;
            }

            Assert.AreEqual(13, result);
        }
    }
}