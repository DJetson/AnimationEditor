﻿using AnimationEditorCore.Commands.BaseClasses;
using AnimationEditorCore.Properties;
using AnimationEditorCore.ViewModels;
using AnimationEditorCore.ViewModels.StateObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Ink;

namespace AnimationEditorCore.Commands.Timeline.FrameContent
{
    public class MoveSelectedContentsToPreviousFrameCommand : RequeryBase
    {
        public override string Description => Resources.MoveSelectedContentsToPreviousFrameDescription;
        public override string ToolTip => Resources.MoveSelectedContentsToPreviousFrameToolTip;
        public override string UndoStateTitle => Resources.MoveSelectedContentsToPreviousFrameUndoStateTitle;
        public override bool CanExecute(object parameter)
        {
            if (!(parameter is TimelineViewModel Parameter))
                return false;

            //if (!(Parameter.IsFrameIndexValid(Parameter.SelectedFrameIndex - 1)))
            //    return false;

            if (Parameter.Layers.ActiveLayer.Frames[Parameter.SelectedFrameIndex].SelectedStrokes.Count == 0)
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            var Parameter = parameter as TimelineViewModel;

            var frame = Parameter.GetActiveFrameAtIndex(Parameter.SelectedFrameIndex);
            FrameViewModel copyToFrame = null;
            if (!(Parameter.IsFrameIndexValid(Parameter.SelectedFrameIndex - 1)))
            {
                Parameter.AddBlankFrameToTimeline(Parameter.SelectedFrameIndex, true);
                copyToFrame = Parameter.Layers.ActiveLayer.Frames[Parameter.SelectedFrameIndex];
            }
            else
            {
                copyToFrame = Parameter.GetActiveFrameAtIndex(Parameter.SelectedFrameIndex - 1);
                Parameter.SelectedFrameIndex = Parameter.SelectedFrameIndex - 1;
            }
            StrokeCollection copiedStrokes = new StrokeCollection(frame.SelectedStrokes.Select(e => e.Clone()));

            frame.RemoveStrokes(frame.SelectedStrokes, false);

            copyToFrame.StrokeCollection.Add(copiedStrokes);
            //Reselect the copied Strokes
            copyToFrame.SelectedStrokes.Add(copiedStrokes);

            WorkspaceHistoryViewModel.PushUndoRecord(UndoStateTitle);
        }
    }
}
