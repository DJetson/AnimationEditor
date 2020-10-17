using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Interfaces;
using AnimationEditorCore.Models;
using AnimationEditorCore.ViewModels.Classes;
using AnimationEditorCore.Utilities;
using AnimationEditorCore.ViewModels.StateObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Ink;
using System.Windows.Media.Imaging;

namespace AnimationEditorCore.ViewModels
{
    public class TimelineViewModel : ViewModelBase
    {
        private LayerCollection _Layers;
        public LayerCollection Layers
        {
            get { return _Layers; }
            set { _Layers = value; NotifyPropertyChanged(); }
        }

        private AnimationPlaybackViewModel _AnimationPlaybackViewModel;
        public AnimationPlaybackViewModel AnimationPlaybackViewModel
        {
            get { return _AnimationPlaybackViewModel; }
            set { _AnimationPlaybackViewModel = value; NotifyPropertyChanged(); }
        }

        public string CurrentIndexOutOfFrameCount
        {
            get => $"{SelectedFrameIndex}/{LastFrameIndex}";
        }

        private int _SelectedFrameIndex = 0;
        public int SelectedFrameIndex
        {
            get { return _SelectedFrameIndex; }
            set
            {
                _SelectedFrameIndex = value;
                NotifyPropertyChanged(nameof(SelectedFrameIndex),
                                      nameof(PreviousFrameStrokes),
                                      nameof(PreviousOnionSkins),
                                      nameof(NextFrameStrokes),
                                      nameof(NextOnionSkins),
                                      nameof(CurrentIndexOutOfFrameCount));
                UpdateSelectedFrames();
            }
        }

        private ObservableCollection<FrameViewModel> _SelectedFrames;
        public ObservableCollection<FrameViewModel> SelectedFrames
        {
            get { return _SelectedFrames; }
            set { _SelectedFrames = value; NotifyPropertyChanged(); }
        }

        public ObservableCollection<StrokeCollection> NextOnionSkins
        {
            get
            {
                return new ObservableCollection<StrokeCollection>(OnionSkinUtilities.GetAllSucceedingOnionSkins(Layers.ToList(), SelectedFrameIndex, Properties.Settings.Default.NextFramesSkinCount));
            }
        }

        public StrokeCollection NextFrameStrokes
        {
            get
            {
                var nextOnionSkins = new StrokeCollection(OnionSkinUtilities.GetAllSucceedingOnionSkins(Layers.ToList(), SelectedFrameIndex, Properties.Settings.Default.NextFramesSkinCount).SelectMany(e => e));

                if (nextOnionSkins == null)
                    return new StrokeCollection();
                else
                {
                    return nextOnionSkins;
                }
            }
        }

        public ObservableCollection<StrokeCollection> PreviousOnionSkins
        {
            get
            {
                return new ObservableCollection<StrokeCollection>(OnionSkinUtilities.GetAllPrecedingOnionSkins(Layers.ToList(), SelectedFrameIndex, Properties.Settings.Default.PreviousFrameSkinCount));
            }
        }

        public StrokeCollection PreviousFrameStrokes
        {
            get
            {
                var previousOnionSkins = new StrokeCollection(OnionSkinUtilities.GetAllPrecedingOnionSkins(Layers.ToList(), SelectedFrameIndex, Properties.Settings.Default.PreviousFrameSkinCount).SelectMany(e => e));

                if (previousOnionSkins == null)
                    return new StrokeCollection();
                else
                {
                    return previousOnionSkins;
                }
            }
        }

        public StrokeCollection FlattenFrames(List<FrameViewModel> frames, bool ExcludeHiddenLayers = false)
        {
            var strokes = new StrokeCollection();
            List<FrameViewModel> flattenFrames = frames;
            if (ExcludeHiddenLayers)
            {
                flattenFrames = frames.Where(e => e.LayerViewModel.IsVisible).ToList();
            }

            foreach (var frame in flattenFrames)
            {
                strokes.Add(frame.StrokeCollection);
            }

            return strokes;
        }

