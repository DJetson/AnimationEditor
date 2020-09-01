using AnimationEditor.BaseClasses;
using AnimationEditor.Interfaces;
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


        //private UndoStateViewModel _CurrentState;
        //public UndoStateViewModel CurrentState
        //{
        //    get { return PeekUndo() as UndoStateViewModel; }

        //}

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

            //NOTE: I think this whole concept might be missing from the main undo manager. The Current State really needs to be stored
            // and updated in the IMementoCaretaker immediately when a state is pushed onto the undo or redo stack
            //var currentState = new WorkspaceHistoryItemViewModel(undoRange.Last().State.Originator.CurrentState as UndoStateViewModel) { IsUndoable = false };
            //undoRange.Add(currentState);
            SelectedItem = undoRange.Where(e => e.State == CurrentState).FirstOrDefault();
            SelectedItem.StateType = HistoryStateType.Current;
            //undoRange.Add(SelectedItem);
            var redoList = redoStack.ToList();
            redoList.Reverse();
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

        public void SelectHistoricalState(UndoStateViewModel state)
        {
            if (PeekUndo() == (state as IMemento))
            {
                Undo();
            }
            else if (UndoStack.Contains(state as IMemento))
            {
                UndoToState(state);
            }
            else if (PeekRedo() == (state as IMemento))
            {
                Redo();
            }
            else if (RedoStack.Contains(state as IMemento))
            {
                RedoToState(state);
            }
            else
            {
                AddHistoricalState(state);
            }
        }

        /// <summary>
        /// This should be called BEFORE any undoable action takes place
        /// </summary>
        /// <param name="state">The object state</param>
        public void AddHistoricalState(IMemento state)
        {
            //if (RedoStack != null)
            RedoStack.Clear();
            //if (UndoStack != null)
            UndoStack.Push(state);

            CurrentState = UndoStack.Peek() as UndoStateViewModel;
            //            NotifyPropertyChanged(nameof(CurrentState));

            PopulateHistory(UndoStack, RedoStack);
        }

        public void Undo()
        {
            //var current = UndoStack.Pop();

            //            if (current != CurrentState)
            //              Console.WriteLine($"Error: Undo.Top = [{current}] and CurrentState = [{CurrentState}]. These should always match. If they don't, it means the history is corrupted.");

            //var CurrentState= UndoStack.Pop();
            RedoStack.Push(UndoStack.Pop());
            //UndoStack.Pop();
            CurrentState = UndoStack.Peek() as UndoStateViewModel;
            //NotifyPropertyChanged(nameof(CurrentState));
            CurrentState.Originator.LoadState(CurrentState);

            PopulateHistory(UndoStack, RedoStack);
        }

        public void UndoToState(IMemento state)
        {
            if (!UndoStack.Contains(state))
                throw new IndexOutOfRangeException($"The historical state \"{state}\" could not be found");

            //IMemento revertTo = null;

            while (CurrentState != state)
            {
                RedoStack.Push(UndoStack.Pop());
                CurrentState = UndoStack.Peek() as UndoStateViewModel;
                CurrentState.Originator.LoadState(CurrentState);
                //NotifyPropertyChanged(nameof(CurrentState));
            }


            //revertTo.Originator.LoadState(revertTo);

            PopulateHistory(UndoStack, RedoStack);
        }

        public void Redo()
        {
            //var resumeTo = RedoStack.Pop();

            UndoStack.Push(RedoStack.Pop());
            CurrentState = UndoStack.Peek() as UndoStateViewModel;
            //NotifyPropertyChanged(nameof(CurrentState));
            CurrentState.Originator.LoadState(CurrentState);

            PopulateHistory(UndoStack, RedoStack);
        }

        public void RedoToState(IMemento state)
        {
            if (!RedoStack.Contains(state))
                throw new IndexOutOfRangeException($"The historical state \"{state}\" could not be found");

            while (CurrentState != state)
            {
                UndoStack.Push(RedoStack.Pop());
                //UndoStack.Push(resumeTo.Originator.CurrentState);
                CurrentState = UndoStack.Peek() as UndoStateViewModel;
                CurrentState.Originator.LoadState(CurrentState);
                //NotifyPropertyChanged(nameof(CurrentState));
            }

            //revertTo.Originator.LoadState(revertTo);

            PopulateHistory(UndoStack, RedoStack);
        }
    }
}
