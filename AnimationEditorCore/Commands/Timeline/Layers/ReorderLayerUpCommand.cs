using AnimationEditorCore.Commands.BaseClasses;
using AnimationEditorCore.Properties;
using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationEditorCore.Commands.Timeline.Layers
{
    public class ReorderLayerUpCommand : RequeryBase
    {
        public override string Description => Resources.ReorderLayerUpDescription;
        public override string ToolTip => Resources.ReorderLayerUpToolTip;
        public override string UndoStateTitle => Resources.ReorderLayerUpUndoStateTitle; 
        public override bool CanExecute(object parameter)
        {
            if (!(parameter is LayerViewModel Parameter))
                return false;

            if (Parameter.TimelineViewModel == null)
                return false;

            if (Parameter.ZIndex == Parameter.TimelineViewModel.TopZIndex)
                return false;

            if (Parameter.TimelineViewModel.GetLayerAbove(Parameter) == null)
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            var Parameter = parameter as LayerViewModel;
            var timeline = Parameter.TimelineViewModel;

            var swapWith = timeline.GetLayerAbove(Parameter);

            timeline.SwapLayerZIndex(Parameter, swapWith);
            timeline.PushUndoRecord(timeline.CreateUndoState(UndoStateTitle));
        }
    }
}
