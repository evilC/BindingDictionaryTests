using SharpDX.DirectInput;

namespace BindingDictionaryTestTwo.Providers.DirectInput
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
