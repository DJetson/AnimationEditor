using AnimationEditorCore.BaseClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

namespace AnimationEditorCore.ViewModels
{
    public class WorkspaceFileListItemViewModel : ViewModelBase
    {
        private bool _IsSelected;
        public bool IsSelected
        {
            get { return _IsSelected; }
            set { _IsSelected = value; NotifyPropertyChanged(); }
        }

        private string _Filepath;
        public string Filepath
        {
            get { return _Filepath; }
            set { _Filepath = value; NotifyPropertyChanged(); }
        }

        private DateTime _LastModifiedDate;
        public DateTime LastModifiedDate
        {
            get { return _LastModifiedDate; }
            set { _LastModifiedDate = value; NotifyPropertyChanged(); }
        }

        private DateTime _CreatedDate;
        public DateTime CreatedDate
        {
            get { return _CreatedDate; }
            set { _CreatedDate = value; NotifyPropertyChanged(); }
        }
        public WorkspaceFileListItemViewModel(string path)
        {
            LastModifiedDate = File.GetLastWriteTimeUtc(path).ToLocalTime();
            CreatedDate = File.GetCreationTimeUtc(path).ToLocalTime();
            Filepath = path;
            IsSelected = true;
        }
    }
    public class WorkspaceRecoveryViewModel : ViewModelBase
    {
        public enum RecoveryResponse { RecoverSelected, KeepBackups, DeleteBackups };


        //private bool? _SelectedResponse = true;
        //public bool? SelectedResponse
        //{
        //    get { return _SelectedResponse; }
        //    set { _SelectedResponse = value; NotifyPropertyChanged(); }
        //}

        private bool _IsRecoverSelectedChecked = true;
        public bool IsRecoverSelectedChecked
        {
            get { return _IsRecoverSelectedChecked; }
            set { _IsRecoverSelectedChecked = value; NotifyPropertyChanged(); }
        }

        private bool _IsKeepBackupsChecked = false;
        public bool IsKeepBackupsChecked
        {
            get { return _IsKeepBackupsChecked; }
            set { _IsKeepBackupsChecked = value; NotifyPropertyChanged(); }
        }

        private bool _IsDeleteBackupsChecked = false;
        public bool IsDeleteBackupsChecked
        {
            get { return _IsDeleteBackupsChecked; }
            set { _IsDeleteBackupsChecked = value; NotifyPropertyChanged(); }
        }

        private ObservableCollection<WorkspaceFileListItemViewModel> _WorkspaceFileItems = new ObservableCollection<WorkspaceFileListItemViewModel>();
        public ObservableCollection<WorkspaceFileListItemViewModel> WorkspaceFileItems
        {
            get { return _WorkspaceFileItems; }
            set { _WorkspaceFileItems = value; NotifyPropertyChanged(); }
        }

        private DelegateCommand _CloseWorkspaceRecoveryWindow;
        public DelegateCommand CloseWorkspaceRecoveryWindow
        {
            get { return _CloseWorkspaceRecoveryWindow; }
            set { _CloseWorkspaceRecoveryWindow = value; NotifyPropertyChanged(); }
        }


        private WorkspaceManagerViewModel _WorkspaceManager;

        public WorkspaceRecoveryViewModel(List<string> filepaths, WorkspaceManagerViewModel workspaceManager)
        {
            _WorkspaceManager = workspaceManager;

            var workspaceFileList = filepaths.Select(e => new WorkspaceFileListItemViewModel(e));

            WorkspaceFileItems = new ObservableCollection<WorkspaceFileListItemViewModel>(workspaceFileList);

            CloseWorkspaceRecoveryWindow = new DelegateCommand(CloseWorkspaceRecoveryWindow_CanExecute, CloseWorkspaceRecoveryWindow_Execute);
        }

        private bool CloseWorkspaceRecoveryWindow_CanExecute(object parameter)
        {
            if (!(parameter is Window Parameter))
                return false;

            return true;
        }

        private void CloseWorkspaceRecoveryWindow_Execute(object parameter)
        {
            var Parameter = parameter as Window;

            var selectedFilepaths = WorkspaceFileItems.Where(e => e.IsSelected).Select(f => f.Filepath).ToList();
            var unselectedFilepaths = WorkspaceFileItems.Where(e => !e.IsSelected).Select(f => f.Filepath).ToList();
            var allFilepaths = WorkspaceFileItems.Select(e => e.Filepath).ToList();

            if (IsRecoverSelectedChecked)
            {
                _WorkspaceManager.OpenRecoveredWorkspaces(selectedFilepaths);
                _WorkspaceManager.DeleteRecoveryFiles(unselectedFilepaths);
            }
            else if (IsDeleteBackupsChecked)
            {
                _WorkspaceManager.DeleteRecoveryFiles(allFilepaths);
            }
            else
            {
                //Keep backups (do nothing)
            }
            Parameter.Close();
        }
    }
}
