using System.Collections.Generic;
using BindingDictionaryTestTwo.Subscriptions;
using SharpDX.DirectInput;

namespace BindingDictionaryTestTwo.Polling.DirectInput
{
    public class DiDevicePollHandler : DevicePollHandler<JoystickUpdate>
    {
        public DiDevicePollHandler(DeviceDescriptor deviceDescriptor, ISubscriptionHandler subhandler) : base(deviceDescriptor, subhandler)
        {
            PollProcessors.Add((BindingType.Button, 1), new DiButtonProcessor());
            PollProcessors.Add((BindingType.POV, 0), new DiPoVProcessor());
        }

        protected override BindingUpdate[] PreProcess(JoystickUpdate pollData)
        {
            var type = Utilities.OffsetToType(pollData.Offset);
            var index = type == BindingType.POV
                ? pollData.Offset - JoystickOffset.PointOfViewControllers0
                : (int) pollData.Offset;
            return new[] {new BindingUpdate {Binding = new BindingDescriptor() {Type = type, Index = index}, Value = pollData.Value}};
            //InputDescriptor = new InputDescriptor { Type = type, Index = index }, Value = pollData.Value }
        }
    }
}