using System;
using System.Collections.Generic;
using BindingDictionaryTestTwo.Subscriptions;

namespace BindingDictionaryTestTwo.Polling
{
    public abstract class DevicePollHandler<TPollType>
    {
        private readonly DeviceDescriptor _deviceDescriptor;
        protected ISubscriptionHandler SubHandler;
        protected DetectionMode DetectionMode = DetectionMode.Subscription;
        protected Dictionary<(BindingType, int), IPollProcessor> PollProcessors = new Dictionary<(BindingType, int), IPollProcessor>();

        public EventHandler<BindModeUpdate> BindModeUpdate;

        protected DevicePollHandler(DeviceDescriptor deviceDescriptor, ISubscriptionHandler subhandler)
        {
            _deviceDescriptor = deviceDescriptor;
            SubHandler = subhandler;
        }

        public void SetDetectionMode(DetectionMode mode)
        {
            DetectionMode = mode;
        }

        protected void OnBindModeUpdate(BindingUpdate update)
        {
            BindModeUpdate?.Invoke(this, new BindModeUpdate{Device = _deviceDescriptor, Binding = update.Binding, Value = update.Value});
        }

        //public abstract void Poll(TPollType pollData);
        public virtual void Poll(TPollType pollData)
        {
            var preprocessedUpdates = PreProcess(pollData);
            var bindMode = DetectionMode == DetectionMode.Bind;
            foreach (var preprocessedUpdate in preprocessedUpdates)
            {
                var isSubscribed = SubHandler.ContainsKey(preprocessedUpdate.Binding.Type, preprocessedUpdate.Binding.Index);

                if (!(bindMode || isSubscribed)) return;

                // Fire poll processors
                var bindingUpdates = PollProcessors[GetPollProcessorKey(preprocessedUpdate.Binding)].Process(preprocessedUpdate);
                if (bindMode)
                {
                    foreach (var bindingUpdate in bindingUpdates)
                    {
                        OnBindModeUpdate(bindingUpdate);
                    }
                }
                else
                {
                    //SubHandler.FireCallbacks(new Binding{Index = bindingUpdate.Index, Type = bindingUpdate.Type}, bindingUpdate.Value); // short cut test
                    foreach (var bindingUpdate in bindingUpdates)
                    {
                        SubHandler.FireCallbacks(bindingUpdate.Binding, bindingUpdate.Value);
                    }
                }
            }
        }


        protected abstract BindingUpdate[] PreProcess(TPollType pollData);

        protected virtual (BindingType, int) GetPollProcessorKey(BindingDescriptor bindingDescriptor)
        {
            return (bindingDescriptor.Type, bindingDescriptor.Index);
        }
    }
}
