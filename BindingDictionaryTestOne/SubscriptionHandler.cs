using System;

namespace BindingDictionaryTestOne
{
    public class SubscriptionHandler
    {
        public delegate void EmptyHandler(DeviceDescriptor emptyEventArgs);
        private readonly EmptyHandler _emptyHandler;

        private readonly BindingDescriptorDictionary<
                DictionaryWrapper<int, SubscriptionProcessor, BindingDescriptor>
            > _bindings;

        public SubscriptionHandler(DeviceDescriptor deviceDescriptor, EmptyHandler emptyHandler)
        {
            _emptyHandler = emptyHandler;
            _bindings = new BindingDescriptorDictionary<
                    DictionaryWrapper<int, SubscriptionProcessor, BindingDescriptor>
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
                    new DictionaryWrapper<int, SubscriptionProcessor, BindingDescriptor>(subReq.BindingDescriptor, BindingEmptyHandler));
            }

            var dict = _bindings[subReq.BindingDescriptor];
            if (!dict.ContainsKey(subReq.BindingDescriptor.SubIndex))
            {
                dict.Add(subReq.BindingDescriptor.SubIndex, new SubscriptionProcessor(subReq.BindingDescriptor, SubIndexEmptyHandler));
            }
            
            dict[subReq.BindingDescriptor.SubIndex].Add(subReq.SubscriptionDescriptor.SubscriberGuid, subReq);
        }

        private void SubIndexEmptyHandler(BindingDescriptor emptyeventargs)
        {
            _bindings[emptyeventargs].Remove(emptyeventargs.SubIndex);
        }

        public void Unsubscribe(InputSubscriptionRequest subReq)
        {
            if (_bindings.ContainsKey(subReq.BindingDescriptor) && _bindings[subReq.BindingDescriptor].ContainsKey(subReq.BindingDescriptor.SubIndex))
            {
                _bindings[subReq.BindingDescriptor][subReq.BindingDescriptor.SubIndex].Remove(subReq.SubscriptionDescriptor.SubscriberGuid);
            }
        }

        public void FireCallbacks(BindingDescriptor bindingDescriptor, int value)
        {
            if (ContainsKey(bindingDescriptor) && _bindings[bindingDescriptor].ContainsKey(bindingDescriptor.SubIndex))
            {
                _bindings[bindingDescriptor][bindingDescriptor.SubIndex].FireCallbacks(bindingDescriptor, value);
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