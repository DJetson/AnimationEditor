using AnimationEditor.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationEditor.ViewModels
{
    public enum HistoryStateType { Undo, Current, Redo};
    public class WorkspaceHistoryItemViewModel : ViewModelBase
    {
        private HistoryStateType _StateType;
        public HistoryStateType StateType
        {
            get { return _StateType; }
            set { _StateType = value; NotifyPropertyChanged(); }
        }

        private UndoStateViewModel _State;
        public UndoStateViewModel State
        {
            get { return _State; }
            set { _State = value; NotifyPropertyChanged(); }
        }

        public WorkspaceHistoryItemViewModel(UndoStateViewModel state)
        {
            State = state;
            //InitializeCommands();
        }
    }
}
