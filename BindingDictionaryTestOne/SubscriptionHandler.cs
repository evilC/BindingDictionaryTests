using System;
using System.Collections;
using System.Collections.Generic;

namespace BindingDictionaryTestOne
{
    public class SubscriptionHandler : IEnumerable<KeyValuePair<BindingType, DictionaryWrapper<int, DictionaryWrapper<int, SubscriptionProcessor, BindingDescriptor>, BindingDescriptor>>>
    {
        public DictionaryWrapper<int, DictionaryWrapper<int, SubscriptionProcessor, BindingDescriptor>,
            BindingDescriptor> this[BindingType bindingType] => _bindings[bindingType];

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
                .TryAdd(subReq.SubscriptionDescriptor.SubscriberGuid, subReq);
        }

        private void BindingTypeEmptyHandler(BindingDescriptor emptyeventargs)
        {
            _bindings.TryRemove(emptyeventargs.Type, out _);
        }

        private void IndexEmptyHandler(BindingDescriptor emptyeventargs)
        {
            _bindings[emptyeventargs.Type].TryRemove(emptyeventargs.Index, out _);
        }

        private void SubIndexEmptyHandler(BindingDescriptor emptyeventargs)
        {
            _bindings[emptyeventargs.Type][emptyeventargs.Index].TryRemove(emptyeventargs.SubIndex, out _);
        }

        public void Unsubscribe(InputSubscriptionRequest subReq)
        {
            if (ContainsKey(subReq.BindingDescriptor.Type, subReq.BindingDescriptor.Index, subReq.BindingDescriptor.SubIndex))
            {
                _bindings[subReq.BindingDescriptor.Type][subReq.BindingDescriptor.Index][subReq.BindingDescriptor.SubIndex].TryRemove(subReq.SubscriptionDescriptor.SubscriberGuid, out _);
            }
        }

        public void FireCallbacks(BindingDescriptor bindingDescriptor, int value)
        {
            if (ContainsKey(bindingDescriptor.Type, bindingDescriptor.Index, bindingDescriptor.SubIndex))
            {
                _bindings[bindingDescriptor.Type][bindingDescriptor.Index][bindingDescriptor.SubIndex].FireCallbacks(bindingDescriptor, value);
            }
        }

        public bool ContainsKey(BindingType bindingType, int index, int subIndex)
        {
            return ContainsKey(bindingType, index) && _bindings[bindingType][index].ContainsKey(subIndex);
        }

        public bool ContainsKey(BindingType bindingType, int index)
        {
            return _bindings.ContainsKey(bindingType) && _bindings[bindingType].ContainsKey(index);
        }

        public bool ContainsKey(BindingType bindingType)
        {
            return _bindings.ContainsKey(bindingType);
        }

        public int Count()
        {
            return _bindings.Count;
        }

        public int Count(BindingType bindingType)
        {
            return ContainsKey(bindingType) ? _bindings[bindingType].Count : 0;
        }

        public int Count(BindingType bindingType, int index)
        {
            return ContainsKey(bindingType, index) ? _bindings[bindingType][index].Count : 0;
        }

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