using AnimationEditorCore.Commands.BaseClasses;
using AnimationEditorCore.Properties;
using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationEditorCore.Commands.Timeline.Layers
{
    class ReorderLayerDownCommand : RequeryBase
    {
        public override string Description => Resources.ReorderLayerDownDescription;
        public override string ToolTip => Resources.ReorderLayerDownToolTip;
        public override string UndoStateTitle => Resources.ReorderLayerDownUndoStateTitle; 
        public override bool CanExecute(object parameter)
        {
            if (!(parameter is LayerViewModel Parameter))
                return false;

            if (Parameter.TimelineViewModel == null)
                return false;

            if (Parameter.ZIndex == Parameter.TimelineViewModel.Layers.BottomZIndex)
                return false;

            if (Parameter.TimelineViewModel.Layers.GetLayerBelow(Parameter) == null)
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            var Parameter = parameter as LayerViewModel;
            var timeline = Parameter.TimelineViewModel;

            var swapWith = timeline.Layers.GetLayerBelow(Parameter);

            timeline.Layers.SwapLayerZIndex(Parameter, swapWith);
            WorkspaceHistoryViewModel.PushUndoRecord(UndoStateTitle);
        }
    }
}
