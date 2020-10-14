using AnimationEditorCore.Commands.BaseClasses;
using AnimationEditorCore.Interfaces;
using AnimationEditorCore.Models;
using AnimationEditorCore.Properties;
using AnimationEditorCore.ViewModels;
using Microsoft.Win32;

namespace AnimationEditorCore.Commands.Workspace
{
    public class OpenWorkspaceCommand : RequeryBase
    {
        public override string Description => Resources.OpenWorkspaceDescription;
        public override string ToolTip => Resources.OpenWorkspaceToolTip;
        public override string DisplayName => "Open Workspace";

        public OpenWorkspaceCommand()
        {
            JsonSerializerOptions = new System.Text.Json.JsonSerializerOptions();
            JsonSerializerOptions.Converters.Add(new StrokeCollectionConverter());
        }

        private System.Text.Json.JsonSerializerOptions JsonSerializerOptions { get; }

        public override bool CanExecute(object parameter)
        {
            if (!(parameter is IHasWorkspaceCollection Parameter))
                return false;

            if (Parameter?.SelectedWorkspace == null)
                return true;

            if (Parameter.SelectedWorkspace.TimelineViewModel.AnimationPlaybackViewModel.IsPlaybackActive)
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            var Parameter = parameter as IHasWorkspaceCollection;

            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                AddExtension = true,
                Filter = "Animation Workspace | *.anws",
                DefaultExt = "anws",
                ValidateNames = true,
                Multiselect = false
            };

            openFileDialog.FileOk += (______, _) =>
            {
                OpenWorkspaceFile(openFileDialog.FileName, Parameter);
            };

            openFileDialog.ShowDialog(App.Current.MainWindow);
        }

        public void OpenWorkspaceFile(string fileName, IHasWorkspaceCollection workspaceManager)
        {
            var f = WorkspaceFileModel.OpenWorkspaceFile(fileName, JsonSerializerOptions);
            var w = new WorkspaceViewModel(f);
            workspaceManager.AddWorkspace(w);
        }
    }
}
