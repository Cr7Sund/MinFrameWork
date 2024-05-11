using Cr7Sund.Package.Api;
using Cr7Sund.Package.Impl;
using NUnit.Framework;
namespace Cr7Sund.PackageTest.IOC
{
    internal class TestSemiBinding
    {

        private ISemiBinding semibinding;

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
            Assert.AreEqual(typeof(TestSemiBinding), semibinding.SingleValue);
        }

        [Test]
        public void TestIntType()
        {
            semibinding.Add(typeof(int));
            var typeOfInt = typeof(int);
            Assert.AreEqual(typeOfInt, semibinding.SingleValue);
        }

        [Test]
        public void TestObject()
        {
            var o = new ClassWithConstructorParameters(42, "abc");
            semibinding.Add(o);
            Assert.AreEqual(o, semibinding.SingleValue);
            Assert.AreEqual(42, o.intValue);
        }

        [Test]
        public void TestOverwriteSingleSemibinding()
        {
            var o = new ClassWithConstructorParameters(42, "abc");
            semibinding.Add(o);
            var o1 = new ClassWithConstructorParameters(43, "def");
            semibinding.Add(o1);
            var o2 = new ClassWithConstructorParameters(44, "ghi");
            semibinding.Add(o2);
            Assert.AreNotEqual(o, semibinding.SingleValue);
            Assert.AreEqual(o2, semibinding.SingleValue);
            Assert.AreEqual(44, o2.intValue);
        }

        [Test]
        public void TestRemoveFromSingleSemibinding()
        {
            semibinding.Constraint = BindingConstraintType.ONE;

            var o = new ClassWithConstructorParameters(42, "abc");
            semibinding.Add(o);

            var value = semibinding.SingleValue as ClassWithConstructorParameters;

            Assert.AreEqual(o, value);
            Assert.AreEqual(42, value.intValue);

            semibinding.Remove(o);

            Assert.IsNull(semibinding.SingleValue);
        }

        [Test]
        public void TestMultiSemibinding()
        {
            semibinding.Constraint = BindingConstraintType.MANY;
            semibinding.InflationType = PoolInflationType.INCREMENT;

            var o = new ClassWithConstructorParameters(42, "abc");
            semibinding.Add(o);
            var o1 = new ClassWithConstructorParameters(43, "def");
            semibinding.Add(o1);
            var o2 = new ClassWithConstructorParameters(44, "ghi");
            semibinding.Add(o2);

            Assert.AreEqual(3, semibinding.Count);
            Assert.AreEqual(3, ((SemiBinding)semibinding).Capacity);
            var value = semibinding[2] as ClassWithConstructorParameters;
            Assert.AreEqual(o2, value);
            Assert.AreEqual(44, value.intValue);
        }

        [Test]
        public void TestMultiSemibinding_DoubleIncrease()
        {
            semibinding.Constraint = BindingConstraintType.MANY;
            semibinding.InflationType = PoolInflationType.DOUBLE;

            var o = new ClassWithConstructorParameters(42, "abc");
            semibinding.Add(o);
            var o1 = new ClassWithConstructorParameters(43, "def");
            semibinding.Add(o1);
            var o2 = new ClassWithConstructorParameters(44, "ghi");
            semibinding.Add(o2);

            Assert.AreEqual(3, semibinding.Count);
            Assert.AreEqual(4, ((SemiBinding)semibinding).Capacity);
            var value = semibinding[2] as ClassWithConstructorParameters;
            Assert.AreEqual(o2, value);
            Assert.AreEqual(44, value.intValue);
        }

        [Test]
        public void TestAddList()
        {
            semibinding.Constraint = BindingConstraintType.MANY;

            var o = new ClassWithConstructorParameters(42, "abc");
            var o1 = new ClassWithConstructorParameters(43, "def");
            var o2 = new ClassWithConstructorParameters(44, "ghi");

            var list = new ClassWithConstructorParameters[]
            {
                o, o1,
                o2
            };
            semibinding.Add(list);

            Assert.AreEqual(3, semibinding.Count);
            var value = semibinding[2] as ClassWithConstructorParameters;
            Assert.AreEqual(o2, value);
            Assert.AreEqual(44, value.intValue);
        }

        [Test]
        public void TestRemoveFromMultiSemibinding()
        {
            semibinding.Constraint = BindingConstraintType.MANY;

            var o = new ClassWithConstructorParameters(42, "abc");
            semibinding.Add(o);
            var o1 = new ClassWithConstructorParameters(43, "def");
            semibinding.Add(o1);
            var o2 = new ClassWithConstructorParameters(44, "ghi");
            semibinding.Add(o2);

            Assert.AreEqual(3, semibinding.Count);
            var beforeValue = semibinding[2] as ClassWithConstructorParameters;
            Assert.AreEqual(o2, beforeValue);
            Assert.AreEqual(44, beforeValue.intValue);

            semibinding.Remove(o1);

            Assert.AreEqual(2, semibinding.Count);
            var afterValue = semibinding[1] as ClassWithConstructorParameters;
            Assert.AreEqual(o2, afterValue);
            Assert.AreEqual(44, afterValue.intValue);
        }

        [Test]
        public void TestRemoveList()
        {
            semibinding.Constraint = BindingConstraintType.MANY;

            var o = new ClassWithConstructorParameters(42, "abc");
            var o1 = new ClassWithConstructorParameters(43, "def");
            var o2 = new ClassWithConstructorParameters(44, "ghi");
            var list = new ClassWithConstructorParameters[3]
            {
                o, o1,
                o2
            };
            semibinding.Add(list);

            Assert.AreEqual(3, semibinding.Count);
            var beforeValue = semibinding[2] as ClassWithConstructorParameters;
            Assert.AreEqual(o2, beforeValue);
            Assert.AreEqual(44, beforeValue.intValue);

            var removalList = new ClassWithConstructorParameters[2]
            {
                o, o2
            };
            semibinding.Remove(removalList);

            Assert.AreEqual(1, semibinding.Count);
            var afterValue = semibinding[0] as ClassWithConstructorParameters;
            Assert.AreEqual(o1, afterValue);
            Assert.AreEqual(43, afterValue.intValue);
        }

        [Test]
        public void TestClearSemiBinding()
        {
            semibinding.Constraint = BindingConstraintType.MANY;

            var o = new ClassWithConstructorParameters(42, "abc");
            var o1 = new ClassWithConstructorParameters(43, "def");
            var o2 = new ClassWithConstructorParameters(44, "ghi");
            var list = new ClassWithConstructorParameters[3]
            {
                o, o1,
                o2
            };
            semibinding.Add(list);


            semibinding.Dispose();

            Assert.IsNull(semibinding.SingleValue);
        }
    }
}
