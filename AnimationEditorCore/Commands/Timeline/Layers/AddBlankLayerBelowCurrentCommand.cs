using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Commands.BaseClasses;
using AnimationEditorCore.Properties;
using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationEditorCore.Commands.Timeline.Layers
{
    public class AddBlankLayerBelowCurrentCommand : RequeryBase
    {
        public override string Description => Resources.AddBlankLayerBelowCurrentDescription;
        public override string ToolTip => Resources.AddBlankLayerBelowCurrentToolTip;
        public override string UndoStateTitle => Resources.AddBlankLayerBelowCurrentUndoStateTitle;
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

            Parameter.AddBlankLayer(LayerNavigation.Below);
            Parameter.PushUndoRecord(Parameter.CreateUndoState(UndoStateTitle));
        }
    }
}
