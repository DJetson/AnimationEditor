using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Ink;
using System.Linq;
using AnimationEditorCore.Properties;
using AnimationEditorCore.Commands.BaseClasses;

namespace AnimationEditorCore.Commands.Timeline.FrameContent
{
    public class CopySelectedContentsToNextLayerCommand : RequeryBase
    {
        public override string ToolTip => Resources.CopySelectedContentsToNextLayerUndoStateTitle;
        public override string Description => Resources.CopySelectedContentsToNextLayerDescription;
        public override string UndoStateTitle => Resources.CopySelectedContentsToNextLayerUndoStateTitle;
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
            var frame = Parameter.GetActiveFrameAtIndex(Parameter.SelectedFrameIndex);

            StrokeCollection copiedStrokes = new StrokeCollection(frame.SelectedStrokes.Select(e => e.Clone()));

            if (!(Parameter.IsLayerIndexValid(Parameter.ActiveLayerIndex + 1)))
            {
                //Create new layer to move selected contents to
                Parameter.AddBlankLayerAtIndex(Parameter.ActiveLayerIndex + 1);
            }
            else
            {
                Parameter.ActiveLayer = Parameter.Layers[Parameter.ActiveLayerIndex + 1];
            }
            //Parameter.ActiveLayer = Parameter.Layers[Parameter.ActiveLayerIndex + 1];
            var copyToFrame = Parameter.GetActiveFrameAtIndex(Parameter.SelectedFrameIndex);
            copyToFrame.StrokeCollection.Add(copiedStrokes);
            copyToFrame.SelectedStrokes.Add(copiedStrokes);

            Parameter.PushUndoRecord(Parameter.CreateUndoState(UndoStateTitle));
        }
    }
}
