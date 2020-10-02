using AnimationEditorCore.Interfaces;
using AnimationEditorCore.Utilities;
using System.Collections.ObjectModel;
using System.Linq;

namespace AnimationEditorCore.ViewModels
{
    public class WorkspaceManagerViewModel : ViewModelBase, IHasWorkspaceCollection/*, IMementoCaretaker*/
    {
        private ObservableCollection<WorkspaceViewModel> _Workspaces;
        public ObservableCollection<WorkspaceViewModel> Workspaces
        {
            get => _Workspaces;
            set { _Workspaces = value; NotifyPropertyChanged(); }
        }

        private WorkspaceViewModel _SelectedWorkspace;
        public WorkspaceViewModel SelectedWorkspace
        {
            get => _SelectedWorkspace;
            set { _SelectedWorkspace = value; NotifyPropertyChanged(); }
        }

        public WorkspaceManagerViewModel()
        {
            InitializeDependentViewModels();
            SelectedWorkspace = Workspaces.LastOrDefault();
        }

        private void InitializeDependentViewModels()
        {
            Workspaces = new ObservableCollection<WorkspaceViewModel>();
            var newWorkspace = new WorkspaceViewModel()
            {
                DisplayName = FileUtilities.GetUniquePlaceholderName(this),
                HasUnsavedChanges = true,
                Host = this
            };

            Workspaces.Add(newWorkspace);
        }

        public WorkspaceViewModel CreateNewWorkspace()
        {
            var newWorkspace = new WorkspaceViewModel() { DisplayName = FileUtilities.GetUniquePlaceholderName(this), HasUnsavedChanges = true, Host = this };
            Workspaces.Add(newWorkspace);

            SelectedWorkspace = newWorkspace;

            return newWorkspace;
        }

        public void AddWorkspace(WorkspaceViewModel item)
        {
            if (!Workspaces.Contains(item))
            {
                item.Host = this;
                Workspaces.Add(item);
                SelectedWorkspace = item;
            }
        }

        public void RemoveWorkspace(WorkspaceViewModel workspace)
        {
            if (Workspaces.Contains(workspace))
                Workspaces.Remove(workspace);
        }
    }
}