        public ObservableCollection<FrameViewModel> NextFrame => new ObservableCollection<FrameViewModel>(OnionSkinUtilities.GetAllLayerFramesAtIndex(Layers.ToList(), SelectedFrameIndex + 1));
        public ObservableCollection<FrameViewModel> PreviousFrame => new ObservableCollection<FrameViewModel>(OnionSkinUtilities.GetAllLayerFramesAtIndex(Layers.ToList(), SelectedFrameIndex - 1));

        public void UpdateSelectedFrames()
        {
            var selected = new List<FrameViewModel>();

            foreach (var layer in Layers)
            {
                layer.SelectedFrameIndex = SelectedFrameIndex;

                foreach (var frame in layer.Frames)
                {
                    if (frame.Order == layer.SelectedFrameIndex)
                        selected.Add(frame);
                }
            }
            SelectedFrames = new ObservableCollection<FrameViewModel>(selected);
        }

        private WorkspaceViewModel _WorkspaceViewModel;
        public WorkspaceViewModel WorkspaceViewModel
        {
            get { return _WorkspaceViewModel; }
            set { _WorkspaceViewModel = value; NotifyPropertyChanged(); }
        }

        private double _FramesPerSecond = 24;
        public double FramesPerSecond
        {
            get { return _FramesPerSecond; }
            set
            {
                _FramesPerSecond = value;
                NotifyPropertyChanged();
                if (AnimationPlaybackViewModel != null)
                {
                    AnimationPlaybackViewModel.AnimationFps = value;
                }
            }
        }

        private double _CanvasWidth = 700;
        public double CanvasWidth
        {
            get { return _CanvasWidth; }
            set { _CanvasWidth = value; NotifyPropertyChanged(); }
        }

        private double _CanvasHeight = 380;
        public double CanvasHeight
        {
            get { return _CanvasHeight; }
            set { _CanvasHeight = value; NotifyPropertyChanged(); }
        }

        public void AddBlankFrameToTimeline(int index, bool updateSelected = true)
        {
            foreach (var layer in Layers)
            {
                var newFrame = new FrameViewModel(layer, index);

                layer.AddFrameAtIndex(newFrame, index);
                FrameCount = Math.Max(layer.Frames.Count, FrameCount);

                if (updateSelected)
                    layer.SelectedFrameIndex = index;
            }

            if (updateSelected)
            {
                if (index <= SelectedFrameIndex)
                    SelectedFrameIndex++;

                SelectedFrameIndex = index;
            }
        }

        public void DuplicateCurrentFrameToTimeline(int index)
        {
            foreach (var frame in SelectedFrames)
            {
                var newFrame = FrameViewModel.DuplicateFrame(frame, index);

                newFrame.LayerViewModel.AddFrameAtIndex(newFrame, index);
                FrameCount = Math.Max(newFrame.LayerViewModel.Frames.Count, FrameCount);

                newFrame.LayerViewModel.SelectedFrameIndex = index;
            }

            if (index <= SelectedFrameIndex)
                SelectedFrameIndex++;

            SelectedFrameIndex = index;
        }

        public void RemoveFrame(int frameIndex)
        {
            foreach (var layer in Layers)
            {
                layer.Frames.Remove(layer.Frames[frameIndex]);
                layer.UpdateFrameOrderIds();
            }
            FrameCount = AnimationUtilities.GetFrameCount(Layers.ToList());
        }

        public FrameViewModel GetActiveFrameAtIndex(int index)
        {
            if (IsFrameIndexValid(index))
                return Layers.ActiveLayer.Frames[index];
            else
                return null;
        }

        

        public bool IsFrameIndexValid(int index)
        {
            if (index >= 0 && index < FrameCount)
                return true;

            return false;
        }


