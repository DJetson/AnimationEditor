using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Properties;
using AnimationEditorCore.ViewModels;

namespace AnimationEditorCore.Commands
{
    public class SaveWorkspaceCommand : RequeryBase
    {
        public override string Description => Resources.SaveWorkspaceDescription;
        public override string ToolTip => Resources.SaveWorkspaceToolTip;
        public override string DisplayName => "Save Workspace";

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

            Parameter.UpdateModelAndSaveWorkspace();
        }
    }
}
