using AnimationEditor.BaseClasses;
using AnimationEditor.Interfaces;
using AnimationEditor.ViewModels.StateObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationEditor.ViewModels
{
    public class WorkspaceHistoryViewModel : ViewModelBase, IMementoCaretaker
    {
        public override string DisplayName => "Workspace History";

        private ObservableCollection<WorkspaceHistoryItemViewModel> _HistoricalStates = new ObservableCollection<WorkspaceHistoryItemViewModel>();
        public ObservableCollection<WorkspaceHistoryItemViewModel> HistoricalStates
        {
            get => _HistoricalStates;
            set { _HistoricalStates = value; NotifyPropertyChanged(); }
        }

        private WorkspaceHistoryItemViewModel _SelectedItem;
        public WorkspaceHistoryItemViewModel SelectedItem
        {
            get { return _SelectedItem; }
            set { _SelectedItem = value; NotifyPropertyChanged(); }
        }

        private WorkspaceViewModel _WorkspaceViewModel;
        public WorkspaceViewModel WorkspaceViewModel
        {
            get { return _WorkspaceViewModel; }
            set { _WorkspaceViewModel = value; NotifyPropertyChanged(); }
        }

        private DelegateCommand _RevertToState;
        public DelegateCommand RevertToState
        {
            get { return _RevertToState; }
            set { _RevertToState = value; NotifyPropertyChanged(); }
        }

        private bool RevertToState_CanExecute(object parameter)
        {
            if (!(parameter is WorkspaceHistoryItemViewModel Parameter))
                return false;

            if (UndoStack?.Contains(Parameter.State) == false)
                return false;

            return true;
        }

        private void RevertToState_Execute(object parameter)
        {
            var Parameter = parameter as WorkspaceHistoryItemViewModel;

            UndoToState(Parameter.State);
        }

        private DelegateCommand _ResumeToState;
        public DelegateCommand ResumeToState
        {
            get { return _ResumeToState; }
            set { _ResumeToState = value; NotifyPropertyChanged(); }
        }

        private bool ResumeToState_CanExecute(object parameter)
        {
            if (!(parameter is WorkspaceHistoryItemViewModel Parameter))
                return false;

            if (RedoStack.Contains(Parameter.State) == false)
                return false;

            return true;
        }

        private void ResumeToState_Execute(object parameter)
        {
            var Parameter = parameter as WorkspaceHistoryItemViewModel;

            RedoToState(Parameter.State);
        }

        private UndoStateViewModel _CurrentState;
        public UndoStateViewModel CurrentState
        {
            get { return _CurrentState; }
            set { _CurrentState = value; NotifyPropertyChanged(); }
        }

        public void InitializeCommands()
        {
            RevertToState = new DelegateCommand(RevertToState_CanExecute, RevertToState_Execute);
            ResumeToState = new DelegateCommand(ResumeToState_CanExecute, ResumeToState_Execute);
        }

        public WorkspaceHistoryViewModel(WorkspaceViewModel workspace)
        {
            WorkspaceViewModel = workspace;

            InitializeCommands();
        }

        public void PopulateHistory(Stack<IMemento> undoStack, Stack<IMemento> redoStack)
        {
            var undoList = undoStack.ToList();
            undoList.Reverse();
            var undoRange = new List<WorkspaceHistoryItemViewModel>();
            foreach (UndoStateViewModel item in undoList)
            {
                undoRange.Add(new WorkspaceHistoryItemViewModel(item) { StateType = HistoryStateType.Undo });
            }

            SelectedItem = undoRange.Where(e => e.State == CurrentState).FirstOrDefault();
            SelectedItem.StateType = HistoryStateType.Current;
            var redoList = redoStack.ToList();
            //redoList.Reverse();
            var redoRange = new List<WorkspaceHistoryItemViewModel>();
            foreach (UndoStateViewModel item in redoList)
            {
                redoRange.Add(new WorkspaceHistoryItemViewModel(item) { StateType = HistoryStateType.Redo });
            }

            HistoricalStates.Clear();

            HistoricalStates = new ObservableCollection<WorkspaceHistoryItemViewModel>(undoRange.Concat(redoRange));
        }

        public List<IMemento> GetStateHistory()
        {
            return UndoStack.ToList();
        }

        private Stack<IMemento> _UndoStack = new Stack<IMemento>();
        public Stack<IMemento> UndoStack
        {
            get { return _UndoStack; }
            set { _UndoStack = value; NotifyPropertyChanged(); }
        }


        private Stack<IMemento> _RedoStack = new Stack<IMemento>();
        public Stack<IMemento> RedoStack
        {
            get { return _RedoStack; }
            set { _RedoStack = value; NotifyPropertyChanged(); }
        }

        public IMemento PeekUndo()
        {
            if (UndoStack.Count == 0)
                return null;

            return UndoStack.Peek();
        }

        public IMemento PeekRedo()
        {
            if (RedoStack.Count == 0)
                return null;

            return RedoStack.Peek();
        }

        public void AddHistoricalState(IMemento state)
        {
            RedoStack.Clear();
            UndoStack.Push(state);

            CurrentState = UndoStack.Peek() as UndoStateViewModel;

            PopulateHistory(UndoStack, RedoStack);
        }

        public UndoStateViewModel GetLastStateChangeForType(Type type)
        {
            foreach(var item in UndoStack.ToList())
            {
                if (item.GetType() == type)
                {
                    return item as UndoStateViewModel;
                }
                else if(item.GetType() == typeof(MultiState))
                {
                    var state = (item as MultiState).GetStateOfType(type);
                    if (state != null)
                    {
                        return state;
                    }
                }
            }
            return null;
        }

        public void Undo()
        {
            var currentState = UndoStack.Pop() as UndoStateViewModel;

            RedoStack.Push(currentState);
            CurrentState = UndoStack.Peek() as UndoStateViewModel;

            if (currentState is MultiState)
            {
                foreach (var item in (currentState as MultiState).States)
                {
                    GetLastStateChangeForType(item.GetType()).LoadState();
                }
            }
            else
            {
                CurrentState.LoadState();
            }

            PopulateHistory(UndoStack, RedoStack);
        }

        public void UndoToState(IMemento state)
        {
            if (!UndoStack.Contains(state))
                throw new IndexOutOfRangeException($"The historical state \"{state}\" could not be found");

            while (CurrentState != state)
            {
                Undo();
            }
        }

        public void Redo()
        {
            UndoStack.Push(RedoStack.Pop());
            CurrentState = UndoStack.Peek() as UndoStateViewModel;
            CurrentState.LoadState();// Originator.LoadState(CurrentState);

            PopulateHistory(UndoStack, RedoStack);
        }

        public void RedoToState(IMemento state)
        {
            if (!RedoStack.Contains(state))
                throw new IndexOutOfRangeException($"The historical state \"{state}\" could not be found");

            while (CurrentState != state)
            {
                UndoStack.Push(RedoStack.Pop());
                CurrentState = UndoStack.Peek() as UndoStateViewModel;
                CurrentState.LoadState();//.Originator.LoadState(CurrentState);
            }

            PopulateHistory(UndoStack, RedoStack);
        }
    }
}
