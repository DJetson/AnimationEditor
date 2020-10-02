using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationEditorCore.Commands.Timeline
{
    public class AddBlankFrameBeforeCurrentCommand : TimelineCommandBase
    {
        public override bool CanExecute(object parameter)
        {
            if (!(parameter is TimelineViewModel Parameter))
                return false;

            if (Parameter.AnimationPlaybackViewModel.IsPlaybackActive)
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            var Parameter = parameter as TimelineViewModel;

            var frameCount = Parameter?.ActiveLayer?.Frames?.Count ?? 0;
            var currentIndex = Parameter?.SelectedFrameIndex ?? 0;
            var navigation = FrameNavigation.Previous;

            Parameter.AddBlankFrameToTimeline(GetIndexForFrameNavigation(currentIndex, frameCount, navigation));
        }
    }
}
