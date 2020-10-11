using AnimationEditorCore.Interfaces;
using AnimationEditorCore.Models;
using AnimationEditorCore.Utilities;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows.Documents;

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

        public void DeleteRecoveryFiles(List<string> filepaths)
        {
            foreach(var filepath in filepaths)
            {
                File.Delete(filepath);
            }
        }

        public void OpenRecoveredWorkspaces(List<string> filepaths)
        {
            System.Text.Json.JsonSerializerOptions JsonSerializerOptions = new System.Text.Json.JsonSerializerOptions();
            JsonSerializerOptions.Converters.Add(new StrokeCollectionConverter());

            foreach (var filepath in filepaths)
            {
                var f = WorkspaceFileModel.OpenWorkspaceFile(filepath, JsonSerializerOptions);
                var w = new WorkspaceViewModel(f) { IsRecoveredFile = true, HasUnsavedChanges = true };
                AddWorkspace(w);
            }
        }
    }
}
