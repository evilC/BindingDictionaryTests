using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BindingDictionaryTestTwo.Polling
{
    public interface IUpdateProcessor
    {
        BindingUpdate[] Process(BindingUpdate update);
    }
}
