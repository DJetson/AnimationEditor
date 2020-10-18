using AnimationEditorCore.Commands.BaseClasses;
using AnimationEditorCore.Properties;
using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationEditorCore.Commands.Timeline.Frames
{
    public class DeleteCurrentFrameCommand : RequeryBase
    {
        public override string Description => Resources.DeleteCurrentFrameDescription;
        public override string ToolTip => Resources.DeleteCurrentFrameToolTip;
        public override string UndoStateTitle => Resources.DeleteCurrentFrameUndoStateTitle;
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

            Parameter.DeleteCurrentFrame();
            WorkspaceHistoryViewModel.PushUndoRecord(UndoStateTitle);
        }
    }
}