        public void DeleteCurrentFrame()
        {
            int selectedFrameIndex = SelectedFrameIndex;

            RemoveFrame(selectedFrameIndex);

            if (FrameCount == 0)
            {
                foreach (var layer in Layers)
                {
                    FrameViewModel newFrame = null;
                    if (layer.Frames.Count == 0)
                    {
                        newFrame = new FrameViewModel(layer, 0);
                        layer.AddFrameAtIndex(newFrame, 0);
                        continue;
                    }

                }
            }
            SelectedFrameIndex = Math.Max(SelectedFrameIndex - 1, 0);

            //PushUndoRecord(CreateUndoState("Delete Frame"));
        }

        public TimelineViewModel(List<LayerModel> layers, WorkspaceViewModel workspace)
        {
            WorkspaceViewModel = workspace;
            AnimationPlaybackViewModel = new AnimationPlaybackViewModel(this);

            Layers = new LayerCollection(this);
            //InitializeLayerViewSource();

            foreach (var item in layers)
            {
                var newLayer = new LayerViewModel(item, this);
                Layers.Add(newLayer);
                FrameCount = Math.Max(FrameCount, newLayer.Frames.Count);

                if (newLayer.IsActive)
                    Layers.ActiveLayer = newLayer;
            }

            SelectedFrameIndex = Layers.FirstOrDefault().SelectedFrameIndex;

            PushUndoRecord(CreateUndoState("Opened Workspace"), false);
        }

        public TimelineViewModel(WorkspaceViewModel workspace)
        {
            WorkspaceViewModel = workspace;
            AnimationPlaybackViewModel = new AnimationPlaybackViewModel(this);

            //InitializeLayerViewSource();
            InitializeTimeline();

            PushUndoRecord(CreateUndoState("New Workspace"), false);
        }

        public TimelineViewModel(TimelineViewModel originalTimeline)
        {
            WorkspaceViewModel = originalTimeline.WorkspaceViewModel;

            AnimationPlaybackViewModel = originalTimeline.AnimationPlaybackViewModel;
            DisplayName = originalTimeline.DisplayName;
            FrameCount = originalTimeline.FrameCount;
            FramesPerSecond = originalTimeline.FramesPerSecond;
            FrameWidth = originalTimeline.FrameWidth;
            CanvasHeight = originalTimeline.CanvasHeight;
            CanvasWidth = originalTimeline.CanvasWidth;

            Layers = new LayerCollection(this);
            //InitializeLayerViewSource();

            foreach (var layer in originalTimeline.Layers)
            {
                var clonedLayer = layer.Clone();
                clonedLayer.TimelineViewModel = this;
                Layers.Add(clonedLayer);
            }

            SelectedFrameIndex = originalTimeline.SelectedFrameIndex;

            //if (originalTimeline.Layers.Where(e => e.IsActive).Count() == 0)
            //    Layers.ActiveLayer = Layers[0];
            //else
            //    Layers.ActiveLayer = Layers[originalTimeline.Layers.IndexOf(originalTimeline.Layers.Where(e => e.IsActive).FirstOrDefault())];
            if (originalTimeline.Layers.ActiveLayer == null)
                Layers.ActiveLayer = Layers[Layers.BottomZIndex];
            else
                Layers.ActiveLayer = Layers[originalTimeline.Layers.ActiveLayerIndex];
        }

        public void InitializeTimeline()
        {
            Layers = new LayerCollection(this);
            var newLayer = new LayerViewModel(this, 0);
            var newFrame = new FrameViewModel(newLayer, 0);
            newLayer.AddFrameAtIndex(newFrame, 0);
            Layers.AddLayerAtIndex(newLayer, 0);
            FrameCount = AnimationUtilities.GetFrameCount(Layers.ToList());
            SelectedFrameIndex = 0;
            Layers.ActiveLayer = newLayer;
        }

