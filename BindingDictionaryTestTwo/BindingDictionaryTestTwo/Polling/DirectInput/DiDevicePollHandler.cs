using System.Collections.Generic;
using BindingDictionaryTestTwo.Subscriptions;
using SharpDX.DirectInput;

namespace BindingDictionaryTestTwo.Polling.DirectInput
{
    public class DiDevicePollHandler : DevicePollHandler<JoystickUpdate>
    {
        public DiDevicePollHandler(DeviceDescriptor deviceDescriptor, ISubscriptionHandler subhandler) : base(deviceDescriptor, subhandler)
        {
            // All Buttons share one Poll Processor
            PollProcessors.Add((BindingType.Button, 0), new DiButtonProcessor());
            // All Axes share one Poll Processor
            PollProcessors.Add((BindingType.Axis, 0), new DiAxisProcessor());
            // POVs are derived, so have one Poll Processor each (DI supports max of 4)
            PollProcessors.Add((BindingType.POV, 0), new DiPoVProcessor());
            PollProcessors.Add((BindingType.POV, 1), new DiPoVProcessor());
            PollProcessors.Add((BindingType.POV, 2), new DiPoVProcessor());
            PollProcessors.Add((BindingType.POV, 3), new DiPoVProcessor());
        }

        protected override BindingUpdate[] PreProcess(JoystickUpdate pollData)
        {
            var type = Utilities.OffsetToType(pollData.Offset);
            var index = type == BindingType.POV
                ? pollData.Offset - JoystickOffset.PointOfViewControllers0
                : (int) pollData.Offset;
            return new[] {new BindingUpdate {Binding = new BindingDescriptor() {Type = type, Index = index}, Value = pollData.Value}};
        }

        protected override (BindingType, int) GetPollProcessorKey(BindingDescriptor bindingDescriptor)
        {
            var index = bindingDescriptor.Type == BindingType.POV ? bindingDescriptor.Index : 0;
            return (bindingDescriptor.Type, index);
        }
    }
}