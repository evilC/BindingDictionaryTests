﻿using System;

namespace BindingDictionaryTestTwo
{
    public class SubscriptionProcessor : SubscriptionDictionary<Guid, InputSubscriptionRequest, BindingDescriptor>
    {
        public SubscriptionProcessor(BindingDescriptor emptyEventArgs, EmptyHandler emptyHandler) : base(emptyEventArgs, emptyHandler)
        {
        }

        public void FireCallbacks(BindingDescriptor bindingDescriptor, int value)
        {
            foreach (var inputSubscriptionRequest in Dictionary.Values)
            {
                inputSubscriptionRequest.Callback(value);
            }
        }
    }
}