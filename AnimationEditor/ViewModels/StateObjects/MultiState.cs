﻿using AnimationEditor.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationEditor.ViewModels.StateObjects
{
    public class MultiState : UndoStateViewModel
    {
        private ObservableCollection<UndoStateViewModel> _States = new ObservableCollection<UndoStateViewModel>();
        public ObservableCollection<UndoStateViewModel> States
        {
            get { return _States; }
            set { _States = value; NotifyPropertyChanged(); }
        }

        public MultiState(IMementoOriginator viewModel, string stateName = "", params UndoStateViewModel [] dependentStates) : base(null, stateName)
        {
            foreach (var state in dependentStates)
            {
                States.Add(state);
            }
        }

        public override void LoadState()
        {
            foreach (var state in States)
            {
                state.LoadState();
            }
        }

        public UndoStateViewModel GetStateOfType(Type type)
        {
            foreach(var state in States)
            {
                if (state is MultiState)
                {
                    return GetStateOfType(type);
                }
                else if (state.GetType() == type)
                    return state;
            }
            return null;
        }
    }
}
