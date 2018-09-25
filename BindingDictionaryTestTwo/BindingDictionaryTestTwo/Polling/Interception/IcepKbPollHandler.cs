﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BindingDictionaryTestTwo.Polling.Interception.Lib;
using BindingDictionaryTestTwo.Subscriptions;

namespace BindingDictionaryTestTwo.Polling.Interception
{
    public class IcepKbPollHandler : DevicePollHandler<ManagedWrapper.Stroke>
    {
        public IcepKbPollHandler(DeviceDescriptor deviceDescriptor, ISubscriptionHandler subhandler) : base(deviceDescriptor, subhandler)
        {
            PollProcessors.Add((BindingType.Button, 0), new IcepKbPollProcessor());
        }

        protected override BindingUpdate[] PreProcess(ManagedWrapper.Stroke pollData)
        {
            var code = pollData.key.code;
            var state = pollData.key.state;

            // Begin translation of incoming key code, state, extended flag etc...
            // If state is shifted up by 2 (1 or 2 instead of 0 or 1), then this is an "Extended" key code
            if (state > 1)
            {
                if (code == 42)
                {
                    // Shift (42/0x2a) with extended flag = the key after this one is extended.
                    // Example case is Delete (The one above the arrow keys, not on numpad)...
                    // ... this generates a stroke of 0x2a (Shift) with *extended flag set* (Normal shift does not do this)...
                    // ... followed by 0x53 with extended flag set.
                    // We do not want to fire subsriptions for the extended shift, but *do* want to let the key flow through...
                    // ... so that is handled here.
                    // When the extended key (Delete in the above example) subsequently comes through...
                    // ... it will have code 0x53, which we shift to 0x153 (Adding 256 Dec) to signify extended version...
                    // ... as this is how AHK behaves with GetKeySC()


                    // Do not block this stroke
                    // ToDo:Investigate ramifications of this.
                    // What happens if we block the next character, surely that does not consume the extended flag?

                    return new BindingUpdate[0];
                }
                else
                {
                    // Extended flag set
                    // Shift code up by 256 (0x100) to signify extended code
                    code += 256;
                    state -= 2;
                }
            }

            // state should now be 1 for pressed and 0 for released. Convert to UCR format (pressed == 1)
            state = (ushort)(1 - state);

            return new[] { new BindingUpdate { Binding = new BindingDescriptor() { Type = BindingType.Button, Index = code }, Value = state } };
        }

        protected override (BindingType, int) GetPollProcessorKey(BindingDescriptor bindingDescriptor)
        {
            return (bindingDescriptor.Type, 0);
        }
    }
}
