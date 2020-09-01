using AnimationEditor.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationEditor.ViewModels
{
    public class WorkspaceHistoryItemViewModel : ViewModelBase
    {
        private bool _IsUndoable;
        public bool IsUndoable
        {
            get { return _IsUndoable; }
            set { _IsUndoable = value; NotifyPropertyChanged(); }
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
