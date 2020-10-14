using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Properties;
using AnimationEditorCore.ViewModels;

namespace AnimationEditorCore.Commands
{
    public class RedoCommand : RequeryBase
    {
        public override string Description => Resources.RedoDescription;
        public override string ToolTip => Resources.RedoToolTip;
        public override bool CanExecute(object parameter)
        {
            if (!(parameter is WorkspaceViewModel Parameter))
                return false;

            if (Parameter?.WorkspaceHistoryViewModel?.RedoStack.Count < 1)
                return false;

            if (Parameter?.TimelineViewModel?.AnimationPlaybackViewModel == null)
                return false;

            if (Parameter.TimelineViewModel.AnimationPlaybackViewModel.IsPlaybackActive)
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            (parameter as WorkspaceViewModel).WorkspaceHistoryViewModel.Redo();
        }
    }
}