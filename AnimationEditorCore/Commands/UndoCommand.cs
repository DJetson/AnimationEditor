using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Properties;
using AnimationEditorCore.ViewModels;

namespace AnimationEditorCore.Commands
{
    public class UndoCommand : RequeryBase
    {
        public override string Description => Resources.UndoDescription;
        public override string ToolTip => Resources.UndoToolTip;
        public override bool CanExecute(object parameter)
        {
            if (!(parameter is WorkspaceViewModel Parameter))
                return false;

            if (Parameter?.WorkspaceHistoryViewModel?.UndoStack.Count <= 1)
                return false;

            if (Parameter?.TimelineViewModel?.AnimationPlaybackViewModel == null)
                return false;

            if (Parameter.TimelineViewModel.AnimationPlaybackViewModel.IsPlaybackActive)
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            (parameter as WorkspaceViewModel).WorkspaceHistoryViewModel.Undo();
        }
    }
}
