using AnimationEditor.BaseClasses;
using AnimationEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationEditor.Commands
{
    public class NavigateToNextFrameCommand : RequeryBase
    {
        public override bool CanExecute(object parameter)
        {
            if (!(parameter is AnimationTimelineViewModel Parameter))
            {
                return false;
            }

            if (Parameter.AnimationPlaybackViewModel.IsPlaybackActive)
                return false;

            if (Parameter?.Frames == null)
            {
                return false;
            }

            if(Parameter?.SelectedFrame == null)
            {
                return false;
            }

            if(Parameter.Frames.LastOrDefault() == Parameter.SelectedFrame)
            {
                return false;
            }

            return true;
        }

        public override void Execute(object parameter)
        {
            var Parameter = parameter as AnimationTimelineViewModel;

            Parameter.NavigateToFrame(FrameNavigation.Next);
        }
    }
}
