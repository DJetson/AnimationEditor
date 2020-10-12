using AnimationEditorCore.BaseClasses;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;

namespace AnimationEditorCore.ViewModels.Settings
{
    public class AnimationPropertiesViewModel : ViewModelBase
    {
        private double _CanvasWidth;
        public double CanvasWidth
        {
            get { return _CanvasWidth; }
            set { _CanvasWidth = value; NotifyPropertyChanged(); }
        }

        private double _CanvasHeight;
        public double CanvasHeight
        {
            get { return _CanvasHeight; }
            set { _CanvasHeight = value; NotifyPropertyChanged(); }
        }

        private double _FramesPerSecond;
        public double FramesPerSecond
        {
            get { return _FramesPerSecond; }
            set { _FramesPerSecond = value; NotifyPropertyChanged(); }
        }

        private bool _IsDisplayForNewWorkspaceEnabled = false;
        public bool IsDisplayForNewWorkspaceEnabled
        {
            get { return _IsDisplayForNewWorkspaceEnabled; }
            set { _IsDisplayForNewWorkspaceEnabled = value; NotifyPropertyChanged(); }
        }

        private TimelineViewModel _Source = null;

        public AnimationPropertiesViewModel()
        {
            InitializeCommands();
        }

        public AnimationPropertiesViewModel(TimelineViewModel source)
        {
            _Source = source;
            CanvasWidth = _Source.CanvasWidth;
            CanvasHeight = _Source.CanvasHeight;
            FramesPerSecond = _Source.FramesPerSecond;
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            AcceptChanges = new DelegateCommand(AcceptChanges_CanExecute, AcceptChanges_Execute);
            DiscardChanges = new DelegateCommand(DiscardChanges_CanExecute, DiscardChanges_Execute);
        }

        private DelegateCommand _AcceptChanges;
        public DelegateCommand AcceptChanges
        {
            get { return _AcceptChanges; }
            set { _AcceptChanges = value; NotifyPropertyChanged(); }
        }

        private bool AcceptChanges_CanExecute(object parameter)
        {
            if (!(parameter is Window Parameter))
            {
                return false;
            }

            return true;
        }

        private void AcceptChanges_Execute(object parameter)
        {
            var Parameter = parameter as Window;
            bool HasChanged = false;

            if (UpdateCanvasHeight())
                HasChanged = true;

            if (UpdateCanvasWidth())
                HasChanged = true;

            if (UpdateFramesPerSecond())
                HasChanged = true;

            if (HasChanged && !IsDisplayForNewWorkspaceEnabled)
            {
                _Source.PushUndoRecord(_Source.CreateUndoState($"Modified Animation Properties"));
            }

            Parameter.DialogResult = true;

            Parameter.Close();
        }

        private bool UpdateCanvasHeight()
        {
            if (CanvasHeight == _Source.CanvasHeight)
                return false;

            if (CanvasHeight <= 0)
                return false;

            _Source.CanvasHeight = CanvasHeight;
            
            //TODO: Update any dependencies

            return true;
        }

        private bool UpdateCanvasWidth()
        {
            if (CanvasWidth == _Source.CanvasWidth)
                return false;
            if (CanvasWidth <= 0)
                return false;

            _Source.CanvasWidth = CanvasWidth;

            //TODO: Update any dependencies

            return true;
        }

        private bool UpdateFramesPerSecond()
        {
            if (FramesPerSecond == _Source.FramesPerSecond)
                return false;

            if (FramesPerSecond <= 1)
                return false;

            _Source.FramesPerSecond = FramesPerSecond;

            //TODO: Update any dependencies
            return true;
            //if (LayerZIndex < 0)
            //    LayerZIndex = 0;
            //else if (LayerZIndex > _SourceLayer.TimelineViewModel.Layers.Count - 1)
            //    LayerZIndex = _SourceLayer.TimelineViewModel.Layers.Count - 1;

            //if (LayerZIndex != _SourceLayer.LayerId)
            //{
            //    _SourceLayer.TimelineViewModel.Layers.Move(_SourceLayer.LayerId, LayerZIndex);
            //    _SourceLayer.LayerId = LayerZIndex;
            //    _SourceLayer.TimelineViewModel.UpdateLayerIds();
            //    _SourceLayer.TimelineViewModel.SortedLayers.Refresh();
            //}

            //Validate FPS
            //Assign FPS to Source
            //Update any dependencies
        }

        private DelegateCommand _DiscardChanges;
        public DelegateCommand DiscardChanges
        {
            get { return _DiscardChanges; }
            set { _DiscardChanges = value; NotifyPropertyChanged(); }
        }

        private bool DiscardChanges_CanExecute(object parameter)
        {
            if (!(parameter is Window Parameter))
            {
                return false;
            }

            return true;
        }

        private void DiscardChanges_Execute(object parameter)
        {
            var Parameter = parameter as Window;

            Parameter.DialogResult = false;

            Parameter.Close();
        }
    }
}
