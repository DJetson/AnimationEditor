using AnimationEditorCore.Commands.BaseClasses;
using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationEditorCore.Commands.Timeline.Layers
{
    class ReorderLayerDownCommand : RequeryBase
    {
        public override bool CanExecute(object parameter)
        {
            if (!(parameter is LayerViewModel Parameter))
                return false;

            if (Parameter.TimelineViewModel == null)
                return false;

            if (Parameter.ZIndex == Parameter.TimelineViewModel.BottomZIndex)
                return false;

            if (Parameter.TimelineViewModel.GetLayerBelow(Parameter) == null)
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            var Parameter = parameter as LayerViewModel;
            var timeline = Parameter.TimelineViewModel;

            var swapWith = timeline.GetLayerBelow(Parameter);

            timeline.SwapLayerZIndex(Parameter, swapWith);
        }
    }
}
