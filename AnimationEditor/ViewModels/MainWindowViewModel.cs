using AnimationEditor.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Ink;

namespace AnimationEditor.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private WorkspaceManagerViewModel _WorkspaceManager = new WorkspaceManagerViewModel();
        public WorkspaceManagerViewModel WorkspaceManager
        {
            get => _WorkspaceManager;
            set { _WorkspaceManager = value; NotifyPropertyChanged(); }
        }

        //private ObservableCollection<WorkspaceViewModel> _Workspaces = new ObservableCollection<WorkspaceViewModel>();
        //public ObservableCollection<WorkspaceViewModel> Workspaces
        //{
        //    get => _Workspaces;
        //    set { _Workspaces = value; NotifyPropertyChanged(); }
        //}

        //private WorkspaceViewModel _SelectedWorkspace;
        //public WorkspaceViewModel SelectedWorkspace
        //{
        //    get => _SelectedWorkspace;
        //    set { _SelectedWorkspace = value; NotifyPropertyChanged(); }
        //}

        public MainWindowViewModel()
        {
        }

        //public void CreateNewWorkspace()
        //{
        //    Workspaces.Add(new WorkspaceViewModel() { DisplayName = "Untitled", HasUnsavedChanges = true });
        //}

        //public void AddWorkspace(WorkspaceViewModel workspace)
        //{
        //    if (!Workspaces.Contains(workspace))
        //        Workspaces.Add(workspace);
        //}

        //public void RemoveWorkspace(WorkspaceViewModel workspace)
        //{
        //    if(workspace.HasUnsavedChanges)
        //    {
        //        var mbResult = MessageBox.Show($"{workspace.DisplayName} has unsaved changes. Do you want to save before closing?", "Save changes?", MessageBoxButton.YesNoCancel);
        //        if(mbResult == MessageBoxResult.Yes)
        //        {
        //            workspace.UpdateModelAndSaveWorkspace();
        //            //"Save" or "Save As" workflow
        //        }
        //        else if(mbResult == MessageBoxResult.No)
        //        {
        //            if (Workspaces.Contains(workspace))
        //                Workspaces.Remove(workspace);
        //        }
        //        else
        //        {
        //            return;
        //        }
        //    }
        //}
    }
}
