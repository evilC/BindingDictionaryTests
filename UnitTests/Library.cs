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
                SubscriptionDescriptor = subscriptionDescriptor
            };
        }

        public InputSubscriptionRequest SubIsr(BindingDescriptor bindingDescriptor)
        {
            var subReq = BuildIsr(bindingDescriptor);
            _subHandler.Subscribe(subReq);
            return subReq;
        }

    }
}
