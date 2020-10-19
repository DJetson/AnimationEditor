using AnimationEditorCore.Commands.BaseClasses;
using AnimationEditorCore.Properties;
using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Ink;

namespace AnimationEditorCore.Commands.Timeline.FrameContent
{
    public class CopySelectedContentsToPreviousFrameCommand : RequeryBase
    {
        public override string ToolTip => Resources.CopySelectedContentsToPreviousFrameToolTip;
        public override string Description => Resources.CopySelectedContentsToPreviousFrameDescription;
        public override string UndoStateTitle => Resources.CopySelectedContentsToPreviousFrameUndoStateTitle;

        public override bool CanExecute(object parameter)
        {
            if (!(parameter is TimelineViewModel Parameter))
                return false;

            //if (!(Parameter.IsFrameIndexValid(Parameter.SelectedFrameIndex - 1)))
            //    return false;
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
            KeyFrameViewModel copyToFrame = null;

            if (!(Parameter.IsFrameIndexValid(Parameter.SelectedFrameIndex - 1)))
            {
                Parameter.AddBlankKeyFrameToTimeline(Parameter.SelectedFrameIndex, true);
                copyToFrame = Parameter.Layers.ActiveLayer.Frames[Parameter.SelectedFrameIndex] as KeyFrameViewModel;
            }
            else
            {
                copyToFrame = Parameter.Layers.ActiveLayer.ConvertToKeyFrame(Parameter.SelectedFrameIndex - 1);
                Parameter.SelectedFrameIndex = Parameter.SelectedFrameIndex - 1;
            }

            StrokeCollection copiedStrokes = new StrokeCollection(sourceFrame.SelectedStrokes.Select(e => e.Clone()));

            copyToFrame.StrokeCollection.Add(copiedStrokes);
            copyToFrame.SelectedStrokes.Add(copiedStrokes);

            WorkspaceHistoryViewModel.PushUndoRecord(UndoStateTitle);
        }
    }
}
