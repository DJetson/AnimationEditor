﻿using AnimationEditor.Interfaces;
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
            return SelectedWorkspace.UndoStack.ToList();
        }

        public Stack<IMemento> ActiveUndoStack => SelectedWorkspace.UndoStack;
        public Stack<IMemento> ActiveRedoStack => SelectedWorkspace.RedoStack;

        public IMemento PeekUndo()
        {
            if (ActiveUndoStack == null || ActiveUndoStack.Count == 0)
                return null;

            return ActiveUndoStack.Peek();
        }

        public IMemento PeekRedo()
        {
            if (ActiveRedoStack == null || ActiveRedoStack.Count == 0)
                return null;

            return ActiveRedoStack.Peek();
        }

        /// <summary>
        /// This should be called BEFORE any undoable action takes place
        /// </summary>
        /// <param name="state">The object state</param>
        public void AddHistoricalState(IMemento state)
        {
            ActiveRedoStack.Clear();
            ActiveUndoStack.Push(state);
        }

        public void Undo()
        {
            var revertTo = ActiveUndoStack.Pop();

            ActiveRedoStack.Push(revertTo.Originator.SaveState());
            revertTo.Originator.LoadState(revertTo);
        }

        public void Redo()
        {
            var resumeTo = ActiveRedoStack.Pop();

            ActiveUndoStack.Push(resumeTo.Originator.SaveState());
            resumeTo.Originator.LoadState(resumeTo);
        }
    }
}
