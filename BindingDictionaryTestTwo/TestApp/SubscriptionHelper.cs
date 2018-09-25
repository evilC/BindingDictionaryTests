using System;
using BindingDictionaryTestTwo;
using BindingDictionaryTestTwo.Subscriptions;

namespace TestApp
{
    public class SubscriptionHelper
    {
        private string _text;
        private readonly SubscriptionHandler _handler;
        private readonly InputSubscriptionRequest _subReq;

        public SubscriptionHelper(string text, SubscriptionHandler handler, DeviceDescriptor dev, BindingDescriptor bindingDescriptor)
        {
            _text = text;
            _handler = handler;
            _subReq = new InputSubscriptionRequest
            {
                BindingDescriptor = bindingDescriptor,
                DeviceDescriptor = dev,
                SubscriptionDescriptor = new SubscriptionDescriptor
                {
                    SubscriberGuid = Guid.NewGuid()
                },
                Callback = new Action<int>(value =>
                {
                    Console.WriteLine($"{text}| Value: {value}");
                })
            };
            _handler.Subscribe(_subReq);
        }

        public void Unsubscribe()
        {
            _handler.Unsubscribe(_subReq);
        }

        public void Callback(int value)
        {
            Console.WriteLine($"Value: {value}");
        }
    }
}
