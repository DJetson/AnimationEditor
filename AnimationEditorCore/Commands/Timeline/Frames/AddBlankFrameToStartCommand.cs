﻿using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Commands.BaseClasses;
using AnimationEditorCore.Properties;
using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationEditorCore.Commands.Timeline.Frames
{
    public class AddBlankFrameToStartCommand : TimelineCommandBase
    {
        public override string Description => Resources.AddBlankFrameToStartDescription;
        public override string ToolTip => Resources.AddBlankFrameToStartToolTip;
        public override string UndoStateTitle => Resources.AddBlankFrameToStartUndoStateTitle; 
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
            var navigation = FrameNavigation.Start;

            Parameter.AddBlankKeyFrameToTimeline(GetIndexForFrameNavigation(currentIndex, frameCount, navigation));
            WorkspaceHistoryViewModel.PushUndoRecord(UndoStateTitle);
        }
    }
}
