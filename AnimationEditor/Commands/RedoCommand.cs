using AnimationEditor.BaseClasses;
using AnimationEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationEditor.Commands
{
    public class RedoCommand : RequeryBase
    {
        //public override string DisplayName => "Redo";

        public override bool CanExecute(object parameter)
        {
            if (!(parameter is WorkspaceViewModel Parameter))
                return false;

            if (Parameter?.WorkspaceHistoryViewModel?.RedoStack.Count < 1)
                return false;

            if (Parameter?.AnimationTimelineViewModel?.AnimationPlaybackViewModel == null)
                return false;

            if (Parameter.AnimationTimelineViewModel.AnimationPlaybackViewModel.IsPlaybackActive)
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            (parameter as WorkspaceViewModel).WorkspaceHistoryViewModel.Redo();
        }
    }
}