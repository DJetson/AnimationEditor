using AnimationEditor.BaseClasses;
using AnimationEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationEditor.Commands
{
    public class UndoCommand : RequeryBase
    {
        //public override string DisplayName => "Undo";

        public override bool CanExecute(object parameter)
        {
            if (!(parameter is WorkspaceViewModel Parameter))
                return false;

            if (Parameter?.WorkspaceHistoryViewModel?.PeekUndo() == null)
                return false;

            if (Parameter?.AnimationTimelineViewModel?.AnimationPlaybackViewModel.CurrentState != PlaybackStates.Stop)
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            (parameter as WorkspaceViewModel).WorkspaceHistoryViewModel.Undo();
        }
    }
}
