using System;
using BindingDictionaryTestOne;
using NUnit.Framework;

namespace UnitTests
{
    [TestFixture]
    public partial class SubscriptionHandlerTests
    {
        private SubscriptionHandler _subHandler;
        private bool _deviceEmptyHandlerFired;
        private readonly DeviceDescriptor _device = new DeviceDescriptor { DeviceHandle = "Test Device" };
        private static readonly BindingDescriptor BdBut1 = new BindingDescriptor { Type = BindingType.Button, Index = 0 };
        private static readonly BindingDescriptor BdBut2 = new BindingDescriptor { Type = BindingType.Button, Index = 1 };
        private static readonly BindingDescriptor BdAxis1 = new BindingDescriptor { Type = BindingType.Axis, Index = 0 };
        private static readonly BindingDescriptor BdAxis2 = new BindingDescriptor { Type = BindingType.Axis, Index = 1 };
        private static readonly BindingDescriptor BdPov1Up = new BindingDescriptor { Type = BindingType.POV, Index = 0, SubIndex = 0 };
        private static readonly BindingDescriptor BdPov1Right = new BindingDescriptor { Type = BindingType.POV, Index = 0, SubIndex = 1 };
        private static readonly BindingDescriptor BdPov2Up = new BindingDescriptor { Type = BindingType.POV, Index = 1, SubIndex = 0 };
        private static readonly BindingDescriptor BdPov2Right = new BindingDescriptor { Type = BindingType.POV, Index = 1, SubIndex = 1 };

        [SetUp]
        public void Init()
        {
            _subHandler = new SubscriptionHandler(_device, DeviceEmptyHandler);
            _deviceEmptyHandlerFired = false;
        }

        private void DeviceEmptyHandler(DeviceDescriptor emptyeventargs)
        {
            _deviceEmptyHandlerFired = true;
        }

        [TestCase(TestName = "EmptyHandler should only fire when last item is removed")]
        public void EmptyHandlerTest()
        {
            var isrB1A = SubIsr(BdBut1);
            var isrB1B = SubIsr(BdBut1);
            var isrB2 = SubIsr(BdBut2);
            var isrA1 = SubIsr(BdAxis1);
            var isrA2 = SubIsr(BdAxis2);
            var isrPov1Up = SubIsr(BdPov1Up);
            var isrPov1Right = SubIsr(BdPov1Right);

            _subHandler.Unsubscribe(isrB1A);
            Assert.That(_deviceEmptyHandlerFired, Is.EqualTo(false));
            _subHandler.Unsubscribe(isrB1B);
            Assert.That(_deviceEmptyHandlerFired, Is.EqualTo(false));
            _subHandler.Unsubscribe(isrB2);
            Assert.That(_deviceEmptyHandlerFired, Is.EqualTo(false));
            _subHandler.Unsubscribe(isrA1);
            Assert.That(_deviceEmptyHandlerFired, Is.EqualTo(false));
            _subHandler.Unsubscribe(isrA2);
            Assert.That(_deviceEmptyHandlerFired, Is.EqualTo(false));
            _subHandler.Unsubscribe(isrPov1Up);
            Assert.That(_deviceEmptyHandlerFired, Is.EqualTo(false));
            _subHandler.Unsubscribe(isrPov1Right);
            Assert.That(_deviceEmptyHandlerFired, Is.EqualTo(true));
        }

