using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationEditorCore.Commands
{
    public class NavigateToLastFrameCommand : RequeryBase
    {
        public override bool CanExecute(object parameter)
        {
            if (!(parameter is TimelineViewModel Parameter))
            {
                return false;
            }

            if (Parameter.AnimationPlaybackViewModel.IsPlaybackActive)
            {
                return false;
            }

            return true;
        }

        public override void Execute(object parameter)
        {
            var Parameter = parameter as TimelineViewModel;

            Parameter.SelectedFrameIndex = Parameter.FrameCount - 1;
        }
    }
}
