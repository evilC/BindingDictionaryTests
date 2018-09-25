using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BindingDictionaryTestTwo.Polling.Interception
{
    public class IcepKbPollProcessor : IPollProcessor
    {
        public BindingUpdate[] Process(BindingUpdate update)
        {
            var value = update.Value;
            return new[] { new BindingUpdate { Binding = new BindingDescriptor { Type = update.Binding.Type, Index = update.Binding.Index, SubIndex = update.Binding.SubIndex }, Value = value } };
        }
    }
}
