using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Ink;
using System.Linq;

namespace AnimationEditorCore.Commands.Timeline.FrameContent
{
    public class CopySelectedContentsToNextLayerCommand : RequeryBase
    {
        public override bool CanExecute(object parameter)
        {
            if (!(parameter is TimelineViewModel Parameter))
                return false;

            if (Parameter.ActiveLayer.Frames[Parameter.SelectedFrameIndex].SelectedStrokes.Count == 0)
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            var Parameter = parameter as TimelineViewModel;

            //var targetLayer = Parameter.GetLayerAtIndex(Parameter.ActiveLayerIndex + 1);

            if (!(Parameter.IsLayerIndexValid(Parameter.ActiveLayerIndex + 1)))
            {
                //Create new layer to move selected contents to
                Parameter.AddBlankLayerAtIndex(Parameter.ActiveLayerIndex + 1);
            }

            var frame = Parameter.GetActiveFrameAtIndex(Parameter.SelectedFrameIndex);

            StrokeCollection copiedStrokes = new StrokeCollection(frame.SelectedStrokes.Select(e => e.Clone()));

            Parameter.ActiveLayer = Parameter.Layers[Parameter.ActiveLayerIndex + 1];
            var copyToFrame = Parameter.GetActiveFrameAtIndex(Parameter.SelectedFrameIndex);

            copyToFrame.StrokeCollection.Add(copiedStrokes);

            Parameter.PushUndoRecord(Parameter.CreateUndoState("Copy To Layer"));
        }
    }
}
