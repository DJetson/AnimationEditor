using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Commands.BaseClasses;
using AnimationEditorCore.Properties;
using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationEditorCore.Commands.Timeline.Layers
{
    public class AddDuplicateLayerBelowCurrentCommand : RequeryBase
    {
        public override string Description => Resources.AddDuplicateLayerBelowCurrentDescription;
        public override string ToolTip => Resources.AddDuplicateLayerBelowCurrentToolTip;
        public override string UndoStateTitle => Resources.AddDuplicateLayerBelowCurrentUndoStateTitle;
        public override bool CanExecute(object parameter)
        {
            if (!(parameter is TimelineViewModel Parameter))
                return false;

            if (Parameter.AnimationPlaybackViewModel.IsPlaybackActive)
                return false;

            if (Parameter.ActiveLayer == null)
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            var Parameter = parameter as TimelineViewModel;

            Parameter.DuplicateActiveLayer(LayerNavigation.Below);
        }
    }
}
