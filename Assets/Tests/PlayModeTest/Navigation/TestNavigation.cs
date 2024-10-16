using System.Threading.Tasks;
using Cr7Sund;
using Cr7Sund.FrameWork.Util;
using Cr7Sund.LifeCycle;
using Cr7Sund.Navigation;
using Cr7Sund.PackageTest.IOC;
using Cr7Sund.UILifeTime;
using NUnit.Framework;
using Assert = UnityEngine.Assertions.Assert;

namespace C7rSund.Test.Play
{
    public class ActivityA : Activity
    {
        public Cr7Sund.Navigation.Navigation Navigation { get => _navigation; }
    }

    public class ActivityB : Activity
    {

    }
    public class FragmentA : Fragment
    {

    }

    public class FragmentB : Fragment
    {

    }

    public class FragmentC : Fragment
    {

    }

    public class UIViewA : UIView
    {
        protected override IAssetKey _uiKey
        {
            get => SampleUIKeys.SampleOneUI;
        }
    }

    public class UIViewB : UIView
    {
        protected override IAssetKey _uiKey
        {
            get => SampleUIKeys.SampleTwoUI;
        }
    }

    // auto generated

    public static class UIKeys
    {
        public static IFragmentKey FragmentKeyA = FragmentKey<FragmentA, UIViewA>.Create();
        public static IFragmentKey FragmentKeyC = FragmentKey<FragmentB, UIViewA>.Create();
        public static IFragmentKey FragmentKeyB = FragmentKey<FragmentC, UIViewB>.Create();
    }
   
    public class RouteGraphA : UINavGraph
    {
        public override string Name
        {
            get => nameof(RouteGraphA);
        }

        public RouteGraphA()
        {
            this.Add(UIKeys.FragmentKeyA);
            this.Add(UIKeys.FragmentKeyB);
            // this.Add<FragmentB, UIViewA>();
        }
    }

    public class RouteGraphB : UINavGraph
    {
        public override string Name
        {
            get => nameof(RouteGraphA);
        }

        public RouteGraphB()
        {
            this.Add(UIKeys.FragmentKeyA);
            this.Add(UIKeys.FragmentKeyC);
            // this.Add<FragmentA, UIViewA>();
            // this.Add<FragmentC, UIViewB>();
        }
    }

    public class TestActivies
    {

        [Test]
        public async Task TestLaunchActivity()
        {
            await TaskStackBuilder.LaunchActivity();
            var activity = await TaskStackBuilder.PushActivity<ActivityA>();

            Assert.AreEqual(LifeCycleState.Resumed, activity.State);
        }

        [Test]
        public async Task TestPushActivity()
        {
            await TaskStackBuilder.LaunchActivity();
            var activity = await TaskStackBuilder.PushActivity<ActivityA>();

            var destActivity = await TaskStackBuilder.PushActivity<ActivityB>();
            Assert.AreEqual(LifeCycleState.Resumed, destActivity.State);
        }

        [Test]
        public async Task TestPopActivity()
        {
            await TaskStackBuilder.LaunchActivity();
            var activity = await TaskStackBuilder.PushActivity<ActivityA>();

            var destActivity = await TaskStackBuilder.PushActivity<ActivityB>();
            await TaskStackBuilder.PopActivity();

            Assert.AreEqual(LifeCycleState.Resumed, activity.State);
            Assert.AreEqual(LifeCycleState.Created, destActivity.State);
        }
    }

    public class TestFragments
    {
        private ActivityA _activity;
        private NavController _navController;
        
        [SetUp]
        public async Task SetUp()
        {
            Console.Init(InternalLoggerFactory.Create());
            
            await TaskStackBuilder.LaunchActivity();
            _activity = await TaskStackBuilder.PushActivity<ActivityA>();

            _navController = _activity.Navigation.FindNavController(_activity.ID);
        }

        [TearDown]
        public void TearDown()
        {
            
        }
        
