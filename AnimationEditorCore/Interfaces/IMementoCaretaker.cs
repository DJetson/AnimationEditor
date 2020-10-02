using System.Collections.Generic;

namespace AnimationEditorCore.Interfaces
{
    public interface IMementoCaretaker
    {
        List<IMemento> GetStateHistory();

        Stack<IMemento> UndoStack { get; }
        Stack<IMemento> RedoStack { get; }

        IMemento PeekUndo();
        IMemento PeekRedo();

        void AddHistoricalState(IMemento state, bool raiseChangedFlag = true);
        void Undo();
        void UndoToState(IMemento state);
        void Redo();
        void RedoToState(IMemento state);
    }
}
