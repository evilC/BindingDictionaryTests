namespace BindingDictionaryTestOne
{
    public class BindingDescriptorDictionary<TValue> : DictionaryWrapper<(BindingType, int), TValue, DeviceDescriptor>
    {
        public TValue this[BindingDescriptor bindingDescriptor] => Dictionary[GetKey(bindingDescriptor)];

        public BindingDescriptorDictionary(DeviceDescriptor emptyEventArgs, EmptyHandler emptyHandler) : base(emptyEventArgs, emptyHandler)
        {
        }

        private static (BindingType, int) GetKey(BindingDescriptor bindingDescriptor)
        {
            return (bindingDescriptor.Type, bindingDescriptor.Index);
        }

        public TValue GetOrAdd(BindingDescriptor bindingDescriptor, TValue value)
        {
            return GetOrAdd(GetKey(bindingDescriptor), value);
        }

        public void Add(BindingDescriptor bindingDescriptor, TValue value)
        {
            Add(GetKey(bindingDescriptor), value);
        }

        public void Remove(BindingDescriptor bindingDescriptor)
        {
            Remove(GetKey(bindingDescriptor));
        }

        public bool ContainsKey(BindingDescriptor bindingDescriptor)
        {
            return ContainsKey(GetKey(bindingDescriptor));
        }

        public bool ContainsKey(BindingType bindingType, int index)
        {
            return ContainsKey((bindingType, index));
        }
    }
}
