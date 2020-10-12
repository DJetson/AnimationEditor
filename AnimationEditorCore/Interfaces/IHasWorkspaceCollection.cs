using AnimationEditorCore.ViewModels;
using System.Collections.ObjectModel;

namespace AnimationEditorCore.Interfaces
{
    public interface IHasWorkspaceCollection
    {
        ObservableCollection<WorkspaceViewModel> Workspaces { get; set; }
        WorkspaceViewModel SelectedWorkspace { get; set; }
        WorkspaceViewModel CreateNewWorkspace(bool promptForAnimationProperties = true);
        void AddWorkspace(WorkspaceViewModel workspace);
        void RemoveWorkspace(WorkspaceViewModel workspace);
    }
}
