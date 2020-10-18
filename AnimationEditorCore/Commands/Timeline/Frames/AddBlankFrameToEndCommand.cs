using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Commands.BaseClasses;
using AnimationEditorCore.Properties;
using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationEditorCore.Commands.Timeline.Frames
{
    public class AddBlankFrameToEndCommand : TimelineCommandBase
    {
        public override string Description => Resources.AddBlankFrameToEndDescription;
        public override string ToolTip => Resources.AddBlankFrameToEndToolTip;
        public override string UndoStateTitle => Resources.AddBlankFrameToEndUndoStateTitle; 
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

            var frameCount = Parameter?.Layers?.ActiveLayer?.Frames?.Count ?? 0;
            var currentIndex = Parameter?.SelectedFrameIndex ?? 0;
            var navigation = FrameNavigation.End;

            Parameter.AddBlankFrameToTimeline(GetIndexForFrameNavigation(currentIndex, frameCount, navigation));
            WorkspaceHistoryViewModel.PushUndoRecord(UndoStateTitle);
        }
    }
}
