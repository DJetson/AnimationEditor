using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Interfaces;

namespace AnimationEditorCore.Commands
{
    public class CreateNewWorkspaceCommand : RequeryBase
    {
        public override string DisplayName => "Create New Workspace";

        public override bool CanExecute(object parameter)
        {
            if (!(parameter is IHasWorkspaceCollection Parameter))
                return false;

            if (Parameter?.SelectedWorkspace == null)
                return true;

            if (Parameter.SelectedWorkspace.TimelineViewModel.AnimationPlaybackViewModel.IsPlaybackActive)
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            var Parameter = parameter as IHasWorkspaceCollection;

            Parameter.CreateNewWorkspace();
        }
    }
}
