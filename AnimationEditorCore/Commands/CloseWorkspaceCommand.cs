using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Interfaces;
using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationEditorCore.Commands
{
    public class CloseWorkspaceCommand : RequeryBase
    {
        public override string DisplayName => "Close Workspace";

        public override bool CanExecute(object parameter)
        {
            if (!(parameter is WorkspaceViewModel Parameter))
                return false;

            if (Parameter.AnimationTimelineViewModel.AnimationPlaybackViewModel.IsPlaybackActive)
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            var Parameter = parameter as WorkspaceViewModel;
            Parameter.Close();
        }
    }
}