        [TestCase(TestName = "Count on root dictionary should function correctly")]
        public void RootDictionaryCountTest()
        {
            var isrB1A = SubIsr(BdBut1);
            Assert.That(_subHandler.Count(), Is.EqualTo(1));
            var isrB1B = SubIsr(BdBut1);
            Assert.That(_subHandler.Count(), Is.EqualTo(1));
            var isrB2 = SubIsr(BdBut2);
            Assert.That(_subHandler.Count(), Is.EqualTo(1));
            var isrA1 = SubIsr(BdAxis1);
            Assert.That(_subHandler.Count(), Is.EqualTo(2));
            var isrA2 = SubIsr(BdAxis2);
            Assert.That(_subHandler.Count(), Is.EqualTo(2));
            var isrPov1Up = SubIsr(BdPov1Up);
            Assert.That(_subHandler.Count(), Is.EqualTo(3));
            var isrPov1Right = SubIsr(BdPov1Right);
            Assert.That(_subHandler.Count(), Is.EqualTo(3));

            _subHandler.Unsubscribe(isrB1A);
            Assert.That(_subHandler.Count(), Is.EqualTo(3));
            _subHandler.Unsubscribe(isrB1B);
            Assert.That(_subHandler.Count(), Is.EqualTo(3));
            _subHandler.Unsubscribe(isrB2);
            Assert.That(_subHandler.Count(), Is.EqualTo(2));
            _subHandler.Unsubscribe(isrA1);
            Assert.That(_subHandler.Count(), Is.EqualTo(2));
            _subHandler.Unsubscribe(isrA2);
            Assert.That(_subHandler.Count(), Is.EqualTo(1));
            _subHandler.Unsubscribe(isrPov1Up);
            Assert.That(_subHandler.Count(), Is.EqualTo(1));
            _subHandler.Unsubscribe(isrPov1Right);
            Assert.That(_subHandler.Count(), Is.EqualTo(0));
        }

        [TestCase(TestName = "Count on BindingType level dictionary should function correctly")]
        public void BindingDictionaryCountTest()
        {
            var isrB1A = SubIsr(BdBut1);
            Assert.That(_subHandler.Count(BindingType.Button), Is.EqualTo(1));
            var isrB1B = SubIsr(BdBut1);
            Assert.That(_subHandler.Count(BindingType.Button), Is.EqualTo(1));
            var isrB2 = SubIsr(BdBut2);
            Assert.That(_subHandler.Count(BindingType.Button), Is.EqualTo(2));
            var isrA1 = SubIsr(BdAxis1);
            Assert.That(_subHandler.Count(BindingType.Axis), Is.EqualTo(1));
            var isrA2 = SubIsr(BdAxis2);
            Assert.That(_subHandler.Count(BindingType.Axis), Is.EqualTo(2));
            var isrPov1Up = SubIsr(BdPov1Up);
            Assert.That(_subHandler.Count(BindingType.POV), Is.EqualTo(1));
            var isrPov1Right = SubIsr(BdPov1Right);
            Assert.That(_subHandler.Count(BindingType.POV), Is.EqualTo(1));
            var isrPov2Up = SubIsr(BdPov2Up);
            Assert.That(_subHandler.Count(BindingType.POV), Is.EqualTo(2));
            var isrPov2Right = SubIsr(BdPov2Right);
            Assert.That(_subHandler.Count(BindingType.POV), Is.EqualTo(2));

            _subHandler.Unsubscribe(isrB1A);
            Assert.That(_subHandler.Count(BindingType.Button), Is.EqualTo(2));
            _subHandler.Unsubscribe(isrB1B);
            Assert.That(_subHandler.Count(BindingType.Button), Is.EqualTo(1));
            _subHandler.Unsubscribe(isrB2);
            Assert.That(_subHandler.Count(BindingType.Button), Is.EqualTo(0));
            _subHandler.Unsubscribe(isrA1);
            Assert.That(_subHandler.Count(BindingType.Axis), Is.EqualTo(1));
            _subHandler.Unsubscribe(isrA2);
            Assert.That(_subHandler.Count(BindingType.Axis), Is.EqualTo(0));
            _subHandler.Unsubscribe(isrPov1Up);
            Assert.That(_subHandler.Count(BindingType.POV), Is.EqualTo(2));
            _subHandler.Unsubscribe(isrPov1Right);
            Assert.That(_subHandler.Count(BindingType.POV), Is.EqualTo(1));
            _subHandler.Unsubscribe(isrPov2Up);
            Assert.That(_subHandler.Count(BindingType.POV), Is.EqualTo(1));
            _subHandler.Unsubscribe(isrPov2Right);
            Assert.That(_subHandler.Count(BindingType.POV), Is.EqualTo(0));
        }
    }
}
