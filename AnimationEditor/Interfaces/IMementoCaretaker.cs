using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationEditor.Interfaces
{
    public interface IMementoCaretaker
    {
        List<IMemento> GetStateHistory();

        Stack<IMemento> UndoStack { get; }
        Stack<IMemento> RedoStack { get; }

        IMemento PeekUndo();
        IMemento PeekRedo();

        void AddHistoricalState(IMemento state);
        void Undo();
        void UndoToState(IMemento state);
        void Redo();
        void RedoToState(IMemento state);
    }
}
