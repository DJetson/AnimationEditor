using AnimationEditorCore.Commands.BaseClasses;
using AnimationEditorCore.Properties;
using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationEditorCore.Commands
{
    class NavigateToLayerBelowCurrentCommand : RequeryBase
    {
        public override string Description => Resources.NavigateToLayerBelowDescription;
        public override string ToolTip => Resources.NavigateToLayerBelowToolTip;
        public override bool CanExecute(object parameter)
        {
            if (!(parameter is TimelineViewModel Parameter))
                return false;

            if (!(Parameter.IsLayerIndexValid(Parameter.ActiveLayerIndex - 1)))
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            var Parameter = parameter as TimelineViewModel;
            Parameter.ActivateLayerAtIndex(Parameter.ActiveLayerIndex - 1);
        }
    }
}
