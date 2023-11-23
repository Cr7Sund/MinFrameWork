using Cr7Sund.Framework.Impl;
using NUnit.Framework;
namespace Cr7Sund.Framework.Tests
{
    public class TestCrossContext
    {
        private CrossContext ChildOne;
        private CrossContext ChildTwo;
        private CrossContext Parent;
        private object view;

        [SetUp]
        public void SetUp()
        {
            Context.FirstContext = null;
            view = new object();
            Parent = new CrossContext(view, true);
            ChildOne = new CrossContext(view, true); //Ctr will automatically add to Context.firstcontext. No need to call it manually (and you should not).
            ChildTwo = new CrossContext(view, true);
        }

        [Test]
        public void TestValue()
        {
            var parentModel = new TestModel();
            Parent.InjectionBinder.Bind<TestModel>().ToValue(parentModel).CrossContext(); //bind it once here and it should be accessible everywhere

            var parentModelTwo = Parent.InjectionBinder.GetInstance<TestModel>();

            Assert.AreSame(parentModel, parentModelTwo); //Assure that this value is correct

            var childOneModel = ChildOne.InjectionBinder.GetInstance<TestModel>();
            Assert.IsNotNull(childOneModel);
            var childTwoModel = ChildTwo.InjectionBinder.GetInstance<TestModel>();
            Assert.IsNotNull(childTwoModel);
            Assert.AreSame(childOneModel, childTwoModel); //These two should be the same object

            Assert.AreEqual(0, parentModel.Value);

            parentModel.Value++;
            Assert.AreEqual(1, childOneModel.Value);

            parentModel.Value++;
            Assert.AreEqual(2, childTwoModel.Value);

        }

        [Test]
        public void TestFactory()
        {
            var parentModel = new TestModel();
            Parent.InjectionBinder.Bind<TestModel>().To<TestModel>().CrossContext();

            var parentModelTwo = Parent.InjectionBinder.GetInstance<TestModel>();

            Assert.AreNotSame(parentModel, parentModelTwo); //As it's a factory, we should not have the same objects

            var childOneModel = ChildOne.InjectionBinder.GetInstance<TestModel>();
            Assert.IsNotNull(childOneModel);
            var childTwoModel = ChildTwo.InjectionBinder.GetInstance<TestModel>();
            Assert.IsNotNull(childTwoModel);
            Assert.AreNotSame(childOneModel, childTwoModel); //These two should be DIFFERENT

            Assert.AreEqual(0, parentModel.Value);

            parentModel.Value++;
            Assert.AreEqual(0, childOneModel.Value); //doesn't change

            parentModel.Value++;
            Assert.AreEqual(0, childTwoModel.Value); //doesn't change

        }

        [Test]
        public void TestDifferInstanceOfValueType()
        {
            var parentModel = new TestModel();
            Parent.InjectionBinder.Bind<TestModel>().To<TestModel>().CrossContext();

            var parentModelTwo = Parent.InjectionBinder.GetInstance<TestModel>();
            var parentModelOne = Parent.InjectionBinder.GetInstance<TestModel>();

            Assert.AreNotEqual(parentModelOne, parentModelTwo);
        }

        [Test]
        public void TestNamed()
        {
            string name = "Name";
            var parentModel = new TestModel();
            Parent.InjectionBinder.Bind<TestModel>().ToValue(parentModel).ToName(name).CrossContext(); //bind it once here and it should be accessible everywhere

            var parentModelTwo = Parent.InjectionBinder.GetInstance<TestModel>(name);

            Assert.AreSame(parentModel, parentModelTwo); //Assure that this value is correct

            var childOneModel = ChildOne.InjectionBinder.GetInstance<TestModel>(name);
            Assert.IsNotNull(childOneModel);
            var childTwoModel = ChildTwo.InjectionBinder.GetInstance<TestModel>(name);
            Assert.IsNotNull(childTwoModel);
            Assert.AreSame(childOneModel, childTwoModel); //These two should be the same object

            Assert.AreEqual(0, parentModel.Value);

            parentModel.Value++;
            Assert.AreEqual(1, childOneModel.Value);

            parentModel.Value++;
            Assert.AreEqual(2, childTwoModel.Value);
        }

        //test that local bindings will override cross bindings
        [Test]
        public void TestLocalOverridesCrossContext()
        {
            Parent.InjectionBinder.Bind<TestModel>().To<TestModel>().AsSingleton().CrossContext(); //bind the cross context binding.
            var initialChildOneModel = new TestModel();
            initialChildOneModel.Value = 1000;


            ChildOne.InjectionBinder.Bind<TestModel>().ToValue(initialChildOneModel); //Bind a local override in this child

            var parentModel = Parent.InjectionBinder.GetInstance<TestModel>(); //Get the instance from the parent injector (The cross context binding)


            var childOneModel = ChildOne.InjectionBinder.GetInstance<TestModel>();
            Assert.AreSame(initialChildOneModel, childOneModel); // The value from getInstance is the same as the value we just mapped as a value locally
            Assert.AreNotSame(childOneModel, parentModel); //The value from getinstance is NOT the same as the cross context value. We have overidden the cross context value locally


            var childTwoModel = ChildTwo.InjectionBinder.GetInstance<TestModel>();
            Assert.IsNotNull(childTwoModel);
            Assert.AreNotSame(childOneModel, childTwoModel); //These two are different objects, the childTwoModel being cross context, and childone being the override
            Assert.AreSame(parentModel, childTwoModel); //Both cross context models are the same


            parentModel.Value++;
            Assert.AreEqual(1, childTwoModel.Value); //cross context model should be changed

            parentModel.Value++;
            Assert.AreEqual(1000, childOneModel.Value); //local model is not changed


            Assert.AreEqual(2, parentModel.Value); //cross context model is changed

        }

        [Test]
        public void TestSingleton()
        {
            Parent.InjectionBinder.Bind<TestModel>().To<TestModel>().AsSingleton().CrossContext(); //bind it once here and it should be accessible everywhere

            var parentModel = Parent.InjectionBinder.GetInstance<TestModel>();
            Assert.IsNotNull(parentModel);

            var childOneModel = ChildOne.InjectionBinder.GetInstance<TestModel>();
            Assert.IsNotNull(childOneModel);
            var childTwoModel = ChildTwo.InjectionBinder.GetInstance<TestModel>();
            Assert.IsNotNull(childTwoModel);

            Assert.AreSame(parentModel, childOneModel);
            Assert.AreSame(parentModel, childTwoModel);
            Assert.AreSame(childOneModel, childTwoModel);

            var binding = Parent.InjectionBinder.GetBinding<TestModel>();
            Assert.IsNotNull(binding);
            Assert.IsTrue(binding.IsCrossContext);

            var childBinding = ChildOne.InjectionBinder.GetBinding<TestModel>();
            Assert.IsNotNull(childBinding);
            Assert.IsTrue(childBinding.IsCrossContext);


            Assert.AreEqual(0, parentModel.Value);

            parentModel.Value++;
            Assert.AreEqual(1, childOneModel.Value);

            parentModel.Value++;
            Assert.AreEqual(2, childTwoModel.Value);

        }
    }

    public class TestModel
    {
        public int Value;
    }

}
