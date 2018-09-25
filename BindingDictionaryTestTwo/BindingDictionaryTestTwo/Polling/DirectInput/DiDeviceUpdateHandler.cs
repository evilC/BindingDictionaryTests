﻿using System.Collections.Generic;
using BindingDictionaryTestTwo.Subscriptions;
using SharpDX.DirectInput;

namespace BindingDictionaryTestTwo.Polling.DirectInput
{
    public class DiDeviceUpdateHandler : DeviceUpdateHandler<JoystickUpdate>
    {
        public DiDeviceUpdateHandler(DeviceDescriptor deviceDescriptor, ISubscriptionHandler subhandler) : base(deviceDescriptor, subhandler)
        {
            // All Buttons share one Update Processor
            UpdateProcessors.Add((BindingType.Button, 0), new DiButtonProcessor());
            // All Axes share one Update Processor
            UpdateProcessors.Add((BindingType.Axis, 0), new DiAxisProcessor());
            // POVs are derived, so have one Update Processor each (DI supports max of 4)
            UpdateProcessors.Add((BindingType.POV, 0), new DiPoVProcessor());
            UpdateProcessors.Add((BindingType.POV, 1), new DiPoVProcessor());
            UpdateProcessors.Add((BindingType.POV, 2), new DiPoVProcessor());
            UpdateProcessors.Add((BindingType.POV, 3), new DiPoVProcessor());
        }

        protected override BindingUpdate[] PreProcessUpdate(JoystickUpdate update)
        {
            var type = Utilities.OffsetToType(update.Offset);
            var index = type == BindingType.POV
                ? update.Offset - JoystickOffset.PointOfViewControllers0
                : (int) update.Offset;
            return new[] {new BindingUpdate {Binding = new BindingDescriptor() {Type = type, Index = index}, Value = update.Value}};
        }

        protected override (BindingType, int) GetUpdateProcessorKey(BindingDescriptor bindingDescriptor)
        {
            var index = bindingDescriptor.Type == BindingType.POV ? bindingDescriptor.Index : 0;
            return (bindingDescriptor.Type, index);
        }
    }
}