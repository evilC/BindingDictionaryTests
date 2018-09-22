using System;
using System.Collections;
using System.Collections.Generic;

namespace BindingDictionaryTestOne
{
    /// <summary>
    /// A class to handle subscriptions for one device
    /// 
    /// Primary Functions
    /// 
    /// Subscription storage:
    /// Allow adding / removing of Subscriptions, automatically adding or removing nested dictionaries as needed.
    /// 
    /// Subscription Querying:
    /// This allows poll processors etc to make queries such as "Does the Y axis have any subscriptions?" ...
    /// ... which allows poll processors to not bother checking if an axis changed if nothing is subscribed to it.
    /// </summary>
    public class SubscriptionHandler : IEnumerable<KeyValuePair<BindingType, DictionaryWrapper<int, DictionaryWrapper<int, SubscriptionProcessor, BindingDescriptor>, BindingDescriptor>>>
    {
        /// <summary>
        /// Indexer for accessing root dictionary
        /// </summary>
        /// <param name="bindingType">The BindingType of the binding</param>
        /// <returns></returns>
        public DictionaryWrapper<int, DictionaryWrapper<int, SubscriptionProcessor, BindingDescriptor>,
            BindingDescriptor> this[BindingType bindingType] => _bindings[bindingType];

        /// <summary>
        /// Delegate to call when there are no more subscriptions
        /// </summary>
        /// <param name="emptyEventArgs">Describes the device</param>
        public delegate void EmptyHandler(DeviceDescriptor emptyEventArgs);
        private readonly EmptyHandler _emptyHandler;

        /// <summary>
        /// Holds the list of bindings
        /// </summary>
        private readonly DictionaryWrapper<BindingType, 
            DictionaryWrapper<int, 
                DictionaryWrapper<int, SubscriptionProcessor, BindingDescriptor> , BindingDescriptor> , DeviceDescriptor>
            _bindings;

        /// <summary>
        /// Creates a new Subscription Handler
        /// </summary>
        /// <param name="deviceDescriptor">A DeviceDescriptor object that describes the device being handled</param>
        /// <param name="emptyHandler">The delegate to fire when the device has no more subscriptions</param>
        public SubscriptionHandler(DeviceDescriptor deviceDescriptor, EmptyHandler emptyHandler)
        {
            _emptyHandler = emptyHandler;
            _bindings = new DictionaryWrapper<BindingType, 
                DictionaryWrapper<int, 
                    DictionaryWrapper<int, SubscriptionProcessor, BindingDescriptor> , 
                BindingDescriptor>, 
            DeviceDescriptor>(deviceDescriptor, DeviceEmptyHandler);
        }

        #region Subscriptions

        /// <summary>
        /// Add a subscription
        /// </summary>
        /// <param name="subReq">The Subscription Request object holding details of the subscription</param>
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

        /// <summary>
        /// Remove a subscription
        /// </summary>
        /// <param name="subReq">The Subscription Request object holding details of the subscription</param>
        public void Unsubscribe(InputSubscriptionRequest subReq)
        {
            if (ContainsKey(subReq.BindingDescriptor.Type, subReq.BindingDescriptor.Index, subReq.BindingDescriptor.SubIndex))
            {
                _bindings[subReq.BindingDescriptor.Type][subReq.BindingDescriptor.Index][subReq.BindingDescriptor.SubIndex].TryRemove(subReq.SubscriptionDescriptor.SubscriberGuid, out _);
            }
        }

        /// <summary>
        /// Fires all subscription callbacks for a given Type / Index / SubIndex
        /// </summary>
        /// <param name="bindingDescriptor">A BindingDescriptor describing the binding</param>
        /// <param name="value">The new value for the input</param>
        public void FireCallbacks(BindingDescriptor bindingDescriptor, int value)
        {
            if (ContainsKey(bindingDescriptor.Type, bindingDescriptor.Index, bindingDescriptor.SubIndex))
            {
                _bindings[bindingDescriptor.Type][bindingDescriptor.Index][bindingDescriptor.SubIndex].FireCallbacks(bindingDescriptor, value);
            }
        }

        #endregion

        #region ConcurrentDictionary method wrappings

        #region ContainsKey

        public bool ContainsKey(BindingType bindingType)
        {
            return _bindings.ContainsKey(bindingType);
        }

        public bool ContainsKey(BindingType bindingType, int index)
        {
            return _bindings.ContainsKey(bindingType) && _bindings[bindingType].ContainsKey(index);
        }

        // Should not need to be externally visible
        private bool ContainsKey(BindingType bindingType, int index, int subIndex)
        {
            return ContainsKey(bindingType, index) && _bindings[bindingType][index].ContainsKey(subIndex);
        }

        #endregion

        #region Count

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

        #endregion

        #region IEnumerable

        public IEnumerator<KeyValuePair<BindingType, DictionaryWrapper<int, DictionaryWrapper<int, SubscriptionProcessor, BindingDescriptor>, BindingDescriptor>>> GetEnumerator()
        {
            return _bindings.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        #endregion

        #region Delegates

        /// <summary>
        /// Gets called when there are no more subscriptions
        /// </summary>
        /// <param name="emptyEventArgs">A DeviceDescriptor describing the device</param>
        private void DeviceEmptyHandler(DeviceDescriptor emptyEventArgs)
        {
            _emptyHandler?.Invoke(emptyEventArgs);
        }

        /// <summary>
        /// Gets called when a given BindingType (Axes, Buttons or POVs) no longer has any subscriptions
        /// </summary>
        /// <param name="emptyeventargs">A BindingDescriptor describing the binding</param>
        private void BindingTypeEmptyHandler(BindingDescriptor emptyeventargs)
        {
            _bindings.TryRemove(emptyeventargs.Type, out _);
        }

        /// <summary>
        /// Gets called when a given Index (A single Axis, Button or POV) no longer has any subscriptions
        /// </summary>
        /// <param name="emptyeventargs">A BindingDescriptor describing the binding</param>
        private void IndexEmptyHandler(BindingDescriptor emptyeventargs)
        {
            _bindings[emptyeventargs.Type].TryRemove(emptyeventargs.Index, out _);
        }

        /// <summary>
        /// Gets called when a given SubIndex (eg POV direction) no longer has any subscriptions
        /// </summary>
        /// <param name="emptyeventargs">A BindingDescriptor describing the binding</param>
        private void SubIndexEmptyHandler(BindingDescriptor emptyeventargs)
        {
            _bindings[emptyeventargs.Type][emptyeventargs.Index].TryRemove(emptyeventargs.SubIndex, out _);
        }

        #endregion
    }
}