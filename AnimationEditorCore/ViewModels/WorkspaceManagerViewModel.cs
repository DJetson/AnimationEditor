using AnimationEditorCore.Interfaces;
using AnimationEditorCore.Utilities;
using AnimationEditorCore.ViewModels.StateObjects;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AnimationEditorCore.ViewModels
{
    public class WorkspaceManagerViewModel : ViewModelBase, IHasWorkspaceCollection/*, IMementoCaretaker*/
    {
        private ObservableCollection<WorkspaceViewModel> _Workspaces;
        public ObservableCollection<WorkspaceViewModel> Workspaces
        {
            get => _Workspaces;
            set { _Workspaces = value; NotifyPropertyChanged(); }
        }

        private WorkspaceViewModel _SelectedWorkspace;
        public WorkspaceViewModel SelectedWorkspace
        {
            get => _SelectedWorkspace;
            set { _SelectedWorkspace = value; NotifyPropertyChanged(); }
        }

        public WorkspaceManagerViewModel()
        {
            InitializeDependentViewModels();
            SelectedWorkspace = Workspaces.LastOrDefault();
        }

        private void InitializeDependentViewModels()
        {
            Workspaces = new ObservableCollection<WorkspaceViewModel>();
            var newWorkspace = new WorkspaceViewModel()
            {
                DisplayName = FileUtilities.GetUniquePlaceholderName(this),
                HasUnsavedChanges = true,
                Host = this
            };

            Workspaces.Add(newWorkspace);
        }

        public WorkspaceViewModel CreateNewWorkspace()
        {
            var newWorkspace = new WorkspaceViewModel() { DisplayName = FileUtilities.GetUniquePlaceholderName(this), HasUnsavedChanges = true, Host = this };
            Workspaces.Add(newWorkspace);

            SelectedWorkspace = newWorkspace;

            return newWorkspace;
        }

        public void AddWorkspace(WorkspaceViewModel item)
        {
            if (!Workspaces.Contains(item))
            {
                item.Host = this;
                Workspaces.Add(item);
                SelectedWorkspace = item;
            }
        }

        public void RemoveWorkspace(WorkspaceViewModel workspace)
        {
            if (Workspaces.Contains(workspace))
                Workspaces.Remove(workspace);
        }

        //public List<IMemento> GetStateHistory()
        //{
        //    return SelectedWorkspace.UndoStack.ToList();
        //}

        //public Stack<IMemento> UndoStack => SelectedWorkspace?.UndoStack;
        //public Stack<IMemento> RedoStack => SelectedWorkspace?.RedoStack;

        //public IMemento PeekUndo()
        //{
        //    if (UndoStack == null || UndoStack.Count == 0)
        //        return null;

        //    return UndoStack.Peek();
        //}

        //public IMemento PeekRedo()
        //{
        //    if (RedoStack == null || RedoStack.Count == 0)
        //        return null;

        //    return RedoStack.Peek();
        //}

        ///// <summary>
        ///// This should be called BEFORE any undoable action takes place
        ///// </summary>
        ///// <param name="state">The object state</param>
        //public void AddHistoricalState(IMemento state)
        //{
        //    //var lastTimelineState = ActiveUndoStack.Where(e => e.Originator is AnimationTimelineViewModel).FirstOrDefault() as UndoStateViewModel<AnimationTimelineState>;

        //    //if(lastTimelineState != null && (lastTimelineState.State.SelectedFrame != SelectedWorkspace.AnimationTimelineViewModel.SelectedFrame))
        //    //{
        //    //    var selectionChangeState = SelectedWorkspace.AnimationTimelineViewModel.SaveState() as UndoStateViewModel<AnimationTimelineState>;
        //    //    selectionChangeState.DisplayName = "Navigate to Frame";
        //    //    AddHistoricalState(selectionChangeState);
        //    //}

        //    //if(SelectedWorkspace.AnimationTimelineViewModel.SelectedFrame != )
        //    if (RedoStack != null)
        //        RedoStack.Clear();
        //    if (UndoStack != null)
        //        UndoStack.Push(state);

        //    WorkspaceHistoryViewModel.PopulateHistory(UndoStack, RedoStack);
        //}

        //public void Undo()
        //{
        //    var revertTo = UndoStack.Pop();

        //    RedoStack.Push(revertTo.Originator.CurrentState);
        //    revertTo.Originator.LoadState(revertTo);
        //    WorkspaceHistoryViewModel.PopulateHistory(UndoStack, RedoStack);
        //}

        //public void UndoToState(IMemento state)
        //{
        //    if (!UndoStack.Contains(state))
        //        throw new IndexOutOfRangeException($"The historical state \"{state}\" could not be found");

        //    IMemento revertTo = null;

        //    do
        //    {
        //        revertTo = UndoStack.Pop();
        //        RedoStack.Push(revertTo.Originator.CurrentState);
        //        revertTo.Originator.LoadState(revertTo);

        //    } while (revertTo != state);

        //    //revertTo.Originator.LoadState(revertTo);

        //    WorkspaceHistoryViewModel.PopulateHistory(UndoStack, RedoStack);
        //}

        //public void Redo()
        //{
        //    var resumeTo = RedoStack.Pop();

        //    UndoStack.Push(resumeTo.Originator.CurrentState);
        //    resumeTo.Originator.LoadState(resumeTo);

        //    WorkspaceHistoryViewModel.PopulateHistory(UndoStack, RedoStack);
        //}

        //public void RedoToState(IMemento state)
        //{
        //    if (!RedoStack.Contains(state))
        //        throw new IndexOutOfRangeException($"The historical state \"{state}\" could not be found");

        //    IMemento resumeTo = null;

        //    do
        //    {
        //        resumeTo = RedoStack.Pop();
        //        UndoStack.Push(resumeTo.Originator.CurrentState);
        //        resumeTo.Originator.LoadState(resumeTo);

        //    } while (resumeTo != state);

        //    //revertTo.Originator.LoadState(revertTo);

        //    WorkspaceHistoryViewModel.PopulateHistory(UndoStack, RedoStack);
        //}
    }
}
