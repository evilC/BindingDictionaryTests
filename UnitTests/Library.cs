using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BindingDictionaryTestOne;

namespace UnitTests
{
    public partial class SubscriptionHandlerTests
    {
        private bool _callbackFired;

        private static SubscriptionDescriptor CreateSubscriptionDescriptor()
        {
            return new SubscriptionDescriptor
            {
                SubscriberGuid = Guid.NewGuid()
            };
        }

        public InputSubscriptionRequest BuildIsr(BindingDescriptor bindingDescriptor,
            SubscriptionDescriptor subscriptionDescriptor = null)
        {
            if (subscriptionDescriptor == null) subscriptionDescriptor = CreateSubscriptionDescriptor();
            return new InputSubscriptionRequest
            {
                DeviceDescriptor = _device,
                BindingDescriptor = bindingDescriptor,
                SubscriptionDescriptor = subscriptionDescriptor,
                Callback = new Action<int>(value =>
                {
                    _callbackFired = true;
                    _callbackResults.Add(new CallbackResult{BindingDescriptor = bindingDescriptor, Value = value});
                })
            };
        }

        public InputSubscriptionRequest SubIsr(BindingDescriptor bindingDescriptor)
        {
            var subReq = BuildIsr(bindingDescriptor);
            _subHandler.Subscribe(subReq);
            return subReq;
        }

    }

    public class CallbackResult
    {
        public BindingDescriptor BindingDescriptor { get; set; }
        public int Value { get; set; }
    }
}
