﻿using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Commands;
using AnimationEditorCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AnimationEditorCore.ViewModels
{
    public class WorkspaceHistoryViewModel : ViewModelBase
    {
        public override string DisplayName => "Workspace History";

        private WorkspaceViewModel _WorkspaceViewModel;
        public WorkspaceViewModel WorkspaceViewModel
        {
            get { return _WorkspaceViewModel; }
            set { _WorkspaceViewModel = value; NotifyPropertyChanged(); }
        }

        private UndoStateViewModel _CurrentState;
        public UndoStateViewModel CurrentState
        {
            get { return _CurrentState; }
            set
            {
                SetCurrentState(value);
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

        private void SetCurrentState(UndoStateViewModel value)
        {
            //Update outgoing CurrentState.
            if (_CurrentState != null)
            {
                if (value?.CurrentStateType == StateType.Undo)
                    _CurrentState.CurrentStateType = StateType.Redo;
                else if (value?.CurrentStateType == StateType.Redo)
                    _CurrentState.CurrentStateType = StateType.Undo;
                else
                    _CurrentState.CurrentStateType = StateType.Undo;
            }

            //Set and update incoming CurrentState
            _CurrentState = value;
            _CurrentState.CurrentStateType = StateType.Current;
        }

        private ObservableCollection<UndoStateViewModel> _HistoricalStates = new ObservableCollection<UndoStateViewModel>();
        public ObservableCollection<UndoStateViewModel> HistoricalStates
        {
            get { return _HistoricalStates; }
            set { _HistoricalStates = value; NotifyPropertyChanged(); }
        }

        public int CurrentStateIndex
        {
            get
            {
                if (!HistoricalStates.Contains(CurrentState))
                    return -1;

                return HistoricalStates.IndexOf(CurrentState);
            }
        }

        public UndoStateViewModel PreviousState
        {
            get
            {
                if (CurrentStateIndex > 0)
                    return HistoricalStates[CurrentStateIndex - 1];
                else
                    return null;
            }
        }

        public UndoStateViewModel NextState
        {
            get
            {
                if (CurrentStateIndex >= 0 && CurrentStateIndex < HistoricalStates.Count - 1)
                    return HistoricalStates[CurrentStateIndex + 1];
                else
                    return null;
            }
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
        }

        public void AddHistoricalState(UndoStateViewModel state, bool raiseChangedFlag = true)
        {
            ClearFutureStates();

            HistoricalStates.Add(state as UndoStateViewModel);

            CurrentState = state as UndoStateViewModel;

            if (raiseChangedFlag)
            {
                WorkspaceViewModel.HasUnsavedChanges = raiseChangedFlag;
                WorkspaceViewModel.WriteToTempFile();
            }
        }

        public void ClearFutureStates()
        {
            var futureStates = HistoricalStates.Where(e => e.CurrentStateType == StateType.Redo).ToList();

            foreach (var state in futureStates)
            {
                HistoricalStates.Remove(state);
            }
        }

        public void Undo()
        {
            if (PreviousState == null)
                return;

            CurrentState = PreviousState;
            CurrentState.LoadState();

            WorkspaceViewModel.WriteToTempFile();
        }

        public void UndoToState(UndoStateViewModel state)
        {
            if (!HistoricalStates.Contains(state))
                throw new IndexOutOfRangeException($"The historical state \"{state}\" could not be found");

            var originalCurrentState = CurrentState;
            while (CurrentState != state)
            {
                if (PreviousState == null)
                {
                    CurrentState = originalCurrentState;
                    return;
                }

                CurrentState = PreviousState;
            }

            CurrentState.LoadState();
            WorkspaceViewModel.WriteToTempFile();
        }

        public void Redo()
        {
            if (NextState == null)
                return;

            CurrentState = NextState;
            CurrentState.LoadState();

            WorkspaceViewModel.WriteToTempFile();
        }

        public void RedoToState(UndoStateViewModel state)
        {
            if (!HistoricalStates.Contains(state))
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
            WorkspaceViewModel.WriteToTempFile();
        }
    }
}
