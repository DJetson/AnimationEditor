using AnimationEditorCore.Commands.BaseClasses;
using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationEditorCore.Commands.Timeline.Frames
{
    public class InsertBlankKeyFrameCommand : RequeryBase
    {
        public override bool CanExecute(object parameter)
        {
            if (!(parameter is TimelineViewModel Parameter))
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            var Parameter = parameter as TimelineViewModel;

            Parameter.InsertBlankKeyFrame(Parameter.Layers.ActiveLayer, Parameter.SelectedFrameIndex);
            WorkspaceHistoryViewModel.PushUndoRecord("Insert Blank Key Frame");
        }
    }
}
