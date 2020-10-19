﻿using AnimationEditorCore.Commands.BaseClasses;
using AnimationEditorCore.Properties;
using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Ink;

namespace AnimationEditorCore.Commands.Timeline.FrameContent
{
    public class MoveSelectedContentsToPreviousLayerCommand : RequeryBase
    {
        public override string Description => Resources.MoveSelectedContentsToPreviousLayerDescription;
        public override string ToolTip => Resources.MoveSelectedContentsToPreviousLayerToolTip;
        public override string UndoStateTitle => Resources.MoveSelectedContentsToPreviousLayerUndoStateTitle; 
        public override bool CanExecute(object parameter)
        {
            if (!(parameter is TimelineViewModel Parameter))
                return false;

            if (!(Parameter.Layers.ActiveLayer.Frames[Parameter.SelectedFrameIndex] is KeyFrameViewModel keyFrame))
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

            sourceFrame.RemoveStrokes(sourceFrame.SelectedStrokes, false);

            if (!(Parameter.Layers.IsLayerIndexValid(Parameter.Layers.ActiveLayerIndex - 1)))
            {
                //Create new layer to move selected contents to
                Parameter.Layers.AddBlankLayerAtIndex(Parameter.Layers.ActiveLayerIndex);
            }
            else
            {
                Parameter.Layers.ActiveLayer = Parameter.Layers[Parameter.Layers.ActiveLayerIndex - 1];
            }
            var copyToFrame = Parameter.Layers.ActiveLayer.ConvertToKeyFrame(Parameter.SelectedFrameIndex);

            copyToFrame.StrokeCollection.Add(copiedStrokes);
            //Reselect the copied Strokes
            copyToFrame.SelectedStrokes.Add(copiedStrokes);

            WorkspaceHistoryViewModel.PushUndoRecord(UndoStateTitle);
        }
    }
}