        public void AddBlankLayer(LayerNavigation direction)
        {
            var insertAtIndex = Layers.ActiveLayerIndex;

            if (direction == LayerNavigation.Above)
                insertAtIndex += 1;

            var newLayer = new LayerViewModel(this, insertAtIndex, "");
            newLayer.Frames = new ObservableCollection<FrameViewModel>();

            for (int i = 0; i < FrameCount; i++)
            {
                var newFrame = new FrameViewModel(newLayer, i);
                newLayer.AddFrameAtIndex(newFrame, i);
            }

            newLayer.SelectedFrameIndex = SelectedFrameIndex;

            AddLayerAtIndex(newLayer, newLayer.ZIndex);
            Layers.Refresh();
        }

        public int GetFirstAvailableZIndexAbove(int zIndex = 0)
        {
            var zIndexes = Layers.Select(e => e.ZIndex);

            for (int i = zIndex; ; i++)
            {
                if (!zIndexes.Contains(i))
                    return i;
            }
        }

        public void AddBlankLayerAtIndex(int index)
        {
            var newLayer = new LayerViewModel(this, index, "");
            newLayer.Frames = new ObservableCollection<FrameViewModel>();

            for (int i = 0; i < FrameCount; i++)
            {
                var newFrame = new FrameViewModel(newLayer, i);
                newLayer.AddFrameAtIndex(newFrame, i);
            }
            newLayer.SelectedFrameIndex = SelectedFrameIndex;

            AddLayerAtIndex(newLayer, newLayer.ZIndex);
        }

        public void DeleteLayerFromTimeline(int zIndex)
        {
            //TODO: This should get a confirmation dialog that can optionally be skipped permanently (e.g. "Don't show this again" checkbox)
            if (zIndex < 0 || zIndex >= Layers.Count)
                throw new IndexOutOfRangeException($"No layer found at index:{zIndex}.");
            var toRemoveIndex = zIndex;
            var toRemove = Layers[toRemoveIndex];

            Layers.Remove(toRemove);
            LayerOrdering.ConsolidateZIndices(Layers.ToList());
            if (Layers.Count == 0)
            {
                InitializeTimeline();
            }
            else if(Layers.ActiveLayer == toRemove)
            { 
                var newActiveIndex = LayerOrdering.GetNextLayerZIndexBelow(Layers.ToList(), toRemoveIndex);
                if (newActiveIndex == -1)
                    Layers.ActiveLayer = Layers[Layers.BottomZIndex];
                else
                    Layers.ActiveLayer = Layers[newActiveIndex];
            }

            toRemove.ClearFrames();
            Layers.Refresh();
            //PushUndoRecord(CreateUndoState("Deleted Layer"));
        }

        public void DuplicateActiveLayer(LayerNavigation direction)
        {
            var layerClone = Layers.ActiveLayer.Clone();
            var duplicateLayerIndex = Layers.ActiveLayerIndex;

            if (direction == LayerNavigation.Above)
                duplicateLayerIndex += 1;

            layerClone.ZIndex = duplicateLayerIndex;
            layerClone.SelectedFrameIndex = SelectedFrameIndex;

            AddLayerAtIndex(layerClone, layerClone.ZIndex);

            //PushUndoRecord(CreateUndoState("Duplicate Layer"));
        }

        public void AddLayerAtIndex(LayerViewModel layer, int index, bool createUndoState = true)
        {
            if (String.IsNullOrWhiteSpace(layer.DisplayName))
            {
                layer.DisplayName = FileUtilities.GetUniqueNameForCollection(Layers.Select(e => e.DisplayName).ToList(), $"Layer {Layers?.Count ?? 0}");
            }
            else
            {
                layer.DisplayName = FileUtilities.GetUniqueNameForCollection(Layers.Select(e => e.DisplayName).ToList(), layer.DisplayName);
            }


            if (index < 0)
            {
                LayerOrdering.CreateSpaceAtZIndex(Layers.ToList(), index);
                Layers.Add(layer);
            }
            else if (index < Layers.Count)
            {
                LayerOrdering.CreateSpaceAtZIndex(Layers.ToList(), index);
                Layers.Add(layer);
            }
            else
            {
                Layers.Add(layer);
            }

            Layers.ActiveLayer = layer;
            UpdateSelectedFrames();
            Layers.Refresh();
        }

