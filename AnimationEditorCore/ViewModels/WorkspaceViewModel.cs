using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Interfaces;
using AnimationEditorCore.Models;
using AnimationEditorCore.Utilities;
using AnimationEditorCore.ViewModels.StateObjects;
using BumpKit;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;

namespace AnimationEditorCore.ViewModels
{
    public class WorkspaceViewModel : ViewModelBase
    {
        private WorkspaceFileModel _WorkspaceModel;

        public static double MaxZoomLevel => 5.0f;
        public static double MinZoomLevel => 0.25f;

        private double _ZoomLevel = 1.0f;
        public double ZoomLevel
        {
            get { return _ZoomLevel; }
            set { _ZoomLevel = value; NotifyPropertyChanged(); }
        }

        private IHasWorkspaceCollection _Host;
        public IHasWorkspaceCollection Host
        {
            get => _Host;
            set { _Host = value; NotifyPropertyChanged(); }
        }

        private EditorToolsViewModel _EditorTools;
        public EditorToolsViewModel EditorTools
        {
            get => _EditorTools;
            set { _EditorTools = value; NotifyPropertyChanged(); }
        }

        private string _Filepath;
        public string Filepath
        {
            get => _Filepath;
            set { _Filepath = value; NotifyPropertyChanged(); NotifyPropertyChanged(nameof(Filename)); }
        }

        private Stack<IMemento> _UndoStack = new Stack<IMemento>();
        public Stack<IMemento> UndoStack => _UndoStack;

        private Stack<IMemento> _RedoStack = new Stack<IMemento>();
        public Stack<IMemento> RedoStack => _RedoStack;

        //private OnionSkinVisibility _OnionSkinVisibility = OnionSkinVisibility.NextFramesOnly;
        //public OnionSkinVisibility OnionSkinVisibility
        //{
        //    get => _OnionSkinVisibility;
        //    set { _OnionSkinVisibility = value; NotifyPropertyChanged(); }
        //}

        public string Filename
        {
            get => Path.GetFileNameWithoutExtension(_DisplayName);
        }

        //private FrameViewModel _SelectedFrame;
        //public FrameViewModel SelectedFrame
        //{
        //    get => _SelectedFrame;
        //    set { _SelectedFrame = value; NotifyPropertyChanged(); }
        //}

        private AnimationTimelineViewModel _AnimationTimelineViewModel;
        public AnimationTimelineViewModel AnimationTimelineViewModel
        {
            get => _AnimationTimelineViewModel;
            set { _AnimationTimelineViewModel = value; NotifyPropertyChanged(); }
        }

        private WorkspaceHistoryViewModel _WorkspaceHistoryViewModel;
        public WorkspaceHistoryViewModel WorkspaceHistoryViewModel
        {
            get { return _WorkspaceHistoryViewModel; }
            set { _WorkspaceHistoryViewModel = value; NotifyPropertyChanged(); }
        }

        private bool _HasUnsavedChanges = false;
        public bool HasUnsavedChanges
        {
            get => _HasUnsavedChanges;
            set { _HasUnsavedChanges = value; NotifyPropertyChanged(); NotifyPropertyChanged(nameof(DisplayName)); }
        }

        public override string DisplayName
        {
            get => $"{_DisplayName}{(_HasUnsavedChanges ? "*" : "")}";
            set { _DisplayName = value.TrimEnd('*'); NotifyPropertyChanged(); }
        }

        private DelegateCommand _ExportToGif;
        public DelegateCommand ExportToGif
        {
            get { return _ExportToGif; }
            set { _ExportToGif = value; NotifyPropertyChanged(); }
        }


        #region ZoomIn Command

        private DelegateCommand _ZoomIn;
        public DelegateCommand ZoomIn
        {
            get { return _ZoomIn; }
            set { _ZoomIn = value; NotifyPropertyChanged(); }
        }

        public bool ZoomIn_CanExecute(object parameter)
        {
            if (!(parameter is EditorToolType))
                return false;

            if ((EditorToolType)parameter != EditorToolType.Zoom)
                return false;

            if (ZoomLevel >= MaxZoomLevel)
                return false;

            return true;
        }

        public void ZoomIn_Execute(object parameter)
        {
            ZoomLevel += 0.25f;
            ZoomLevel = Math.Min(MaxZoomLevel, ZoomLevel);
        }
        #endregion ZoomIn Command

        #region ZoomOut Command
        private DelegateCommand _ZoomOut;
        public DelegateCommand ZoomOut
        {
            get { return _ZoomOut; }
            set { _ZoomOut = value; NotifyPropertyChanged(); }
        }

