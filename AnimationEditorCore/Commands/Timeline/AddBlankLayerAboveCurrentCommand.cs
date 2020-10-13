using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Properties;
using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationEditorCore.Commands.Timeline
{
    public class AddBlankLayerAboveCurrentCommand : RequeryBase
    {
        public override string Description => Resources.AddBlankLayerAboveCurrentDescription;
        public override string ToolTip => Resources.AddBlankLayerAboveCurrentToolTip;
        public override string UndoStateTitle => Resources.AddBlankLayerAboveCurrentUndoStateTitle;
        public override bool CanExecute(object parameter)
        {
            if (!(parameter is TimelineViewModel Parameter))
                return false;

            if (Parameter.AnimationPlaybackViewModel.IsPlaybackActive)
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            var Parameter = parameter as TimelineViewModel;

            Parameter.AddBlankLayer(LayerNavigation.Above);
        }
    }
}
