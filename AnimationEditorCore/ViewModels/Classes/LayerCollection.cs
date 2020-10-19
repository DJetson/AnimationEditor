using AnimationEditorCore.Interfaces;
using AnimationEditorCore.Utilities;
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
            newLayer.Frames = new ObservableCollection<IFrameViewModel>();

            for (int i = 0; i < TimelineViewModel.FrameCount; i++)
            {
                var newFrame = new KeyFrameViewModel(newLayer, i);
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
            var selected = new List<IFrameViewModel>();

            foreach (var layer in Items)
            {
                layer.SelectedFrameIndex = TimelineViewModel.SelectedFrameIndex;

                foreach (var frame in layer.Frames)
                {
                    if (frame.Order == layer.SelectedFrameIndex)
                        selected.Add(frame);
                }
            }
            TimelineViewModel.SelectedFrames = new ObservableCollection<IFrameViewModel>(selected);
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

        public bool IsLayerIndexValid(int index)
        {
            if (Items.Select(e => e.ZIndex).Contains(index))
                return true;

            return false;
        }

        public void ActivateLayerAtIndex(int index)
        {
            if (IsLayerIndexValid(index))
                ActiveLayer = GetLayerAtZIndex(index);
        }


        new public LayerViewModel this[int key]
        {
            get => Items.Where(e => e.ZIndex == key).First();
            set
            {
                Items[value.ZIndex] = value;
            }
        }

        public int TopZIndex => Items.Select(e => e.ZIndex).Max();

        public int BottomZIndex => Items.Select(e => e.ZIndex).Min();

        public LayerViewModel GetLayerAbove(LayerViewModel layer)
        {
            if (layer == null)
                return null;

            if (layer.ZIndex == TopZIndex)
                return null;

            var layersAboveCurrent = Items.Where(e => e.ZIndex > layer.ZIndex);

            if (layersAboveCurrent.Count() == 0)
                return null;

            int aboveIndex = layersAboveCurrent.Select(e => e.ZIndex).Min();

            return GetLayerAtZIndex(aboveIndex);
        }

        public LayerViewModel GetLayerBelow(LayerViewModel layer)
        {
            if (layer == null)
                return null;

            if (layer.ZIndex == BottomZIndex)
                return null;

            var layersBelowCurrent = Items.Where(e => e.ZIndex < layer.ZIndex);

            if (layersBelowCurrent.Count() == 0)
                return null;

            int belowIndex = layersBelowCurrent.Select(e => e.ZIndex).Max();

            return GetLayerAtZIndex(belowIndex);
        }

        public void SwapLayerZIndex(LayerViewModel layerA, LayerViewModel layerB)
        {
            if (layerA == null || layerB == null)
                return;

            int tmp = layerA.ZIndex;
            layerA.ZIndex = layerB.ZIndex;
            layerB.ZIndex = tmp;

            Refresh();
        }

        public LayerViewModel GetLayerAtZIndex(int zIndex)
        {
            var layer = Items.Where(e => e.ZIndex == zIndex).FirstOrDefault();

            return layer;
        }

        public void RemoveLayerAtZIndex(int zIndex)
        {
            var toRemoveIndex = zIndex;
            var toRemove = Items[toRemoveIndex];

            Items.Remove(toRemove);
            LayerOrdering.ConsolidateZIndices(Items.ToList());
            if (Items.Count == 0)
            {
                AddBlankLayerAtIndex(0);
            }
            else if (ActiveLayer == toRemove)
            {
                var newActiveIndex = LayerOrdering.GetNextLayerZIndexBelow(Items.ToList(), toRemoveIndex);
                if (newActiveIndex == -1)
                    ActiveLayer = GetLayerAtZIndex(BottomZIndex);
                else
                    ActiveLayer = GetLayerAtZIndex(newActiveIndex);
            }

            toRemove.ClearFrames();
            Refresh();
        }

        public void DuplicateLayer(LayerViewModel layer, int insertAtZIndex)
        {
            var layerClone = layer.Clone();

            layerClone.ZIndex = insertAtZIndex;
            layerClone.SelectedFrameIndex = layer.SelectedFrameIndex;

            AddLayerAtIndex(layerClone, layerClone.ZIndex);
        }
    }
}
