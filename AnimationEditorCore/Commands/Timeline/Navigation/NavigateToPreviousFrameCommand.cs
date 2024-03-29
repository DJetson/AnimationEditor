﻿using AnimationEditorCore.Commands.BaseClasses;
using AnimationEditorCore.Properties;
using AnimationEditorCore.ViewModels;
using System;

namespace AnimationEditorCore.Commands.Timeline.Navigation
{
    public class NavigateToPreviousFrameCommand : RequeryBase
    {
        public override string Description => Resources.NavigateToPreviousFrameDescription;
        public override string ToolTip => Resources.NavigateToPreviousFrameToolTip;
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

            Parameter.SelectedFrameIndex = Math.Max(Parameter.SelectedFrameIndex - 1, 0);
        }
    }
}
