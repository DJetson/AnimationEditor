﻿using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Commands.BaseClasses;
using AnimationEditorCore.Properties;
using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationEditorCore.Commands.Timeline.Frames
{
    public class AddBlankFrameAfterCurrentCommand : TimelineCommandBase
    {
        public override string Description => Resources.AddBlankFrameAfterCurrentDescription;
        public override string ToolTip => Resources.AddBlankFrameAfterCurrentToolTip;
        public override string UndoStateTitle => Resources.AddBlankFrameAfterCurrentUndoStateTitle;
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
            var navigation = FrameNavigation.Next;

            Parameter.AddBlankFrameToTimeline(GetIndexForFrameNavigation(currentIndex, frameCount, navigation));
            WorkspaceHistoryViewModel.PushUndoRecord(UndoStateTitle);
        }
    }
}
