using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Properties;
using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationEditorCore.Commands.Timeline
{
    public class InsertDuplicateFrameAfterCurrentCommand : TimelineCommandBase
    {
        public override string Description => Resources.InsertDuplicateFrameAfterCurrentDescription;
        public override string ToolTip => Resources.InsertDuplicateFrameAfterCurrentToolTip;
        public override string UndoStateTitle => Resources.InsertDuplicateFrameAfterCurrentUndoStateTitle;
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
            var navigation = FrameNavigation.Next;

            Parameter.DuplicateCurrentFrameToTimeline(GetIndexForFrameNavigation(currentIndex, frameCount, navigation));
        }
    }
}
