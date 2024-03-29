﻿using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Commands.BaseClasses;
using AnimationEditorCore.Properties;
using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationEditorCore.Commands.Timeline.Playback
{
    public class SelectPlaybackSpeedHalfCommand : RequeryBase
    {
        public override string Description => Resources.SelectPlaybackSpeedHalfDescription;
        public override string ToolTip => Resources.SelectPlaybackSpeedHalfToolTip;
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

            Parameter.AnimationPlaybackViewModel.SetPlaybackSpeed(PlaybackSpeed.Half);
        }
    }
}
