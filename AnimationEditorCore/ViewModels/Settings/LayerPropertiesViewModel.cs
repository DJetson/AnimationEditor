using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Commands;
using AnimationEditorCore.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;

namespace AnimationEditorCore.ViewModels.Settings
{
    public class LayerPropertiesViewModel : ViewModelBase
    {
        private int _LayerZIndex;
        public int LayerZIndex
        {
            get { return _LayerZIndex; }
            set { _LayerZIndex = value; NotifyPropertyChanged(); }
        }

        private Color _LayerDisplayColor;
        public Color LayerDisplayColor
        {
            get { return _LayerDisplayColor; }
            set { _LayerDisplayColor = value; NotifyPropertyChanged(); }
        }

        private string _LayerDisplayName;
        public string LayerDisplayName
        {
            get { return _LayerDisplayName; }
            set { _LayerDisplayName = value; NotifyPropertyChanged(); }
        }

        private LayerViewModel _SourceLayer = null;

        public LayerPropertiesViewModel()
        {
            InitializeCommands();
        }

        public LayerPropertiesViewModel(LayerViewModel layer)
        {
            InitializeCommands();

            _SourceLayer = layer;
            DisplayName = layer.DisplayName;
            LayerDisplayName = layer.DisplayName;
            LayerZIndex = layer.ZIndex;
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

            if (LayerZIndex != _SourceLayer.ZIndex)
            {
                UpdateLayerOrder();
                HasChanged = true;
            }

            if (LayerDisplayName != _SourceLayer.DisplayName)
            {
                UpdateDisplayName();
                HasChanged = true;
            }

            if(HasChanged)
            {
                //_SourceLayer.TimelineViewModel.PushUndoRecord(_SourceLayer.TimelineViewModel.CreateUndoState($"Modified Layer Properties"));
                Parameter.DialogResult = true;
            }

            Parameter.Close();
        }

        private void UpdateDisplayName()
        {
            _SourceLayer.DisplayName = LayerDisplayName;
        }

        private void UpdateLayerOrder()
        {
            if (LayerZIndex < 0)
                LayerZIndex = 0;

            if (LayerZIndex != _SourceLayer.ZIndex)
            {
                List<LayerViewModel> layers = _SourceLayer.TimelineViewModel.Layers.ToList();

                LayerOrdering.CreateSpaceAtZIndex(layers.ToList(), LayerZIndex);
                _SourceLayer.ZIndex = LayerZIndex;
                LayerOrdering.ConsolidateZIndices(layers.ToList());

                _SourceLayer.TimelineViewModel.Layers.Refresh();
            }
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
