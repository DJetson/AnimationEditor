using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;

namespace AnimationEditor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private ObservableCollection<WorkspaceViewModel> _Workspaces = new ObservableCollection<WorkspaceViewModel>();
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

        public MainWindowViewModel()
        {
            Workspaces.Add(new WorkspaceViewModel() { DisplayName = "Untitled.anm", UnsavedChanged = true });
            Workspaces.Add(new WorkspaceViewModel() { DisplayName = "Untitled2.anm", UnsavedChanged = true });
            SelectedWorkspace = Workspaces.FirstOrDefault();
        }

    }
}
