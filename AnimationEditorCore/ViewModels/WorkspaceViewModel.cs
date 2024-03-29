﻿using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Commands;
using AnimationEditorCore.Interfaces;
using AnimationEditorCore.Models;
using AnimationEditorCore.Utilities;
using BumpKit;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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

        private bool _IsRecoveredFile = false;
        public bool IsRecoveredFile
        {
            get { return _IsRecoveredFile; }
            set 
            { 
                _IsRecoveredFile = value; 
                NotifyPropertyChanged(nameof(IsRecoveredFile), 
                                      nameof(DisplayName),
                                      nameof(DisplayNameWithInfo)); 
            }
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

        public string Filename
        {
            get => Path.GetFileNameWithoutExtension(_DisplayName);
        }

        private TimelineViewModel _TimelineViewModel;
        public TimelineViewModel TimelineViewModel
        {
            get => _TimelineViewModel;
            set { _TimelineViewModel = value; NotifyPropertyChanged(); }
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
            set 
            { 
                _HasUnsavedChanges = value; 
                NotifyPropertyChanged(nameof(DisplayName),
                                      nameof(DisplayNameWithInfo)); 
            }
        }

        public override string DisplayName
        {
            get => _DisplayName;
            set 
            { 
                _DisplayName = value; 
                NotifyPropertyChanged(nameof(DisplayName),
                                      nameof(DisplayNameWithInfo)); 
            }
        }

        public string DisplayNameWithInfo
        {
            get => $"{_DisplayName}{(IsRecoveredFile ? "(recovered)" : "")}{(HasUnsavedChanges ? "*" : "")}";
        }

        private DelegateCommand _ExportToGif;
        public DelegateCommand ExportToGif
        {
            get { return _ExportToGif; }
            set { _ExportToGif = value; NotifyPropertyChanged(); }
        }

        private bool _IsMouseLeftButtonDown = false;
        private Point _MouseMoveStartPosition = default(Point);
        private double _StartingZoomLevel = 0;

        private DelegateCommand _RequestBeginZoom;
        public DelegateCommand RequestBeginZoom
        {
            get { return _RequestBeginZoom; }
            set { _RequestBeginZoom = value; NotifyPropertyChanged(); }
        }

        public bool RequestBeginZoom_CanExecute(object parameter)
        {
            if (EditorTools.SelectedToolType != EditorToolType.Zoom)
                return false;

            if (!(parameter is IInputElement))
                return false;

            return true;
        }

        public void RequestBeginZoom_Execute(object parameter)
        {
            _MouseMoveStartPosition = Mouse.GetPosition(parameter as IInputElement);
            _IsMouseLeftButtonDown = true;
            _StartingZoomLevel = ZoomLevel;
        }

        private DelegateCommand _RequestZoom;
        public DelegateCommand RequestZoom
        {
            get { return _RequestZoom; }
            set { _RequestZoom = value; NotifyPropertyChanged(); }
        }

        public bool RequestZoom_CanExecute(object parameter)
        {
            if (EditorTools.SelectedToolType != EditorToolType.Zoom)
                return false;

            if (!_IsMouseLeftButtonDown)
                return false;

            if (!(parameter is IInputElement))
                return false;

            return true;
        }

        public void RequestZoom_Execute(object parameter)
        {
            var mouseDelta = Mouse.GetPosition(parameter as IInputElement) - _MouseMoveStartPosition;

            var newZoomLevel = _StartingZoomLevel + (mouseDelta.X / 300);
            if (newZoomLevel > 0.5f && newZoomLevel < 6.0f)
                ZoomLevel = newZoomLevel;
        }

        private DelegateCommand _RequestEndZoom;
        public DelegateCommand RequestEndZoom
        {
            get { return _RequestEndZoom; }
            set { _RequestEndZoom = value; NotifyPropertyChanged(); }
        }

        public bool RequestEndZoom_CanExecute(object parameter)
        {
            if (!_IsMouseLeftButtonDown)
                return false;

            if (!(parameter is IInputElement))
                return false;

            return true;
        }

        public void RequestEndZoom_Execute(object parameter)
        {
            _MouseMoveStartPosition = default(Point);
            _IsMouseLeftButtonDown = false;
            _StartingZoomLevel = 0;
        }

        private string _TempFilePath = String.Empty;

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

        public void InitializeCommands()
        {
            RequestBeginZoom = new DelegateCommand(RequestBeginZoom_CanExecute, RequestBeginZoom_Execute);
            RequestZoom = new DelegateCommand(RequestZoom_CanExecute, RequestZoom_Execute);
            RequestEndZoom = new DelegateCommand(RequestEndZoom_CanExecute, RequestEndZoom_Execute);
            ZoomIn = new DelegateCommand("Zoom In", ZoomIn_CanExecute, ZoomIn_Execute);
            ZoomOut = new DelegateCommand("Zoom Out", ZoomOut_CanExecute, ZoomOut_Execute);
            ExportToGif = new DelegateCommand(ExportToGif_CanExecute, ExportToGif_Execute);
        }

        public void InitializeDependentViewModels()
        {
            WorkspaceHistoryViewModel = new WorkspaceHistoryViewModel(this);
            TimelineViewModel = new TimelineViewModel(this);
            EditorTools = EditorToolsViewModel.Instance;
        }

        public WorkspaceViewModel()
        {
            _TempFilePath = GetTemporaryFile();
            InitializeDependentViewModels();
            InitializeCommands();
        }

        public WorkspaceViewModel(WorkspaceFileModel model)
        {
            Filepath = model.Filepath;
            _TempFilePath = GetTemporaryFile();

            InitializeCommands();
            _WorkspaceModel = model;


            WorkspaceHistoryViewModel = new WorkspaceHistoryViewModel(this);

            TimelineViewModel = new TimelineViewModel(model.Layers, this)
            {
                FramesPerSecond = _WorkspaceModel.AnimationProperties.FramesPerSecond,
                CanvasWidth = _WorkspaceModel.CanvasProperties.Width,
                CanvasHeight = _WorkspaceModel.CanvasProperties.Height
            };

            EditorTools = EditorToolsViewModel.Instance;

            HasUnsavedChanges = false;
            DisplayName = Path.GetFileNameWithoutExtension(Filepath);
        }

        public bool Close()
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
                    //Host.RemoveWorkspace(this);
                    DeleteTempFile();
                }
                else
                {
                    return false;
                }
            }

            Host.RemoveWorkspace(this);
            return true;
        }

        public void WriteToTempFile()
        {
            //Prompt for a save location if needed
            if (String.IsNullOrWhiteSpace(_TempFilePath))
            {
                _TempFilePath = GetTemporaryFile();
            }

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

            _WorkspaceModel.SaveWorkspaceFile(_TempFilePath);

            //WorkspaceHistoryViewModel.InitialState = WorkspaceHistoryViewModel.CurrentState;
        }

        public void ExportAnimation(InkCanvas canvas)
        {
            var filepath = SelectExportFilepath();
            if ((!String.IsNullOrWhiteSpace(filepath)))
                SaveAsGif(filepath, canvas);
        }

        public void UpdateModelAndSaveWorkspace()
        {
            //Prompt for a save location if needed
            if (String.IsNullOrWhiteSpace(Filepath) || IsRecoveredFile)
            {
                Filepath = SelectDestinationFilepath();
                DisplayName = Path.GetFileNameWithoutExtension(Filepath);
            }

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
            WorkspaceHistoryViewModel.InitialState = WorkspaceHistoryViewModel.CurrentState;
            IsRecoveredFile = false;
            DeleteTempFile();
        }

        public void SaveAsGif(string filepath, InkCanvas canvas)
        {
            try
            {
                using (FileStream fs = new FileStream(filepath, FileMode.Create))
                {
                    GifEncoder gEnc = new GifEncoder(fs);
                    var frameBitmaps = AnimationUtilities.RenderFrameBitmaps(canvas, TimelineViewModel.Layers.ToList(), TimelineViewModel.CanvasWidth, TimelineViewModel.CanvasHeight);
                    var frameInterval = new TimeSpan((long)(TimeSpan.TicksPerSecond * (1.0 / (TimelineViewModel.FramesPerSecond))));
                    foreach (var frame in frameBitmaps)
                    {
                        gEnc.AddFrame(frame, (int)TimelineViewModel.CanvasWidth, (int)TimelineViewModel.CanvasHeight, frameInterval);
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

        public string GetTemporaryFile()
        {
            if (IsRecoveredFile)
            {
                return Filepath;
            }

            string tmpFilePathRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "AnimationEditor", "Recovered");
            string tmpFileName = String.Empty;
            if (String.IsNullOrEmpty(Filepath))
                tmpFileName = Path.GetFileName(Path.GetTempFileName());
            else
                tmpFileName = Path.GetFileName(Filepath);

            return Path.Combine(tmpFilePathRoot, $"{Path.GetFileNameWithoutExtension(tmpFileName)}.atmp");
        }

        private void DeleteTempFile()
        {
            File.Delete(_TempFilePath);
            _TempFilePath = String.Empty;
        }

    }
}
