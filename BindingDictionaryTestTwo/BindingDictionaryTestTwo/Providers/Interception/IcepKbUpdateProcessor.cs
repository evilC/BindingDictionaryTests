using BindingDictionaryTestTwo.Updates;

namespace BindingDictionaryTestTwo.Providers.Interception
{
    public class IcepKbUpdateProcessor : IUpdateProcessor
    {
        public BindingUpdate[] Process(BindingUpdate update)
        {
            var value = update.Value;
            return new[] { new BindingUpdate { Binding = new BindingDescriptor { Type = update.Binding.Type, Index = update.Binding.Index, SubIndex = update.Binding.SubIndex }, Value = value } };
        }
    }
}
