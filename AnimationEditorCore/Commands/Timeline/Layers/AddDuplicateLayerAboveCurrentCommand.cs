using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Commands.BaseClasses;
using AnimationEditorCore.Properties;
using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationEditorCore.Commands.Timeline.Layers
{
    public class AddDuplicateLayerAboveCurrentCommand : RequeryBase
    {
        public override string Description => Resources.AddDuplicateLayerAboveCurrentDescription;
        public override string ToolTip => Resources.AddDuplicateLayerAboveCurrentToolTip;
        public override string UndoStateTitle => Resources.AddDuplicateLayerAboveCurrentUndoStateTitle;
        public override bool CanExecute(object parameter)
        {
            if (!(parameter is TimelineViewModel Parameter))
                return false;

            if (Parameter.AnimationPlaybackViewModel.IsPlaybackActive)
                return false;

            if (Parameter.Layers.ActiveLayer == null)
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            var Parameter = parameter as TimelineViewModel;

            Parameter.DuplicateActiveLayer(LayerNavigation.Above);
            WorkspaceHistoryViewModel.PushUndoRecord(UndoStateTitle, false);
        }
    }
}
