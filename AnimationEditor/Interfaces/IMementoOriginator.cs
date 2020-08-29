using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationEditor.Interfaces
{
    public interface IMementoOriginator
    {
        IMemento SaveState();
        void LoadState(IMemento state);
    }
}
