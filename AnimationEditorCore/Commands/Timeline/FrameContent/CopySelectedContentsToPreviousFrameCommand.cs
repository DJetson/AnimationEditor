using AnimationEditorCore.BaseClasses;
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
        public override bool CanExecute(object parameter)
        {
            if (!(parameter is TimelineViewModel Parameter))
                return false;

            //if (!(Parameter.IsFrameIndexValid(Parameter.SelectedFrameIndex - 1)))
            //    return false;

            if (Parameter.ActiveLayer.Frames[Parameter.SelectedFrameIndex].SelectedStrokes.Count == 0)
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
                Parameter.AddBlankFrameToTimeline(Parameter.SelectedFrameIndex, true, false);
                copyToFrame = Parameter.ActiveLayer.Frames[Parameter.SelectedFrameIndex];
            }
            else
            {
                copyToFrame = Parameter.GetActiveFrameAtIndex(Parameter.SelectedFrameIndex - 1);
                Parameter.SelectedFrameIndex = Parameter.SelectedFrameIndex - 1;
            }

            StrokeCollection copiedStrokes = new StrokeCollection(frame.SelectedStrokes.Select(e => e.Clone()));

            copyToFrame.StrokeCollection.Add(copiedStrokes);
            
            Parameter.PushUndoRecord(Parameter.CreateUndoState("Copy Strokes To Previous Frame"));
        }
    }
}
