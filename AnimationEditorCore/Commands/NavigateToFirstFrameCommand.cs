﻿using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Properties;
using AnimationEditorCore.ViewModels;

namespace AnimationEditorCore.Commands
{
    public class NavigateToFirstFrameCommand : RequeryBase
    {
        public override string Description => Resources.NavigateToFirstFrameDescription;
        public override string ToolTip => Resources.NavigateToFirstFrameToolTip;
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

            Parameter.SelectedFrameIndex = 0;
        }
    }
}
