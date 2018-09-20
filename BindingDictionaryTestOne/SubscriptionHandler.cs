namespace BindingDictionaryTestOne
{
    public class SubscriptionHandler
    {
        public delegate void EmptyHandler(DeviceDescriptor emptyEventArgs);
        private readonly EmptyHandler _emptyHandler;

        private readonly BindingDescriptorDictionary<
                DictionaryWrapper<int, InputSubscriptionRequest, BindingDescriptor>
            > _bindings;

        public SubscriptionHandler(DeviceDescriptor deviceDescriptor, EmptyHandler emptyHandler)
        {
            _emptyHandler = emptyHandler;
            _bindings = new BindingDescriptorDictionary<
                    DictionaryWrapper<int, InputSubscriptionRequest, BindingDescriptor>
                >(deviceDescriptor, DeviceEmptyHandler);
        }

        private void DeviceEmptyHandler(DeviceDescriptor emptyEventArgs)
        {
            _emptyHandler?.Invoke(emptyEventArgs);
        }

        public void Subscribe(InputSubscriptionRequest subReq)
        {
            if (!_bindings.ContainsKey(subReq.BindingDescriptor))
            {
                _bindings.Add(subReq.BindingDescriptor, 
                    new DictionaryWrapper<int, InputSubscriptionRequest, BindingDescriptor>(subReq.BindingDescriptor, BindingEmptyHandler));
            }

            _bindings[subReq.BindingDescriptor].Add(subReq.BindingDescriptor.SubIndex, subReq);
        }

        public void Unsubscribe(InputSubscriptionRequest subReq)
        {
            if (_bindings.ContainsKey(subReq.BindingDescriptor))
            {
                _bindings[subReq.BindingDescriptor].Remove(subReq.BindingDescriptor.SubIndex);
            }
        }

        public bool ContainsKey(BindingDescriptor bindingDescriptor)
        {
            return _bindings.ContainsKey(bindingDescriptor);
        }

        public bool ContainsKey((BindingType, int) key)
        {
            return _bindings.ContainsKey(key);
        }

        public bool ContainsKey(BindingType bindingType, int index)
        {
            return _bindings.ContainsKey((bindingType, index));
        }

        public int Count((BindingType, int) key)
        {
            return ContainsKey(key) ? _bindings[key].Count : 0;
        }

        public int Count(BindingType bindingType, int index)
        {
            return Count((bindingType, index));
        }

        public int Count(BindingDescriptor bindingDescriptor)
        {
            return Count((bindingDescriptor.Type, bindingDescriptor.Index));
        }

        private void BindingEmptyHandler(BindingDescriptor emptyeventargs)
        {
            _bindings.Remove(emptyeventargs);
        }
    }
}