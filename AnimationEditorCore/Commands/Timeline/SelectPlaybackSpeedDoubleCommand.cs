using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Properties;
using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationEditorCore.Commands.Timeline
{
    public class SelectPlaybackSpeedDoubleCommand : RequeryBase
    {
        public override string Description => Resources.SelectPlaybackSpeedDoubleDescription;
        public override string ToolTip => Resources.SelectPlaybackSpeedDoubleToolTip;
        public override bool CanExecute(object parameter)
        {
            if (!(parameter is TimelineViewModel Parameter))
                return false;

            if (Parameter.AnimationPlaybackViewModel == null)
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            var Parameter = parameter as TimelineViewModel;

            Parameter.AnimationPlaybackViewModel.SetPlaybackSpeed(PlaybackSpeed.Double);
        }
    }
}
