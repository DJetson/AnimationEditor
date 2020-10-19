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
    public class MoveSelectedContentsToNextFrameCommand : RequeryBase
    {
        public override string Description => Resources.MoveSelectedContentsToNextFrameDescription;
        public override string ToolTip => Resources.MoveSelectedContentsToNextFrameToolTip;
        public override string UndoStateTitle => Resources.MoveSelectedContentsToNextFrameUndoStateTitle;

        public override bool CanExecute(object parameter)
        {
            if (!(parameter is TimelineViewModel Parameter))
                return false;

            //if (!(Parameter.IsFrameIndexValid(Parameter.SelectedFrameIndex + 1)))
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

            if (!(Parameter.IsFrameIndexValid(Parameter.SelectedFrameIndex + 1)))
            {
                Parameter.AddBlankKeyFrameToTimeline(Parameter.SelectedFrameIndex + 1, false);
            }

            var copyToFrame = Parameter.Layers.ActiveLayer.ConvertToKeyFrame(Parameter.SelectedFrameIndex + 1);

            StrokeCollection copiedStrokes = new StrokeCollection(sourceFrame.SelectedStrokes.Select(e => e.Clone()));

            sourceFrame.RemoveStrokes(sourceFrame.SelectedStrokes, false);

            copyToFrame.StrokeCollection.Add(copiedStrokes);
            Parameter.SelectedFrameIndex = Parameter.SelectedFrameIndex + 1;

            //Reselect the copied Strokes
            copyToFrame.SelectedStrokes.Add(copiedStrokes);

            WorkspaceHistoryViewModel.PushUndoRecord(UndoStateTitle);
        }
    }
}
