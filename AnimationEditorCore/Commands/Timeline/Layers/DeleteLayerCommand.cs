using AnimationEditorCore.Commands.BaseClasses;
using AnimationEditorCore.Properties;
using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationEditorCore.Commands.Timeline.Layers
{
    public class DeleteLayerCommand : RequeryBase
    {
        public override string Description => Resources.DeleteLayerDescription;
        public override string ToolTip => Resources.DeleteLayerToolTip;
        public override string UndoStateTitle => Resources.DeleteLayerUndoStateTitle;
        public override bool CanExecute(object parameter)
        {
            if (!(parameter is LayerViewModel Parameter))
                return false;

            if (Parameter.TimelineViewModel.AnimationPlaybackViewModel.IsPlaybackActive)
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            var Parameter = parameter as LayerViewModel;

            Parameter.TimelineViewModel.Layers.RemoveLayerAtZIndex(Parameter.ZIndex);
            WorkspaceHistoryViewModel.PushUndoRecord(UndoStateTitle);
        }
    }
}
