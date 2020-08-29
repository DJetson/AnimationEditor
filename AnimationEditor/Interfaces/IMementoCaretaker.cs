using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationEditor.Interfaces
{
    public interface IMementoCaretaker
    {
        List<IMemento<IMementoOriginator>> GetStateHistory();

        IMemento<IMementoOriginator> PeekUndo();
        IMemento<IMementoOriginator> PeekRedo();

        void Undo();
        void Redo();
    }
}
