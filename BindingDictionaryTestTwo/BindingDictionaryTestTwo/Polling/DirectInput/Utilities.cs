using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX.DirectInput;

namespace BindingDictionaryTestTwo.Polling.DirectInput
{
    public static class Utilities
    {
        public static BindingType OffsetToType(JoystickOffset offset)
        {
            int index = (int)offset;
            if (index <= (int)JoystickOffset.Sliders1) return BindingType.Axis;
            if (index <= (int)JoystickOffset.PointOfViewControllers3) return BindingType.POV;
            return BindingType.Button;
        }


    }
}
