using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BindingDictionaryTestOne;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var dev = new DeviceDescriptor { DeviceHandle = "TestHandle" };
            var subHandler = new SubscriptionHandler(dev, DeviceEmptyHandler);

            var bd1 = new BindingDescriptor { Type = BindingType.Button, Index = 1 };
            var pov1Up = new BindingDescriptor { Type = BindingType.POV, Index = 0, SubIndex = 0 };
            var pov1Right = new BindingDescriptor { Type = BindingType.POV, Index = 0, SubIndex = 1 };

            var isr1 = new InputSubscriptionRequest
            {
                BindingDescriptor = bd1,
                DeviceDescriptor = dev,
                SubscriptionDescriptor = new SubscriptionDescriptor
                {
                    SubscriberGuid = Guid.NewGuid()
                }
            };

            var isr2 = new InputSubscriptionRequest
            {
                BindingDescriptor = bd1,
                DeviceDescriptor = dev,
                SubscriptionDescriptor = new SubscriptionDescriptor
                {
                    SubscriberGuid = Guid.NewGuid()
                }
            };

            var isrpov1 = new InputSubscriptionRequest
            {
                BindingDescriptor = pov1Up,
                DeviceDescriptor = dev,
                SubscriptionDescriptor = new SubscriptionDescriptor
                {
                    SubscriberGuid = Guid.NewGuid()
                }
            };

            var isrpov2 = new InputSubscriptionRequest
            {
                BindingDescriptor = pov1Right,
                DeviceDescriptor = dev,
                SubscriptionDescriptor = new SubscriptionDescriptor
                {
                    SubscriberGuid = Guid.NewGuid()
                }
            };

            subHandler.Subscribe(isr1);
            subHandler.Subscribe(isr2);
            subHandler.Subscribe(isrpov1);
            subHandler.Subscribe(isrpov2);

            var x = subHandler.ContainsKey(bd1);
            var countBefore = subHandler.Count(BindingType.POV, 0);
            var containsBefore = subHandler.ContainsKey(BindingType.POV, 0);

            subHandler.Unsubscribe(isrpov2);
            subHandler.Unsubscribe(isrpov1);
            var countAfter = subHandler.Count(BindingType.POV, 0);
            var containsAfter = subHandler.ContainsKey(BindingType.POV, 0);
            subHandler.Unsubscribe(isr1);
            subHandler.Unsubscribe(isr2);

            Console.ReadLine();
        }

        private static void DeviceEmptyHandler(DeviceDescriptor emptyeventargs)
        {
            Console.WriteLine($"Device {emptyeventargs.DeviceHandle} / {emptyeventargs.DeviceInstance} has no more subscriptions");
        }
    }
}
