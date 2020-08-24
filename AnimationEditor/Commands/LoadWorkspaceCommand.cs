using AnimationEditor.BaseClasses;
using AnimationEditor.Interfaces;
using AnimationEditor.Models;
using AnimationEditor.ViewModels;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationEditor.Commands
{
    public class LoadWorkspaceCommand : RequeryBase
    {
        public override bool CanExecute(object parameter)
        {
            if (!(parameter is IHasWorkspaceCollection Parameter))
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

            openFileDialog.FileOk += (sender, e) =>
            {
                var ofn = sender as OpenFileDialog;

                var newModel = WorkspaceFileModel.LoadWorkspaceFile(ofn.FileName);

                var newViewModel = new WorkspaceViewModel(newModel);

                Parameter.AddWorkspace(newViewModel);
            };

            openFileDialog.ShowDialog(App.Current.MainWindow);
        }
    }
}
