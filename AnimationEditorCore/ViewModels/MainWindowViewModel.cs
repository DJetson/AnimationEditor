using AnimationEditorCore.Properties;

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
        }
    }
}
