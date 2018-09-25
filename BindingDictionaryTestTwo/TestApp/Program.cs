﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BindingDictionaryTestTwo;
using BindingDictionaryTestTwo.Polling;
using BindingDictionaryTestTwo.Polling.DirectInput;
using BindingDictionaryTestTwo.Polling.XInput;
using BindingDictionaryTestTwo.Subscriptions;
using SharpDX.DirectInput;
using SharpDX.XInput;

namespace TestApp
{
    class Program
    {
        static void Main(string[] args)
        {
            var dev = new DeviceDescriptor { DeviceHandle = "TestHandle" };
            var subHandler = new SubscriptionHandler(dev, DeviceEmptyHandler);
            var diHandler = new DiDevicePollHandler(new DeviceDescriptor(), subHandler);
            diHandler.BindModeUpdate = BindModeHandler;
            var xiHandler = new XiDevicePollHandler(new DeviceDescriptor(), subHandler);
            xiHandler.BindModeUpdate = BindModeHandler;

            var bd1 = new BindingDescriptor { Type = BindingType.Button, Index = 1 };
            var bd2 = new BindingDescriptor { Type = BindingType.Button, Index = 2 };
            var ax1 = new BindingDescriptor { Type = BindingType.Axis, Index = 0 };
            var pov1Up = new BindingDescriptor { Type = BindingType.POV, Index = 0, SubIndex = 0 };
            var pov1Right = new BindingDescriptor { Type = BindingType.POV, Index = 0, SubIndex = 1 };

            var b1 = new SubscriptionHelper("Button 1 Subscriber 1", subHandler, dev, bd1);
            var b1a = new SubscriptionHelper("Button 1 Subscriber 2", subHandler, dev, bd1);
            var b2 = new SubscriptionHelper("Button 2", subHandler, dev, bd2);
            var a1 = new SubscriptionHelper("Axis 1", subHandler, dev, ax1);
            var pov1U = new SubscriptionHelper("POV 1 Up", subHandler, dev, pov1Up);
            var pov1R = new SubscriptionHelper("POV 1 Right", subHandler, dev, pov1Right);

            var tCount = subHandler.Count();

            var bCount = subHandler.Count(BindingType.Button);


            //foreach (var item in subHandler.GetKeys(BindingType.Button))
            //{
            //    Console.WriteLine($"Found Item - Index: {item}");
            //}

            var countBefore = subHandler.Count(BindingType.POV, 0);
            var containsBefore = subHandler.ContainsKey(BindingType.POV, 0);

            //subHandler.FireCallbacks(bd1, 100);
            //diHandler.SetDetectionMode(DetectionMode.Bind);
            xiHandler.Poll(new State { Gamepad = { Buttons = GamepadButtonFlags.B } });
            xiHandler.Poll(new State { Gamepad = { Buttons = GamepadButtonFlags.None } });
            xiHandler.Poll(new State { Gamepad = { Buttons = GamepadButtonFlags.DPadRight } });
            xiHandler.Poll(new State { Gamepad = { LeftThumbX = 200} });

            //diHandler.Poll(new JoystickUpdate { RawOffset = (int)JoystickOffset.PointOfViewControllers0, Value = 0 });
            //diHandler.Poll(new JoystickUpdate { RawOffset = (int)JoystickOffset.PointOfViewControllers0, Value = 9000});
            //diHandler.Poll(new JoystickUpdate { RawOffset = (int)JoystickOffset.PointOfViewControllers0, Value = -1});

            pov1U.Unsubscribe();
            pov1R.Unsubscribe();

            var countAfter = subHandler.Count(BindingType.POV, 0);
            var containsAfter = subHandler.ContainsKey(BindingType.POV, 0);

            b1.Unsubscribe();
            b1a.Unsubscribe();
            b2.Unsubscribe();

            Console.ReadLine();

        }

        private static void BindModeHandler(object sender, BindModeUpdate update)
        {
            var dd = update.Device;
            var bd = update.Binding;
            Console.WriteLine($"Bind Mode| Device: {dd.DeviceHandle} / {dd.DeviceInstance}, Index: {bd.Index}, SubIndex: {bd.SubIndex}, Value: {update.Value}");
        }

        private static void DeviceEmptyHandler(object sender, DeviceDescriptor emptyeventargs)
        {
            Console.WriteLine($"Subscribe Mode| Device: {emptyeventargs.DeviceHandle} / {emptyeventargs.DeviceInstance} has no more subscriptions");
        }
    }

}