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
            if (!(parameter is MainWindowViewModel Parameter))
                return false;

            if (Parameter.WorkspaceManager?.PeekRedo() == null)
                return false;

            if (Parameter.WorkspaceManager?.SelectedWorkspace
                ?.AnimationTimelineViewModel?.AnimationPlaybackViewModel.CurrentState != PlaybackStates.Stop)
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            (parameter as MainWindowViewModel).WorkspaceManager.Redo();
        }
    }
}