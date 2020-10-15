using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Commands;
using AnimationEditorCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AnimationEditorCore.ViewModels
{
    public class WorkspaceHistoryViewModel : ViewModelBase/*, IMementoCaretaker*/
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

            if (WorkspaceViewModel.TimelineViewModel.AnimationPlaybackViewModel.IsPlaybackActive)
                return false;

            if (UndoRedoHistory.Contains(Parameter.State) == false)
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

            if (WorkspaceViewModel.TimelineViewModel.AnimationPlaybackViewModel.IsPlaybackActive)
                return false;

            if (UndoRedoHistory.Contains(Parameter.State) == false)
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
            set
            {
                _CurrentState = value;
                NotifyPropertyChanged(nameof(CurrentState),
                                      nameof(CurrentStateIndex),
                                      nameof(PreviousState),
                                      nameof(NextState));

                if (WorkspaceViewModel != null)
                {
                    WorkspaceViewModel.HasUnsavedChanges = (_CurrentState != InitialState);
                }
            }
        }

        private List<UndoStateViewModel> _UndoRedoHistory = new List<UndoStateViewModel>();
        public List<UndoStateViewModel> UndoRedoHistory
        {
            get { return _UndoRedoHistory; }
            set { _UndoRedoHistory = value; NotifyPropertyChanged(); }
        }

        public int CurrentStateIndex
        {
            get
            {
                if (!UndoRedoHistory.Contains(CurrentState))
                    return -1;

                return UndoRedoHistory.IndexOf(CurrentState);
            }
        }

        public UndoStateViewModel PreviousState
        {
            get
            {
                if (CurrentStateIndex > 0)
                    return UndoRedoHistory[CurrentStateIndex - 1];
                else
                    return null;
            }
        }

        public UndoStateViewModel NextState
        {
            get
            {
                if (CurrentStateIndex >= 0 && CurrentStateIndex < UndoRedoHistory.Count - 1)
                    return UndoRedoHistory[CurrentStateIndex + 1];
                else
                    return null;
            }
        }

        public void InitializeCommands()
        {
            RevertToState = new DelegateCommand(RevertToState_CanExecute, RevertToState_Execute);
            ResumeToState = new DelegateCommand(ResumeToState_CanExecute, ResumeToState_Execute);
        }

        private UndoStateViewModel _InitialState;
        public UndoStateViewModel InitialState
        {
            get { return _InitialState; }
            set
            {
                _InitialState = value;
                NotifyPropertyChanged();
                if (WorkspaceViewModel != null)
                {
                    WorkspaceViewModel.HasUnsavedChanges = (CurrentState != _InitialState);
                }
            }
        }


        public WorkspaceHistoryViewModel(WorkspaceViewModel workspace, UndoStateViewModel initialState = null)
        {
            WorkspaceViewModel = workspace;

            InitialState = initialState;
            InitializeCommands();
        }

        public void PopulateHistory()
        {
            var undoRange = new List<WorkspaceHistoryItemViewModel>();
            foreach (UndoStateViewModel item in UndoRedoHistory)
            {
                if (UndoRedoHistory.IndexOf(item) < CurrentStateIndex)
                    undoRange.Add(new WorkspaceHistoryItemViewModel(item) { StateType = HistoryStateType.Undo });
                else if (UndoRedoHistory.IndexOf(item) > CurrentStateIndex)
                    undoRange.Add(new WorkspaceHistoryItemViewModel(item) { StateType = HistoryStateType.Redo });
                else
                {
                    var selected = new WorkspaceHistoryItemViewModel(item) { StateType = HistoryStateType.Current };
                    undoRange.Add(selected);
                    SelectedItem = selected;
                }
            }

            HistoricalStates.Clear();

            HistoricalStates = new ObservableCollection<WorkspaceHistoryItemViewModel>(undoRange/*.Concat(redoRange)*/);

        }

        //private Stack<IMemento> _UndoStack = new Stack<IMemento>();
        //public Stack<IMemento> UndoStack
        //{
        //    get { return _UndoStack; }
        //    set { _UndoStack = value; NotifyPropertyChanged(); }
        //}

        //private Stack<IMemento> _RedoStack = new Stack<IMemento>();
        //public Stack<IMemento> RedoStack
        //{
        //    get { return _RedoStack; }
        //    set { _RedoStack = value; NotifyPropertyChanged(); }
        //}

        //public IMemento PeekUndo()
        //{
        //    if (UndoStack.Count == 0)
        //        return null;

        //    return UndoStack.Peek();
        //}

        //public IMemento PeekRedo()
        //{
        //    if (RedoStack.Count == 0)
        //        return null;

        //    return RedoStack.Peek();
        //}

        public void AddHistoricalState(IMemento state, bool raiseChangedFlag = true)
        {
            ClearFutureStates(); //new undo architecture

            UndoRedoHistory.Add(state as UndoStateViewModel); // new undo architecture

            CurrentState = state as UndoStateViewModel;//NextState; //new undo architecture

            PopulateHistory(); //new undo architecture

            if (raiseChangedFlag)
            {
                WorkspaceViewModel.HasUnsavedChanges = raiseChangedFlag;
                WorkspaceViewModel.WriteToTempFile();
            }
        }

        public void ClearFutureStates()
        {
            var futureStates = UndoRedoHistory.Where(e => UndoRedoHistory.IndexOf(e) > CurrentStateIndex).ToList();

            foreach (var state in futureStates)
            {
                UndoRedoHistory.Remove(state);
            }
        }

        public void Undo()
        {
            if (PreviousState == null)
                return;

            CurrentState = PreviousState; 
            CurrentState.LoadState();

            PopulateHistory();
            WorkspaceViewModel.WriteToTempFile();
        }

        public void UndoToState(IMemento state)
        {
            if (!UndoRedoHistory.Contains(state))
                throw new IndexOutOfRangeException($"The historical state \"{state}\" could not be found");

            var originalCurrentState = CurrentState;
            while (CurrentState != state)
            {
                if (PreviousState == null)
                {
                    CurrentState = originalCurrentState;
                    return;
                }

                CurrentState = PreviousState; //new undo architecture;
            }

            CurrentState.LoadState();

            PopulateHistory();
            WorkspaceViewModel.WriteToTempFile();
        }

        public void Redo()
        {
            if (NextState == null)
                return;

            CurrentState = NextState;
            CurrentState.LoadState();

            PopulateHistory();
            WorkspaceViewModel.WriteToTempFile();
        }

        public void RedoToState(IMemento state)
        {
            if (!UndoRedoHistory.Contains(state))
                throw new IndexOutOfRangeException($"The historical state \"{state}\" could not be found");

            var originalCurrentState = CurrentState;
            while (CurrentState != state)
            {
                if (NextState == null)
                {
                    CurrentState = originalCurrentState;
                    return;
                }
                CurrentState = NextState;
            }

            CurrentState.LoadState();

            PopulateHistory();
            WorkspaceViewModel.WriteToTempFile();
        }
    }
}
