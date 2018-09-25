﻿using System;
using System.Collections.Generic;
using BindingDictionaryTestTwo.Subscriptions;

namespace BindingDictionaryTestTwo.Polling
{
    /// <summary>
    /// Handles processing of Updates for a device. 
    /// Given a series of updates from a device, and a reference to a <see cref="SubscriptionHandler"/> containing subscriptions,
    /// will generate Subscription Events or Bind Mode events accordingly
    /// </summary>
    /// <typeparam name="TUpdate">The Type of the update that comes from the device</typeparam>
    /// <typeparam name="TProcessorKey">The Key type used for the <see cref="UpdateProcessors"/> dictionary</typeparam>
    public abstract class DeviceUpdateHandler<TUpdate, TProcessorKey>
    {
        private readonly DeviceDescriptor _deviceDescriptor;
        protected ISubscriptionHandler SubHandler;
        protected DetectionMode DetectionMode = DetectionMode.Subscription;
        protected Dictionary<TProcessorKey, IUpdateProcessor> UpdateProcessors = new Dictionary<TProcessorKey, IUpdateProcessor>();

        public EventHandler<BindModeUpdate> BindModeUpdate;

        /// <summary>
        /// Create a new DeviceUpdateHandler
        /// </summary>
        /// <param name="deviceDescriptor">The descriptor describing the device</param>
        /// <param name="subhandler">A <see cref="SubscriptionHandler"/> that holds a list of subscriptions</param>
        protected DeviceUpdateHandler(DeviceDescriptor deviceDescriptor, ISubscriptionHandler subhandler)
        {
            _deviceDescriptor = deviceDescriptor;
            SubHandler = subhandler;
        }

        /// <summary>
        /// Enables or disables Bind Mode
        /// </summary>
        /// <param name="mode"></param>
        public void SetDetectionMode(DetectionMode mode)
        {
            DetectionMode = mode;
        }

        /// <summary>
        /// Routes events for Bind Mode
        /// </summary>
        /// <param name="update"></param>
        protected void OnBindModeUpdate(BindingUpdate update)
        {
            BindModeUpdate?.Invoke(this, new BindModeUpdate{Device = _deviceDescriptor, Binding = update.Binding, Value = update.Value});
        }

        /// <summary>
        /// Called by a device poller when the device reports new data
        /// </summary>
        /// <param name="rawUpdate"></param>
        public virtual void ProcessUpdate(TUpdate rawUpdate)
        {
            var bindMode = DetectionMode == DetectionMode.Bind;

            // Convert the raw Update Data from the Generic form into a consistent format
            // At this point, only physical input data is usually present
            var preProcessedUpdates = PreProcessUpdate(rawUpdate);

            foreach (var preprocessedUpdate in preProcessedUpdates)
            {
                // Screen out any updates which are not needed
                // If we are in Bind Mode, let all through, but in Subscription Mode, only let those through which have subscriptions
                var isSubscribed = SubHandler.ContainsKey(preprocessedUpdate.Binding.Type, preprocessedUpdate.Binding.Index);
                if (!(bindMode || isSubscribed)) return;

                // Convert from Pre-processed to procesed updates
                // It is at this point that the state of Logical / Derived inputs are typically calculated (eg DirectInput POVs) ...
                // ... so this may result in one update splitting into many
                var bindingUpdates = UpdateProcessors[GetUpdateProcessorKey(preprocessedUpdate.Binding)].Process(preprocessedUpdate);

                // Route the processed updates to the appropriate place
                // ToDo: Best to make this check, or swap out delegates depending on mode?
                if (bindMode)
                {
                    // Bind Mode - Fire Event Handler
                    foreach (var bindingUpdate in bindingUpdates)
                    {
                        OnBindModeUpdate(bindingUpdate);
                    }
                }
                else
                {
                    // Subscription Mode - Ask SubscriptionHandler to Fire Callbacks
                    foreach (var bindingUpdate in bindingUpdates)
                    {
                        SubHandler.FireCallbacks(bindingUpdate.Binding, bindingUpdate.Value);
                    }
                }
            }
        }

        /// <summary>
        /// Factory method to convert the raw update into one or more <see cref="BindingUpdate"/>s
        /// </summary>
        /// <param name="update">The raw update</param>
        /// <returns></returns>
        protected abstract BindingUpdate[] PreProcessUpdate(TUpdate update);

        /// <summary>
        /// Allows routing of updates to whichever <see cref="IUpdateProcessor"/> is required
        /// </summary>
        /// <param name="bindingDescriptor">Describes the input that changed</param>
        /// <returns>The key for the <see cref="UpdateProcessors"/> dictionary</returns>
        protected abstract TProcessorKey GetUpdateProcessorKey(BindingDescriptor bindingDescriptor);
        //protected virtual (BindingType, int) GetUpdateProcessorKey(BindingDescriptor bindingDescriptor)
        //{
        //    return (bindingDescriptor.Type, bindingDescriptor.Index);
        //}
    }
}
