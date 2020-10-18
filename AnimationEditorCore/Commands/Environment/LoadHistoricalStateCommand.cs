using AnimationEditorCore.Commands.BaseClasses;
using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationEditorCore.Commands.Environment
{
    public class LoadHistoricalStateCommand : RequeryBase
    {
        public override bool CanExecute(object parameter)
        {
            if (!(parameter is UndoStateViewModel Parameter))
                return false;

            if (Parameter?.Originator?.WorkspaceViewModel?.WorkspaceHistoryViewModel?.HistoricalStates?.Contains(Parameter) == false)
                return false;

            if (Parameter?.Originator?.AnimationPlaybackViewModel == null)
                return false;

            if (Parameter?.Originator?.AnimationPlaybackViewModel?.IsPlaybackActive == true)
                return false;

            return true;
        }
        public override void Execute(object parameter)
        {
            var Parameter = parameter as UndoStateViewModel;

            if (Parameter.CurrentStateType == StateType.Undo)
                WorkspaceHistoryViewModel.UndoToState(Parameter);
            else if (Parameter.CurrentStateType == StateType.Redo)
                WorkspaceHistoryViewModel.RedoToState(Parameter);
            else
                return;
        }
    }
}
