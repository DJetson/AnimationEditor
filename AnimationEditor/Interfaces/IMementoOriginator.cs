using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationEditor.Interfaces
{
    public interface IMementoOriginator
    {
        IMemento<IMementoOriginator> SaveState();
        void LoadState(IMemento<IMementoOriginator> state);
    }
}
