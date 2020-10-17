﻿using AnimationEditorCore.Utilities;
using Microsoft.VisualBasic.CompilerServices;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Data;

namespace AnimationEditorCore.ViewModels.Classes
{
    public class LayerCollection : ObservableCollection<LayerViewModel>, INotifyPropertyChanged
    {
        protected override event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName] string property = "", params string[] dependentProperties)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
            foreach (var dependentProperty in dependentProperties)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(dependentProperty));
            }
        }

        private TimelineViewModel _TimelineViewModel;
        public TimelineViewModel TimelineViewModel
        {
            get { return _TimelineViewModel; }
            set { _TimelineViewModel = value; NotifyPropertyChanged(); }
        }


        //private ObservableCollection<LayerViewModel> _Layers = new ObservableCollection<LayerViewModel>();
        //public ObservableCollection<LayerViewModel> Layers
        //{
        //    get { return this.i; }
        //    set { _Layers = value; NotifyPropertyChanged(); }
        //}

        private CollectionViewSource _SortedLayers = new CollectionViewSource();
        public CollectionView SortedLayers
        {
            get => (CollectionView)_SortedLayers.View;
        }

        private LayerViewModel _ActiveLayer;
        public LayerViewModel ActiveLayer
        {
            get { return _ActiveLayer; }
            set
            {
                _ActiveLayer = value;
                NotifyPropertyChanged(nameof(ActiveLayer), nameof(ActiveLayerIndex));
                foreach (var layer in Items)
                {
                    if (layer == ActiveLayer)
                        layer.IsActive = true;
                    else
                        layer.IsActive = false;
                }
            }
        }

        public int ActiveLayerIndex
        {
            get => ActiveLayer.ZIndex;
        }

        public void AddBlankLayerAtIndex(int index)
        {
            var newLayer = new LayerViewModel(TimelineViewModel, index, "");
            newLayer.Frames = new ObservableCollection<FrameViewModel>();

            for (int i = 0; i < TimelineViewModel.FrameCount; i++)
            {
                var newFrame = new FrameViewModel(newLayer, i);
                newLayer.AddFrameAtIndex(newFrame, i);
            }
            newLayer.SelectedFrameIndex = TimelineViewModel.SelectedFrameIndex;

            AddLayerAtIndex(newLayer, newLayer.ZIndex);
        }

        public void AddLayerAtIndex(LayerViewModel layer, int index, bool createUndoState = true)
        {
            if (String.IsNullOrWhiteSpace(layer.DisplayName))
            {
                layer.DisplayName = FileUtilities.GetUniqueNameForCollection(Items.Select(e => e.DisplayName).ToList(), $"Layer {Items?.Count ?? 0}");
            }
            else
            {
                layer.DisplayName = FileUtilities.GetUniqueNameForCollection(Items.Select(e => e.DisplayName).ToList(), layer.DisplayName);
            }


            if (index < 0)
            {
                LayerOrdering.CreateSpaceAtZIndex(Items.ToList(), index);
                Add(layer);
            }
            else if (index < Items.Count)
            {
                LayerOrdering.CreateSpaceAtZIndex(Items.ToList(), index);
                Add(layer);
            }
            else
            {
                Add(layer);
            }

            ActiveLayer = layer;
            UpdateSelectedFrames();
            SortedLayers.Refresh();
        }

        public void UpdateSelectedFrames()
        {
            var selected = new List<FrameViewModel>();

            foreach (var layer in Items)
            {
                layer.SelectedFrameIndex = TimelineViewModel.SelectedFrameIndex;

                foreach (var frame in layer.Frames)
                {
                    if (frame.Order == layer.SelectedFrameIndex)
                        selected.Add(frame);
                }
            }
            TimelineViewModel.SelectedFrames = new ObservableCollection<FrameViewModel>(selected);
        }

        public void InitializeLayerViewSource()
        {
            _SortedLayers.Source = this;
            SortedLayers.SortDescriptions.Add(new System.ComponentModel.SortDescription("ZIndex", ListSortDirection.Descending));
            SortedLayers.Refresh();
        }

        public LayerCollection(TimelineViewModel timeline) : base()
        {
            TimelineViewModel = timeline;
            InitializeLayerViewSource();
        }

        public LayerCollection(TimelineViewModel timeline, IEnumerable<LayerViewModel> collection) : base(collection)
        {
            TimelineViewModel = timeline;
            InitializeLayerViewSource();
        }

        public LayerCollection(TimelineViewModel timeline, List<LayerViewModel> list) : base(list)
        {
            TimelineViewModel = timeline;
            InitializeLayerViewSource();
        }

        public void Refresh()
        {
            SortedLayers.Refresh();
        }

        new public LayerViewModel this[int key]
        {
            get => Items.Where(e => e.ZIndex == key).First();
            set
            {
                Items[value.ZIndex] = value;
            }
        }
    }
}