        [Test]
        public async Task TestStartFragment()
        {
            await _navController.Navigate<RouteGraphA>();

            var fragmentA = _activity.FragmentManager.FindFragment(UIKeys.FragmentKeyA);
            var fragmentB = _activity.FragmentManager.FindFragment(UIKeys.FragmentKeyB);
            Assert.AreEqual(_navController.Count, 1);
            Assert.AreEqual(_activity.FragmentManager.Count, 2);
            Assert.AreEqual(LifeCycleState.Resumed, fragmentA.State);
            Assert.AreEqual(LifeCycleState.Resumed, fragmentB.State);
        }

        [Test]
        public async Task TestPushFragment()
        {
            await _navController.Navigate<RouteGraphA>();
            await _navController.Navigate<RouteGraphB>();

            var fragmentA = _activity.FragmentManager.FindFragment(UIKeys.FragmentKeyA);
            var fragmentB = _activity.FragmentManager.FindFragment(UIKeys.FragmentKeyB);
            var fragmentC = _activity.FragmentManager.FindFragment(UIKeys.FragmentKeyC);
            Assert.AreEqual(_navController.Count, 2);
            Assert.AreEqual(_activity.FragmentManager.Count, 2);
            Assert.AreEqual(LifeCycleState.Resumed, fragmentA.State);
            Assert.IsNull(fragmentB);
            Assert.AreEqual(LifeCycleState.Resumed, fragmentC.State);
        }

        [Test]
        public async Task AddFragmentIntoNavGraph()
        {
            await _navController.Navigate<RouteGraphA>();
            await _navController.Add(UIKeys.FragmentKeyC,null);

            var fragmentC = _activity.FragmentManager.FindFragment(UIKeys.FragmentKeyC);
            Assert.AreEqual(_activity.FragmentManager.Count, 3);
            Assert.AreEqual(LifeCycleState.Resumed, fragmentC.State);
        }
        
        [Test]
        public async Task RemoveFragmentIntoNavGraph()
        {
            await _navController.Navigate<RouteGraphA>();
            await _navController.Remove(UIKeys.FragmentKeyA);

            var fragmentA = _activity.FragmentManager.FindFragment(UIKeys.FragmentKeyA);
            var fragmentB = _activity.FragmentManager.FindFragment(UIKeys.FragmentKeyB);
            Assert.AreEqual(_activity.FragmentManager.Count, 1);
            Assert.IsNull(fragmentA);
            Assert.AreEqual(LifeCycleState.Resumed, fragmentB.State);
        }
        
        [Test]
        public async Task ReplaceFragmentInNavGraph()
        {
            await _navController.Navigate<RouteGraphA>();
            await _navController.Replace(UIKeys.FragmentKeyC, UIKeys.FragmentKeyA);

            var fragmentA = _activity.FragmentManager.FindFragment(UIKeys.FragmentKeyA);
            var fragmentB = _activity.FragmentManager.FindFragment(UIKeys.FragmentKeyB);
            var fragmentC = _activity.FragmentManager.FindFragment(UIKeys.FragmentKeyC);
            Assert.AreEqual(_activity.FragmentManager.Count, 2);
            Assert.IsNull(fragmentA);
            Assert.AreEqual(LifeCycleState.Resumed, fragmentB.State);
            Assert.AreEqual(LifeCycleState.Resumed, fragmentC.State);
        }
        
        [Test]
        public async Task SameFragmentInstance()
        {
            await _navController.Navigate<RouteGraphA>();
            var fragmentA = _activity.FragmentManager.FindFragment(UIKeys.FragmentKeyA);
            var guid = fragmentA.ID;
            await _navController.Navigate<RouteGraphB>();

            fragmentA = _activity.FragmentManager.FindFragment(UIKeys.FragmentKeyA);
            Assert.AreEqual(fragmentA.ID, guid);
        }
    }
}
