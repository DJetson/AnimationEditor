using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationEditor.Interfaces
{
    public interface IMemento<T> where T : IMementoOriginator
    {
        IMementoOriginator Originator { get; }
    }
}
