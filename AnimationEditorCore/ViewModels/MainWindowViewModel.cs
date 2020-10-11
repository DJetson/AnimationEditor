using AnimationEditorCore.Properties;
using AnimationEditorCore.Views;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Documents;

namespace AnimationEditorCore.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public override string DisplayName => Resources.MainWindowViewModelDisplayName;

        private WorkspaceManagerViewModel _WorkspaceManager;
        public WorkspaceManagerViewModel WorkspaceManager
        {
            get => _WorkspaceManager;
            set { _WorkspaceManager = value; NotifyPropertyChanged(); }
        }

        private EditorToolsViewModel _EditorToolsManager = EditorToolsViewModel.Instance;
        public EditorToolsViewModel EditorToolsManager
        {
            get => _EditorToolsManager;
            set { _EditorToolsManager = value; NotifyPropertyChanged(); }
        }

        public MainWindowViewModel()
        {
            _WorkspaceManager = new WorkspaceManagerViewModel();

            var recoveredFilePaths = GetRecoveredFilePaths();
            if(recoveredFilePaths.Count > 0)
            {
                //Display a dialog informing user of application closed unexpectedly the last time it was used
                //The following files had unsaved changes which can be recovered from temporary backups:
                //Checklist of recovered files with Filepath | Last Modified DateTime
                //RadioButtons to select how to address recovered files
                //Recover Selected (Recover Selected, Delete Unselected Backups)
                //Later (Don't Recover, Keep All Backups)
                //Delete Backups (Don't Recover, Delete All Backups)
                //Ok
                var recoveryWindow = new WorkspaceRecoveryWindow() 
                { DataContext = new WorkspaceRecoveryViewModel(recoveredFilePaths, WorkspaceManager) };

                recoveryWindow.ShowDialog();
            }
        }

        public List<string> GetRecoveredFilePaths()
        {
            var paths = new List<string>();
            var recoveryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AnimationEditor", "Recovered");
            paths.AddRange(Directory.GetFiles(recoveryPath).Where(e => e.EndsWith(".atmp")));
            return paths;
        }
    }
}
