using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.ViewModels;
using System;

namespace AnimationEditorCore.Commands
{
    public class NavigateToNextFrameCommand : RequeryBase
    {
        public override bool CanExecute(object parameter)
        {
            if (!(parameter is TimelineViewModel Parameter))
            {
                return false;
            }

            if (Parameter.AnimationPlaybackViewModel.IsPlaybackActive)
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            var Parameter = parameter as TimelineViewModel;

            Parameter.SelectedFrameIndex = Math.Min(Parameter.SelectedFrameIndex + 1, Parameter.LastFrameIndex);
        }
    }
}
