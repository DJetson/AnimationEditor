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

            if (!(Parameter.GetActiveFrameAtIndex(Parameter.SelectedFrameIndex) is KeyFrameViewModel keyFrame))
                return false;

            if (keyFrame.SelectedStrokes.Count == 0)
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            var Parameter = parameter as TimelineViewModel;

            var sourceFrame = Parameter.GetActiveFrameAtIndex(Parameter.SelectedFrameIndex) as KeyFrameViewModel;

            if (!(Parameter.IsFrameIndexValid(Parameter.SelectedFrameIndex + 1)))
            {
                Parameter.AddBlankKeyFrameToTimeline(Parameter.SelectedFrameIndex + 1, false);
            }

            var copyToFrame = Parameter.Layers.ActiveLayer.ConvertToKeyFrame(Parameter.SelectedFrameIndex + 1);

            StrokeCollection copiedStrokes = new StrokeCollection(sourceFrame.SelectedStrokes.Select(e => e.Clone()));

            copyToFrame.StrokeCollection.Add(copiedStrokes);
            Parameter.SelectedFrameIndex = Parameter.SelectedFrameIndex + 1;
            copyToFrame.SelectedStrokes.Add(copiedStrokes);

            WorkspaceHistoryViewModel.PushUndoRecord(UndoStateTitle);
        }
    }
}
