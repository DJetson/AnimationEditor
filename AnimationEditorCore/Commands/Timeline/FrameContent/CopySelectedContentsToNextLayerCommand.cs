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

            if (!(Parameter.GetActiveFrameAtIndex(Parameter.SelectedFrameIndex) is KeyFrameViewModel keyFrame))
                return false;

            if (keyFrame.SelectedStrokes.Count == 0)
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            var Parameter = parameter as TimelineViewModel;

            //var targetLayer = Parameter.GetLayerAtIndex(Parameter.Layers.ActiveLayerIndex + 1);
            var sourceFrame = Parameter.GetActiveFrameAtIndex(Parameter.SelectedFrameIndex) as KeyFrameViewModel;

            StrokeCollection copiedStrokes = new StrokeCollection(sourceFrame.SelectedStrokes.Select(e => e.Clone()));

            if (!(Parameter.Layers.IsLayerIndexValid(Parameter.Layers.ActiveLayerIndex + 1)))
            {
                //Create new layer to move selected contents to
                Parameter.Layers.AddBlankLayerAtIndex(Parameter.Layers.ActiveLayerIndex + 1);
            }
            else
            {
                Parameter.Layers.ActiveLayer = Parameter.Layers[Parameter.Layers.ActiveLayerIndex + 1];
            }
            //Parameter.Layers.ActiveLayer = Parameter.Layers[Parameter.Layers.ActiveLayerIndex + 1];
            var copyToFrame = Parameter.Layers.ActiveLayer.ConvertToKeyFrame(Parameter.SelectedFrameIndex);

            copyToFrame.StrokeCollection.Add(copiedStrokes);
            copyToFrame.SelectedStrokes.Add(copiedStrokes);

            WorkspaceHistoryViewModel.PushUndoRecord(UndoStateTitle);
        }
    }
}
