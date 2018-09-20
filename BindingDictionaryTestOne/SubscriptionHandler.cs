using System;
using System.Collections;
using System.Collections.Generic;

namespace BindingDictionaryTestOne
{
    public class SubscriptionHandler : IEnumerable<KeyValuePair<BindingType, DictionaryWrapper<int, DictionaryWrapper<int, SubscriptionProcessor, BindingDescriptor>, BindingDescriptor>>>
    {
        public DictionaryWrapper<int, DictionaryWrapper<int, SubscriptionProcessor, BindingDescriptor>,
            BindingDescriptor> this[BindingType bindingType] => _bindings[bindingType];

        public int Count => _bindings.Count;

        public delegate void EmptyHandler(DeviceDescriptor emptyEventArgs);

        private readonly EmptyHandler _emptyHandler;

        private readonly DictionaryWrapper<BindingType, 
            DictionaryWrapper<int, 
                DictionaryWrapper<int, SubscriptionProcessor, BindingDescriptor> , BindingDescriptor> , DeviceDescriptor>
            _bindings;

    public SubscriptionHandler(DeviceDescriptor deviceDescriptor, EmptyHandler emptyHandler)
        {
            _emptyHandler = emptyHandler;
            _bindings = new DictionaryWrapper<BindingType, 
                DictionaryWrapper<int, 
                    DictionaryWrapper<int, SubscriptionProcessor, BindingDescriptor> , 
                BindingDescriptor>, 
            DeviceDescriptor>(deviceDescriptor, DeviceEmptyHandler);
        }

        private void DeviceEmptyHandler(DeviceDescriptor emptyEventArgs)
        {
            _emptyHandler?.Invoke(emptyEventArgs);
        }

        public void Subscribe(InputSubscriptionRequest subReq)
        {
            _bindings.GetOrAdd(subReq.BindingDescriptor.Type,
                    new DictionaryWrapper<int, DictionaryWrapper<int, SubscriptionProcessor, BindingDescriptor>,
                        BindingDescriptor>(subReq.BindingDescriptor, BindingTypeEmptyHandler))
                .GetOrAdd(subReq.BindingDescriptor.Index,
                    new DictionaryWrapper<int, SubscriptionProcessor, BindingDescriptor>(subReq.BindingDescriptor,
                        IndexEmptyHandler))
                .GetOrAdd(subReq.BindingDescriptor.SubIndex,
                    new SubscriptionProcessor(subReq.BindingDescriptor, SubIndexEmptyHandler))
                .Add(subReq.SubscriptionDescriptor.SubscriberGuid, subReq);
        }

        private void BindingTypeEmptyHandler(BindingDescriptor emptyeventargs)
        {
            _bindings.Remove(emptyeventargs.Type);
        }

        private void IndexEmptyHandler(BindingDescriptor emptyeventargs)
        {
            _bindings[emptyeventargs.Type].Remove(emptyeventargs.Index);
        }

        private void SubIndexEmptyHandler(BindingDescriptor emptyeventargs)
        {
            _bindings[emptyeventargs.Type][emptyeventargs.Index].Remove(emptyeventargs.SubIndex);
        }

        public void Unsubscribe(InputSubscriptionRequest subReq)
        {
            if (_bindings.ContainsKey(subReq.BindingDescriptor.Type) 
                && _bindings[subReq.BindingDescriptor.Type].ContainsKey(subReq.BindingDescriptor.Index)
                && _bindings[subReq.BindingDescriptor.Type][subReq.BindingDescriptor.Index].ContainsKey(subReq.BindingDescriptor.SubIndex))
            {
                _bindings[subReq.BindingDescriptor.Type][subReq.BindingDescriptor.Index][subReq.BindingDescriptor.SubIndex].Remove(subReq.SubscriptionDescriptor.SubscriberGuid);
            }
        }

        public void FireCallbacks(BindingDescriptor bindingDescriptor, int value)
        {
            //if (ContainsKey(bindingDescriptor) && _bindings[bindingDescriptor].ContainsKey(bindingDescriptor.SubIndex))
            //{
            //    _bindings[bindingDescriptor][bindingDescriptor.SubIndex].FireCallbacks(bindingDescriptor, value);
            //}
        }

        /*
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
        */

        public IEnumerator<KeyValuePair<BindingType, DictionaryWrapper<int, DictionaryWrapper<int, SubscriptionProcessor, BindingDescriptor>, BindingDescriptor>>> GetEnumerator()
        {
            return _bindings.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}