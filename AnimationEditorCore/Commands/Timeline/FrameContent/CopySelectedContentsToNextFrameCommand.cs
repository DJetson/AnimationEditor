using AnimationEditorCore.Commands.BaseClasses;
using AnimationEditorCore.Properties;
using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Ink;

namespace AnimationEditorCore.Commands.Timeline.FrameContent
{
    public class CopySelectedContentsToNextFrameCommand : RequeryBase
    {
        public override string ToolTip => Resources.CopySelectedContentsToNextFrameToolTip;
        public override string Description => Resources.CopySelectedContentsToNextFrameDescription;
        public override string UndoStateTitle => Resources.CopySelectedContentsToNextFrameUndoStateTitle;

        public override bool CanExecute(object parameter)
        {
            if (!(parameter is TimelineViewModel Parameter))
                return false;

            if (Parameter.Layers.ActiveLayer.Frames[Parameter.SelectedFrameIndex].SelectedStrokes.Count == 0)
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            var Parameter = parameter as TimelineViewModel;

            var frame = Parameter.GetActiveFrameAtIndex(Parameter.SelectedFrameIndex);

            if (!(Parameter.IsFrameIndexValid(Parameter.SelectedFrameIndex + 1)))
            {
                Parameter.AddBlankFrameToTimeline(Parameter.SelectedFrameIndex + 1, false);
            }

            var copyToFrame = Parameter.GetActiveFrameAtIndex(Parameter.SelectedFrameIndex + 1);

            StrokeCollection copiedStrokes = new StrokeCollection(frame.SelectedStrokes.Select(e => e.Clone()));

            copyToFrame.StrokeCollection.Add(copiedStrokes);
            Parameter.SelectedFrameIndex = Parameter.SelectedFrameIndex + 1;
            copyToFrame.SelectedStrokes.Add(copiedStrokes);

            WorkspaceHistoryViewModel.PushUndoRecord(UndoStateTitle);
        }
    }
}
