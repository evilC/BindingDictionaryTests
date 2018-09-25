using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BindingDictionaryTestTwo.Polling
{
    /// <summary>
    /// Interface for handling Button/Axis value normalization, POV physical to logical values etc
    /// </summary>
    public interface IUpdateProcessor
    {
        // Converts an update for one physical input into one or more updates
        // Eg POV changing angle from 0 to 90 deg would cause two updates - North release and East press
        // In most other cases, the array will be of 1 length
        BindingUpdate[] Process(BindingUpdate update);
    }
}
