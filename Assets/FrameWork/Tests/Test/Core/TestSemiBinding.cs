using System;
using Cr7Sund.Framework.Api;
using Cr7Sund.Framework.Impl;
using NUnit.Framework;

namespace Cr7Sund.Framework.Tests
{
    class TestSemiBinding
    {

        ISemiBinding semibinding;

        [SetUp]
        public void Setup()
        {
            semibinding = new SemiBinding();
        }

        [TearDown]
        public void TearDown()
        {
            semibinding = null;
        }

        [Test]
        public void TestType()
        {
            semibinding.Add(typeof(TestSemiBinding));
            Assert.AreEqual(typeof(TestSemiBinding), semibinding.Value);
        }

        [Test]
        public void TestIntType()
        {
            semibinding.Add(typeof(int));
            Type typeOfInt = typeof(int);
            Assert.AreEqual(typeOfInt, semibinding.Value);
        }

        [Test]
        public void TestObject()
        {
            ClassWithConstructorParameters o = new ClassWithConstructorParameters(42, "abc");
            semibinding.Add(o);
            Assert.AreEqual(o, semibinding.Value);
            Assert.AreEqual(42, o.intValue);
        }

        [Test]
        public void TestOverwriteSingleSemibinding()
        {
            ClassWithConstructorParameters o = new ClassWithConstructorParameters(42, "abc");
            semibinding.Add(o);
            ClassWithConstructorParameters o1 = new ClassWithConstructorParameters(43, "def");
            semibinding.Add(o1);
            ClassWithConstructorParameters o2 = new ClassWithConstructorParameters(44, "ghi");
            semibinding.Add(o2);
            Assert.AreNotEqual(o, semibinding.Value);
            Assert.AreEqual(o2, semibinding.Value);
            Assert.AreEqual(44, o2.intValue);
        }

        [Test]
        public void TestRemoveFromSingleSemibinding()
        {
            semibinding.Constraint = BindingConstraintType.ONE;

            ClassWithConstructorParameters o = new ClassWithConstructorParameters(42, "abc");
            semibinding.Add(o);

            ClassWithConstructorParameters value = semibinding.Value as ClassWithConstructorParameters;

            Assert.AreEqual(o, value);
            Assert.AreEqual(42, value.intValue);

            semibinding.Remove(o);

            Assert.IsNull(semibinding.Value);
        }

        [Test]
        public void TestMultiSemibinding()
        {
            semibinding.Constraint = BindingConstraintType.MANY;

            ClassWithConstructorParameters o = new ClassWithConstructorParameters(42, "abc");
            semibinding.Add(o);
            ClassWithConstructorParameters o1 = new ClassWithConstructorParameters(43, "def");
            semibinding.Add(o1);
            ClassWithConstructorParameters o2 = new ClassWithConstructorParameters(44, "ghi");
            semibinding.Add(o2);

            object[] values = semibinding.Value as object[];
            Assert.AreEqual(3, values.Length);
            ClassWithConstructorParameters value = values[2] as ClassWithConstructorParameters;
            Assert.AreEqual(o2, value);
            Assert.AreEqual(44, value.intValue);
        }

        [Test]
        public void TestAddList()
        {
            semibinding.Constraint = BindingConstraintType.MANY;

            ClassWithConstructorParameters o = new ClassWithConstructorParameters(42, "abc");
            ClassWithConstructorParameters o1 = new ClassWithConstructorParameters(43, "def");
            ClassWithConstructorParameters o2 = new ClassWithConstructorParameters(44, "ghi");

            ClassWithConstructorParameters[] list = new ClassWithConstructorParameters[3] { o, o1, o2 };
            semibinding.Add(list);

            object[] values = semibinding.Value as object[];
            Assert.AreEqual(3, values.Length);
            ClassWithConstructorParameters value = values[2] as ClassWithConstructorParameters;
            Assert.AreEqual(o2, value);
            Assert.AreEqual(44, value.intValue);
        }

        [Test]
        public void TestRemoveFromMultiSemibinding()
        {
            semibinding.Constraint = BindingConstraintType.MANY;

            ClassWithConstructorParameters o = new ClassWithConstructorParameters(42, "abc");
            semibinding.Add(o);
            ClassWithConstructorParameters o1 = new ClassWithConstructorParameters(43, "def");
            semibinding.Add(o1);
            ClassWithConstructorParameters o2 = new ClassWithConstructorParameters(44, "ghi");
            semibinding.Add(o2);

            object[] before = semibinding.Value as object[];
            Assert.AreEqual(3, before.Length);
            ClassWithConstructorParameters beforeValue = before[2] as ClassWithConstructorParameters;
            Assert.AreEqual(o2, beforeValue);
            Assert.AreEqual(44, beforeValue.intValue);

            semibinding.Remove(o1);

            object[] after = semibinding.Value as object[];
            Assert.AreEqual(2, after.Length);
            ClassWithConstructorParameters afterValue = after[1] as ClassWithConstructorParameters;
            Assert.AreEqual(o2, afterValue);
            Assert.AreEqual(44, afterValue.intValue);
        }

        [Test]
        public void TestRemoveList()
        {
            semibinding.Constraint = BindingConstraintType.MANY;

            ClassWithConstructorParameters o = new ClassWithConstructorParameters(42, "abc");
            ClassWithConstructorParameters o1 = new ClassWithConstructorParameters(43, "def");
            ClassWithConstructorParameters o2 = new ClassWithConstructorParameters(44, "ghi");
            ClassWithConstructorParameters[] list = new ClassWithConstructorParameters[3] { o, o1, o2 };
            semibinding.Add(list);

            object[] before = semibinding.Value as object[];
            Assert.AreEqual(3, before.Length);
            ClassWithConstructorParameters beforeValue = before[2] as ClassWithConstructorParameters;
            Assert.AreEqual(o2, beforeValue);
            Assert.AreEqual(44, beforeValue.intValue);

            ClassWithConstructorParameters[] removalList = new ClassWithConstructorParameters[2] { o, o2 };
            semibinding.Remove(removalList);

            object[] after = semibinding.Value as object[];
            Assert.AreEqual(1, after.Length);
            ClassWithConstructorParameters afterValue = after[0] as ClassWithConstructorParameters;
            Assert.AreEqual(o1, afterValue);
            Assert.AreEqual(43, afterValue.intValue);
        }
    }
}