using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BindingDictionaryTestTwo
{
    public class SubscriptionHandler : ISubscriptionStore, ISubscriptionInfo
    {
        private readonly SubscriptionDictionary<BindingType,
            SubscriptionDictionary<int, SubscriptionDictionary<int, SubscriptionProcessor, BindingDescriptor>, BindingDescriptor>,
            DeviceDescriptor> _bindings;

        public delegate void DeviceEmptyHandler(DeviceDescriptor emptyEventArgs);
        private readonly DeviceEmptyHandler _deviceEmptyHandler;

        public SubscriptionHandler(DeviceDescriptor deviceDescriptor, DeviceEmptyHandler deviceEmptyHandler)
        {
            _deviceEmptyHandler = deviceEmptyHandler;
            _bindings =
                new SubscriptionDictionary<BindingType,
                    SubscriptionDictionary<int, SubscriptionDictionary<int, SubscriptionProcessor, BindingDescriptor>,
                        BindingDescriptor>, DeviceDescriptor>(deviceDescriptor, OnDeviceEmpty);
        }

        private void OnDeviceEmpty(DeviceDescriptor emptyeventargs)
        {
            _deviceEmptyHandler?.Invoke(emptyeventargs);
        }

        #region Subscriptions
        /// <summary>
        /// Add a subscription
        /// </summary>
        /// <param name="subReq">The Subscription Request object holding details of the subscription</param>
        public void Subscribe(InputSubscriptionRequest subReq)
        {
            _bindings.GetOrAdd(subReq.BindingDescriptor.Type,
                    new SubscriptionDictionary<int, SubscriptionDictionary<int, SubscriptionProcessor, BindingDescriptor>,
                        BindingDescriptor>(subReq.BindingDescriptor, BindingTypeEmptyHandler))
                .GetOrAdd(subReq.BindingDescriptor.Index,
                    new SubscriptionDictionary<int, SubscriptionProcessor, BindingDescriptor>(subReq.BindingDescriptor,
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

        #region GetKeys

        public IEnumerable<BindingType> GetKeys()
        {
            return _bindings.GetKeys();
        }

        public IEnumerable<int> GetKeys(BindingType bindingType)
        {
            return _bindings[bindingType].GetKeys();
        }

        public IEnumerable<int> GetKeys(BindingType bindingType, int index)
        {
            return _bindings[bindingType][index].GetKeys();
        }

        #endregion

        #region Count

        public int Count()
        {
            return _bindings.Count();
        }

        public int Count(BindingType bindingType)
        {
            return ContainsKey(bindingType) ? _bindings[bindingType].Count() : 0;
        }

        public int Count(BindingType bindingType, int index)
        {
            return ContainsKey(bindingType, index) ? _bindings[bindingType][index].Count() : 0;
        }

        #endregion

        #endregion

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
    }
}
