﻿using BindingDictionaryTestTwo.Updates;

namespace BindingDictionaryTestTwo.Providers.XInput
{
    public class XiButtonProcessor : IUpdateProcessor
    {
        public BindingUpdate[] Process(BindingUpdate update)
        {
            return new[] { update };
        }
    }

    public class XiAxisProcessor : IUpdateProcessor
    {
        public BindingUpdate[] Process(BindingUpdate update)
        {
            return new[] { update };
        }
    }

    public class XiTriggerProcessor : IUpdateProcessor
    {
        public BindingUpdate[] Process(BindingUpdate update)
        {
            update.Value = (update.Value * 257) - 32768;
            return new[] { update };
        }
    }
}
