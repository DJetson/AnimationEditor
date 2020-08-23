using AnimationEditor.Interfaces;
using AnimationEditor.Models;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Ink;

namespace AnimationEditor.ViewModels
{
    public class WorkspaceViewModel : ViewModelBase
    {
        private IWorkspaceFileModel _WorkspaceModel;

        private IHasWorkspaceCollection _Host;
        public IHasWorkspaceCollection Host
        {
            get => _Host;
            set { _Host = value; NotifyPropertyChanged(); }
        }

        private string _Filepath;
        public string Filepath
        {
            get => _Filepath;
            set { _Filepath = value; NotifyPropertyChanged(); NotifyPropertyChanged(nameof(Filename)); }
        }

        public string Filename
        {
            get => Path.GetFileNameWithoutExtension(_DisplayName);
        }

        private StrokeCollection _StrokeCollection = new StrokeCollection();
        public StrokeCollection StrokeCollection
        {
            get => _StrokeCollection;
            set { _StrokeCollection = value; }
        }

        private ObservableCollection<Stroke> _Strokes = new ObservableCollection<Stroke>();
        public ObservableCollection<Stroke> Strokes
        {
            get => _Strokes;
            set { _Strokes = value; NotifyPropertyChanged(); }
        }

        private bool _HasUnsavedChanges = false;
        public bool HasUnsavedChanges
        {
            get => _HasUnsavedChanges;
            set { _HasUnsavedChanges = value; NotifyPropertyChanged(); }
        }

        public override string DisplayName
        {
            get => $"{_DisplayName}{(_HasUnsavedChanges ? "*" : "")}";
            set { _DisplayName = value.TrimEnd('*'); NotifyPropertyChanged(); }
        }

        public WorkspaceViewModel()
        {
            StrokeCollection = new StrokeCollection();
            StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;
        }

        private void StrokeCollection_StrokesChanged(object sender, StrokeCollectionChangedEventArgs e)
        {
            foreach (var stroke in e.Added)
            {
                Strokes.Add(stroke);
            }
            foreach (var stroke in e.Removed)
            {
                Strokes.Remove(stroke);
            }
        }

        public void Close()
        {
            if (HasUnsavedChanges)
            {
                var mbResult = MessageBox.Show($"{Filename} has unsaved changes. Do you want to save before closing?", "Save changes?", MessageBoxButton.YesNoCancel);

                if (mbResult == MessageBoxResult.Yes)
                {
                    UpdateModelAndSaveWorkspace();
                }
                else if (mbResult == MessageBoxResult.No)
                {
                    Host.RemoveWorkspace(this);
                }
                else
                {
                    return;
                }

            }
        }
        public void UpdateModelAndSaveWorkspace()
        {
            //Prompt for a save location if needed
            if (String.IsNullOrWhiteSpace(Filepath))
            {
                Filepath = SelectDestinationFilepath();
                DisplayName = Path.GetFileNameWithoutExtension(Filepath);
            }

            //Update the backing model for this workspace before it is written to disk
            //NOTE: This might not be a good idea. I suspect that it would be really nice to have an actual workspace model 
            //      object for anything like API/Plugin/scripting support that gets planned in the future, but this will almost
            //      certainly add some easy to forget "state management" type garbage to the whole thing. I don't want to have to
            //      worry about Models and ViewModels constantly needing to be synced up manually, or worry about what might happen
            //      if those become critical steps and then someone forgets to do them. So maybe it'd just be easier to do what I
            //      always do and skip the Model layer altogether. But I'm going to keep it there for now and just tread carefully.
            //      For now the idea is to keep any sort of File I/O stuff in the model objects and to write any public API stuff against
            //      the models so that I or others could write companion apps in the future which leverage the existing workspace files.
            _WorkspaceModel = _WorkspaceModel ?? new WorkspaceFileModel();
            _WorkspaceModel.SyncToViewModel(this);

            _WorkspaceModel.SaveWorkspaceFile(Filepath);
            HasUnsavedChanges = false;
        }

        /// <summary>
        /// Prompt the user to select a filepath for the Animation project workspace file. NOTE: This doesn't actually
        /// write anything to the disk. This method merely covers the portion of the Save As... workflow that is not
        /// already handled by the SaveWorkspace workflow/method.
        /// NOTE: This may not be a good way to split up the Save/Save As responsibilities
        /// </summary>
        /// <returns>The selected filepath</returns>
        public string SelectDestinationFilepath()
        {
            string filepath = String.Empty;

            var saveFileDialog = new SaveFileDialog()
            {
                AddExtension = true,
                Filter = "Animation Workspace | *.anws",
                DefaultExt = "anws",
                FileName = Filename,
                OverwritePrompt = true,
                ValidateNames = true
            };

            saveFileDialog.FileOk += (sender, e) =>
            {
                filepath = (sender as SaveFileDialog).FileName;
            };

            var res = saveFileDialog.ShowDialog(App.Current.MainWindow);

            return filepath;
        }

        //NOTE: MAY NOT BE A GOOD IDEA
        private void SyncToModel(IWorkspaceFileModel model)
        {
            //Assign all of and only the NECESSARY properties from the model object to populate a ViewModel with this 
            //workspace.

            _WorkspaceModel = model;
            Filepath = _WorkspaceModel.Filepath;
            DisplayName = Path.GetFileNameWithoutExtension(_WorkspaceModel.Filepath);
        }
    }
}
