﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BindingDictionaryTestTwo.Subscriptions;
using SharpDX.XInput;

namespace BindingDictionaryTestTwo.Polling.XInput
{
    public class XiDevicePollHandler : DevicePollHandler<State>
    {
        private State _lastState;

        public XiDevicePollHandler(DeviceDescriptor deviceDescriptor, ISubscriptionHandler subhandler) : base(deviceDescriptor, subhandler)
        {
            // All Buttons share one Poll Processor
            PollProcessors.Add((BindingType.Button, 0), new XiButtonProcessor());
            // LS and RS share one Poll Processor
            PollProcessors.Add((BindingType.Axis, 0), new XiAxisProcessor());
            // Triggers have their own Poll Processor
            PollProcessors.Add((BindingType.Axis, 1), new XiTriggerProcessor());
            // DPad directions are buttons, so share one Button Poll Processor
            PollProcessors.Add((BindingType.POV, 0), new XiButtonProcessor());
        }


        protected override BindingUpdate[] PreProcess(State pollData)
        {
            var updates = new List<BindingUpdate>();
            for (var j = 0; j < 14; j++)
            {
                var isPovType = j > 9;
                var bindingType = isPovType ? BindingType.POV : BindingType.Button;
                var index = isPovType ? 0 : j;
                var subIndex = isPovType ? j - 10 : 0;
                var flag = Utilities.xinputButtonIdentifiers[bindingType][isPovType ? subIndex : index];

                var thisValue = (flag & pollData.Gamepad.Buttons) == flag ? 1 : 0;
                var lastValue = (flag & _lastState.Gamepad.Buttons) == flag ? 1 : 0;
                if (thisValue != lastValue)
                {
                    updates.Add(new BindingUpdate {Binding = new BindingDescriptor{Type = bindingType, Index = index, SubIndex = subIndex}, Value = thisValue });
                }
            }
            ProcessAxis(updates, 0, pollData.Gamepad.LeftThumbX, _lastState.Gamepad.LeftThumbX);
            ProcessAxis(updates, 1, pollData.Gamepad.LeftThumbY, _lastState.Gamepad.LeftThumbY);
            ProcessAxis(updates, 2, pollData.Gamepad.RightThumbX, _lastState.Gamepad.RightThumbX);
            ProcessAxis(updates, 3, pollData.Gamepad.RightThumbY, _lastState.Gamepad.RightThumbY);
            ProcessAxis(updates, 4, pollData.Gamepad.LeftTrigger, _lastState.Gamepad.LeftTrigger);
            ProcessAxis(updates, 5, pollData.Gamepad.RightTrigger, _lastState.Gamepad.RightTrigger);
            
            _lastState = pollData;

            return updates.ToArray();
        }

        private static void ProcessAxis(ICollection<BindingUpdate> updates, int index, int thisState, int lastState)
        {
            if (thisState != lastState)
            {
                updates.Add(new BindingUpdate { Binding = new BindingDescriptor { Type = BindingType.Axis, Index = index, SubIndex = 0 }, Value = thisState });
            }
        }

        protected override (BindingType, int) GetPollProcessorKey(BindingDescriptor bindingDescriptor)
        {
            var index = bindingDescriptor.Type == BindingType.Axis && bindingDescriptor.Index > 4 ? 1 : 0;
            return (bindingDescriptor.Type, index);
        }
    }
}
