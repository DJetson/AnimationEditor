﻿using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace AnimationEditorCore.Commands.Timeline
{
    public class StopPlaybackCommand : RequeryBase
    {
        public override bool CanExecute(object parameter)
        {
            if (!(parameter is TimelineViewModel Parameter))
                return false;

            if (Parameter.AnimationPlaybackViewModel.IsPlaybackActive == false)
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            var Parameter = parameter as TimelineViewModel;

            Parameter.AnimationPlaybackViewModel.StopPlayback();
            Parameter.SelectedFrameIndex = Parameter.AnimationPlaybackViewModel.CurrentFrameIndex;
        }
    }
}