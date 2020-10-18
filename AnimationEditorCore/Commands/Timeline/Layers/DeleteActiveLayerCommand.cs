using AnimationEditorCore.Commands.BaseClasses;
using AnimationEditorCore.Properties;
using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationEditorCore.Commands.Timeline.Layers
{
    public class DeleteActiveLayerCommand : TimelineCommandBase
    {
        public override string Description => Resources.DeleteActiveLayerDescription;
        public override string ToolTip => Resources.DeleteActiveLayerToolTip;
        public override string UndoStateTitle => Resources.DeleteActiveLayerUndoStateTitle;
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

            Parameter.Layers.RemoveLayerAtZIndex(Parameter.Layers.ActiveLayerIndex);
            WorkspaceHistoryViewModel.PushUndoRecord(UndoStateTitle);
        }
    }
}
