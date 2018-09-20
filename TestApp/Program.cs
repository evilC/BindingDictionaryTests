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
            var bd2 = new BindingDescriptor { Type = BindingType.Button, Index = 2 };
            var pov1Up = new BindingDescriptor { Type = BindingType.POV, Index = 0, SubIndex = 0 };
            var pov1Right = new BindingDescriptor { Type = BindingType.POV, Index = 0, SubIndex = 1 };

            var b1 = new SubscriptionHelper("Button 1 Subscriber 1", subHandler, dev, bd1);
            var b1a = new SubscriptionHelper("Button 1 Subscriber 2", subHandler, dev, bd1);
            var b2 = new SubscriptionHelper("Button 2", subHandler, dev, bd2);
            var pov1U = new SubscriptionHelper("POV 1 Up", subHandler, dev, pov1Up);
            var pov1R = new SubscriptionHelper("POV 1 Right", subHandler, dev, pov1Right);

            //var x = subHandler.ContainsKey(bd1);
            //var countBefore = subHandler.Count(BindingType.POV, 0);
            //var containsBefore = subHandler.ContainsKey(BindingType.POV, 0);

            subHandler.FireCallbacks(bd1, 100);

            pov1U.Unsubscribe();
            pov1R.Unsubscribe();

            //var countAfter = subHandler.Count(BindingType.POV, 0);
            //var containsAfter = subHandler.ContainsKey(BindingType.POV, 0);

            b1.Unsubscribe();
            b1a.Unsubscribe();
            b2.Unsubscribe();

            Console.ReadLine();
        }

        private static void DeviceEmptyHandler(DeviceDescriptor emptyeventargs)
        {
            Console.WriteLine($"Device {emptyeventargs.DeviceHandle} / {emptyeventargs.DeviceInstance} has no more subscriptions");
        }
    }
}