        //public void InitializeLayerViewSource()
        //{
        //    _SortedLayers.Source = Layers;
        //    SortedLayers.SortDescriptions.Add(new System.ComponentModel.SortDescription("ZIndex", ListSortDirection.Descending));
        //    SortedLayers.Refresh();
        //}

        private List<StrokeCollection> _FlattenedFrameStrokes = new List<StrokeCollection>();
        public List<StrokeCollection> FlattenedFrameStrokes
        {
            get { return _FlattenedFrameStrokes; }
            set { _FlattenedFrameStrokes = value; NotifyPropertyChanged(); }
        }

        private int _FrameCount;
        public int FrameCount
        {
            get { return _FrameCount; }
            set
            {
                _FrameCount = value;
                NotifyPropertyChanged(nameof(FrameCount),
                                      nameof(LastFrameIndex),
                                      nameof(ScrubberLength),
                                      nameof(CurrentIndexOutOfFrameCount));
            }
        }

        public int LastFrameIndex => Math.Max(0, FrameCount - 1);
        public double ScrubberLength => FrameCount * FrameWidth;

        private double _FrameWidth = 20.0d;
        public double FrameWidth
        {
            get { return _FrameWidth; }
            set { _FrameWidth = value; NotifyPropertyChanged(nameof(FrameWidth), nameof(ScrubberLength)); }
        }

        public TimelineState CreateUndoState(string title, List<UndoStateViewModel> additionalStates = null)
        {
            return new TimelineState(this, title);
        }

        public void PushUndoRecord(UndoStateViewModel nextState, bool raiseChangedFlag = true)
        {
            WorkspaceViewModel.WorkspaceHistoryViewModel.AddHistoricalState(nextState, raiseChangedFlag);
        }

        public UndoStateViewModel SaveState()
        {
            var memento = new TimelineState(this);

            memento.Originator = this;
            return memento;
        }

        public void ClearLayers()
        {
            foreach (var layer in Layers)
            {
                layer.ClearFrames();
            }

            Layers.Clear();
            Layers.Refresh();
        }

        //public void SwapLayers(int layerIndexOne, int layerIndexTwo)
        //{
        //    if(layerIndexOne >= 0 && layerIndexOne < Layers.Count 
        //    && layerIndexTwo >= 0 && layerIndexTwo < Layers.Count
        //    && layerIndexOne != layerIndexTwo)
        //    {
        //        var layerOne = Layers[layerIndexOne];
        //        var layerTwo = Layers[layerIndexTwo];



        //    }
        //}

        public static void CopyToTimeline(TimelineViewModel original, TimelineViewModel destination)
        {
            destination.ClearLayers();
            destination.WorkspaceViewModel = original.WorkspaceViewModel;

            destination.AnimationPlaybackViewModel = original.AnimationPlaybackViewModel;
            destination.DisplayName = original.DisplayName;
            destination.FrameCount = original.FrameCount;
            destination.FramesPerSecond = original.FramesPerSecond;
            destination.FrameWidth = original.FrameWidth;
            destination.CanvasHeight = original.CanvasHeight;
            destination.CanvasWidth = original.CanvasWidth;

            destination.Layers = new LayerCollection(destination);

            foreach (var layer in original.Layers)
            {
                var clonedLayer = layer.Clone();
                clonedLayer.TimelineViewModel = destination;
                destination.Layers.Add(clonedLayer);
            }

            //destination.InitializeLayerViewSource();
            destination.SelectedFrameIndex = original.SelectedFrameIndex;
            destination.Layers.ActiveLayer = destination.Layers[original.Layers.ActiveLayerIndex];
            destination.Layers.NotifyPropertyChanged(nameof(LayerCollection.SortedLayers));
        }

        public void LoadState(UndoStateViewModel state)
        {
            var Memento = state as TimelineState;

            CopyToTimeline(Memento.Timeline, this);
        }
    }
}
