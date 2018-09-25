using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BindingDictionaryTestTwo.Polling.DirectInput
{
    public class DiButtonProcessor : IPollProcessor
    {
        public BindingUpdate[] Process(BindingUpdate update)
        {
            var value = update.Value == 128 ? 1 : 0;
            return new[] { new BindingUpdate{Binding = new BindingDescriptor { Type = update.Binding.Type, Index = update.Binding.Index, SubIndex = 0 }, Value = value} };
        }
    }

    public class DiAxisProcessor : IPollProcessor
    {
        public BindingUpdate[] Process(BindingUpdate update)
        {
            var value = (65535 - update.Value) - 32768;
            return new[] { new BindingUpdate{Binding = new BindingDescriptor { Type = update.Binding.Type, Index = update.Binding.Index, SubIndex = 0 }, Value = value} };
        }
    }

    public class DiPoVProcessor : IPollProcessor
    {
        private int _currentValue = -1;
        private readonly int[] _directionStates = { 0, 0, 0, 0 };

        public BindingUpdate[] Process(BindingUpdate update)
        {
            return GenerateBindingUpdates(update).ToArray();
        }

        public List<BindingUpdate> GenerateBindingUpdates(BindingUpdate update)
        {
            var ret = new List<BindingUpdate>();
            var newAngle = update.Value;
            if (_currentValue == newAngle) return ret;
            _currentValue = newAngle;
            for (var i = 0; i < 4; i++)
            {
                var currentDirectionState = _directionStates[i];
                var newDirectionState =
                    newAngle == -1 ? 0
                        : PovHelper.StateFromAngle(newAngle, i * 9000);

                if (newDirectionState == currentDirectionState) continue;

                _directionStates[i] = newDirectionState;
                ret.Add(new BindingUpdate
                {
                    Value = newDirectionState,
                    Binding = new BindingDescriptor
                    {
                        Type = BindingType.POV,
                        Index = update.Binding.Index,
                        SubIndex = i
                    }
                });
            }

            return ret;
        }

    }
}
