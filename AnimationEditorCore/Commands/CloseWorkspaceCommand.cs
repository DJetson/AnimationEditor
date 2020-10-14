using AnimationEditorCore.Commands.BaseClasses;
using AnimationEditorCore.Properties;
using AnimationEditorCore.ViewModels;

namespace AnimationEditorCore.Commands
{
    public class CloseWorkspaceCommand : RequeryBase
    {
        public override string Description => Resources.CloseWorkspaceDescription;
        public override string ToolTip => Resources.CloseWorkspaceToolTip;
        public override string DisplayName => "Close Workspace";
        public override bool CanExecute(object parameter)
        {
            if (!(parameter is WorkspaceViewModel Parameter))
                return false;

            if (Parameter.TimelineViewModel.AnimationPlaybackViewModel.IsPlaybackActive)
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