        public bool ZoomOut_CanExecute(object parameter)
        {
            if (!(parameter is EditorToolType))
                return false;

            if ((EditorToolType)parameter != EditorToolType.Zoom)
                return false;

            if (ZoomLevel <= MinZoomLevel)
                return false;

            return true;
        }

        public void ZoomOut_Execute(object parameter)
        {
            ZoomLevel -= 0.25f;
            ZoomLevel = Math.Max(MinZoomLevel, ZoomLevel);
        }
        #endregion ZoomOut Command

        public bool ExportToGif_CanExecute(object parameter)
        {
            if (!(parameter is InkCanvas))
                return false;

            return true;
        }

        public void ExportToGif_Execute(object parameter)
        {
            var canvas = parameter as InkCanvas;

            ExportAnimation(canvas);
        }


        //private System.Text.Json.JsonSerializerOptions JsonSerializerOptions { get; }

        public void InitializeCommands()
        {
            ZoomIn = new DelegateCommand("Zoom In", ZoomIn_CanExecute, ZoomIn_Execute);
            ZoomOut = new DelegateCommand("Zoom Out", ZoomOut_CanExecute, ZoomOut_Execute);
            ExportToGif = new DelegateCommand(ExportToGif_CanExecute, ExportToGif_Execute);
        }

        public void InitializeDependentViewModels()
        {
            WorkspaceHistoryViewModel = new WorkspaceHistoryViewModel(this);
            AnimationTimelineViewModel = new AnimationTimelineViewModel(this);
            EditorTools = EditorToolsViewModel.Instance;
        }

        public WorkspaceViewModel()
        {
            InitializeDependentViewModels();
            InitializeCommands();

            //JsonSerializerOptions = new System.Text.Json.JsonSerializerOptions();
            //JsonSerializerOptions.Converters.Add(new Models.StrokeCollectionConverter());
        }

        public WorkspaceViewModel(WorkspaceFileModel model)
        {
            InitializeCommands();
            _WorkspaceModel = model;

            WorkspaceHistoryViewModel = new WorkspaceHistoryViewModel(this);

            AnimationTimelineViewModel = new AnimationTimelineViewModel(model.Frames, this);
            EditorTools = EditorToolsViewModel.Instance;

            Filepath = model.Filepath;
            //DisplayName = Filename);
            DisplayName = Path.GetFileNameWithoutExtension(Filepath);
            //NotifyPropertyChanged(nameof(Filename));
            //JsonSerializerOptions = new System.Text.Json.JsonSerializerOptions();
            //JsonSerializerOptions.Converters.Add(new Models.StrokeCollectionConverter());
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
            Host.RemoveWorkspace(this);
        }

        public void ExportAnimation(InkCanvas canvas)
        {
            Filepath = SelectExportFilepath();
            SaveAsGif(Filepath, canvas);
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
            //_WorkspaceModel.SaveWorkspaceFile(Filepath, JsonSerializerOptions);
            //HasUnsavedChanges = false;

            WorkspaceHistoryViewModel.InitialState = WorkspaceHistoryViewModel.CurrentState;
        }

        public void SaveAsGif(string filepath, InkCanvas canvas)
        {

            try
            {
                using (FileStream fs = new FileStream(filepath, FileMode.Create))
                {
                    GifEncoder gEnc = new GifEncoder(fs);

                    //var bmpFrames = GetBmpFrames();
                    //foreach (var frame in bmpFrames.Frames)
                    //{
                    var frameBitmaps = this.AnimationTimelineViewModel.RenderFrameBitmaps(canvas);

                    foreach (var frame in frameBitmaps)
                    {
                        //StrokeCollection strokes = new StrokeCollection();
                        //foreach (var layer in frame.Layers)
                        //{
                        //    strokes.Add(layer.StrokeCollection);
                        //}
                        //fs.Write(ImageUtilities.ImageToByteArray(frame), 0, 0);
                        //using (MemoryStream m = new MemoryStream())
                        //{
                        //    strokes.Save(m);

                        //    byte[] imageBytes = m.ToArray();

                        gEnc.AddFrame(frame, 0, 0, new TimeSpan(100));
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Error", MessageBoxButton.OK);
            }
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

        public string SelectExportFilepath()
        {
            string filepath = String.Empty;

            var saveFileDialog = new SaveFileDialog()
            {
                AddExtension = true,
                Filter = "GIF with embedded ISF Metadata | *.gif",
                DefaultExt = "gif",
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
        private void SyncToModel(WorkspaceFileModel model)
        {
            //Assign all of and only the NECESSARY properties from the model object to populate a ViewModel with this 
            //workspace.

            _WorkspaceModel = model;
            Filepath = _WorkspaceModel.Filepath;
            DisplayName = Path.GetFileNameWithoutExtension(_WorkspaceModel.Filepath);
        }
    }
}
