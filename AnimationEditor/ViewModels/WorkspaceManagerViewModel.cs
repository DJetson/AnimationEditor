using AnimationEditor.Interfaces;
using AnimationEditor.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AnimationEditor.ViewModels
{
    public class WorkspaceManagerViewModel : ViewModelBase, IHasWorkspaceCollection, IMementoCaretaker
    {
        private ObservableCollection<WorkspaceViewModel> _Workspaces = new ObservableCollection<WorkspaceViewModel>();
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

        private Stack<IMemento> _UndoStack = new Stack<IMemento>();
        private Stack<IMemento> _RedoStack = new Stack<IMemento>();

        public WorkspaceManagerViewModel()
        {
            SelectedWorkspace = CreateNewWorkspace();
            //SelectedWorkspace = Workspaces.FirstOrDefault();
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

        public List<IMemento> GetStateHistory()
        {
            return _UndoStack.ToList();
        }

        public IMemento PeekUndo()
        {
            if (_UndoStack == null || _UndoStack.Count == 0)
                return null;

            return _UndoStack.Peek();
        }

        public IMemento PeekRedo()
        {
            if (_RedoStack == null || _RedoStack.Count == 0)
                return null;

            return _RedoStack.Peek();
        }

        /// <summary>
        /// This should be called BEFORE any undoable action takes place
        /// </summary>
        /// <param name="state">The object state</param>
        public void AddHistoricalState(IMemento state)
        {
            _RedoStack.Clear();
            _UndoStack.Push(state);
        }

        public void Undo()
        {
            var revertTo = _UndoStack.Pop();

            _RedoStack.Push(revertTo.Originator.SaveState());
            revertTo.Originator.LoadState(revertTo);
        }

        public void Redo()
        {
            var resumeTo = _RedoStack.Pop();

            _UndoStack.Push(resumeTo.Originator.SaveState());
            resumeTo.Originator.LoadState(resumeTo);
        }
    }
}
