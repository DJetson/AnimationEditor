using AnimationEditorCore.Commands.BaseClasses;
using AnimationEditorCore.Properties;
using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationEditorCore.Commands.Timeline.Navigation
{
    public class NavigateToLayerAboveCurrentCommand : RequeryBase
    {
        public override string Description => Resources.NavigateToLayerAboveDescription;
        public override string ToolTip => Resources.NavigateToLayerAboveToolTip;
        public override bool CanExecute(object parameter)
        {
            if (!(parameter is TimelineViewModel Parameter))
                return false;

            if (!(Parameter.IsLayerIndexValid(Parameter.Layers.ActiveLayerIndex + 1)))
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            var Parameter = parameter as TimelineViewModel;
            Parameter.ActivateLayerAtIndex(Parameter.Layers.ActiveLayerIndex + 1);
        }
    }
}
