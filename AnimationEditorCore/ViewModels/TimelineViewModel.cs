using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Interfaces;
using AnimationEditorCore.Models;
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
    public class TimelineViewModel : ViewModelBase, IMementoOriginator
    {
        private ObservableCollection<LayerViewModel> _Layers = new ObservableCollection<LayerViewModel>();
        public ObservableCollection<LayerViewModel> Layers
        {
            get { return _Layers; }
            set { _Layers = value; NotifyPropertyChanged(); }
        }

        private CollectionViewSource _SortedLayers = new CollectionViewSource();
        public CollectionView SortedLayers
        {
            get => (CollectionView)_SortedLayers.View;
        }

        private AnimationPlaybackViewModel _AnimationPlaybackViewModel;
        public AnimationPlaybackViewModel AnimationPlaybackViewModel
        {
            get { return _AnimationPlaybackViewModel; }
            set { _AnimationPlaybackViewModel = value; NotifyPropertyChanged(); }
        }

        private LayerViewModel _ActiveLayer;
        public LayerViewModel ActiveLayer
        {
            get { return _ActiveLayer; }
            set
            {
                _ActiveLayer = value;
                NotifyPropertyChanged(nameof(ActiveLayer), nameof(ActiveLayerIndex));
                foreach (var layer in Layers)
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
            get => Layers.IndexOf(ActiveLayer);
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

                //if (NextFrame == null)
                //    return new StrokeCollection();

                //var strokes = FlattenFrames(NextFrame.ToList(), true).Clone();
                //foreach (var item in strokes)
                //{
                //    item.DrawingAttributes.IsHighlighter = true;
                //    item.DrawingAttributes.Color = System.Windows.Media.Color.FromArgb(128, 0, 255, 0);
                //}
                //return strokes;
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
                //if (PreviousFrame == null)
                //    return new StrokeCollection();

                //var strokes = FlattenFrames(PreviousFrame.ToList(), true).Clone();
                //foreach (var item in strokes)
                //{
                //    item.DrawingAttributes.IsHighlighter = true;
                //    item.DrawingAttributes.Color = System.Windows.Media.Color.FromArgb(128, 255, 0, 0);
                //}
                //return strokes;
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

        public void AddBlankFrameToTimeline(int index, bool updateSelected = true, bool createUndo = true)
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

            //if (createUndo)
                //PushUndoRecord(CreateUndoState($"Insert Frame {index}"));
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

            //PushUndoRecord(CreateUndoState("Duplicate Frame"));
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

        public bool IsLayerIndexValid(int index)
        {
            if (Layers.Select(e => e.LayerId).Contains(index))
                return true;

            return false;
        }

        public FrameViewModel GetActiveFrameAtIndex(int index)
        {
            if (IsFrameIndexValid(index))
                return ActiveLayer.Frames[index];
            else
                return null;
        }

        public bool IsFrameIndexValid(int index)
        {
            if (index >= 0 && index < FrameCount)
                return true;

            return false;
        }

        public void ActivateLayerAtIndex(int index)
        {
            if (IsLayerIndexValid(index))
                ActiveLayer = Layers[index];
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

            Layers = new ObservableCollection<LayerViewModel>();
            InitializeLayerViewSource();

            foreach (var item in layers)
            {
                var newLayer = new LayerViewModel(item, this);
                Layers.Add(newLayer);
                FrameCount = Math.Max(FrameCount, newLayer.Frames.Count);

                if (newLayer.IsActive)
                    ActiveLayer = newLayer;
            }

            SelectedFrameIndex = Layers.FirstOrDefault().SelectedFrameIndex;

            PushUndoRecord(CreateUndoState("Opened Workspace"), false);
        }

        public TimelineViewModel(WorkspaceViewModel workspace)
        {
            WorkspaceViewModel = workspace;
            AnimationPlaybackViewModel = new AnimationPlaybackViewModel(this);

            InitializeLayerViewSource();
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

            Layers = new ObservableCollection<LayerViewModel>();
            InitializeLayerViewSource();

            foreach (var layer in originalTimeline.Layers)
            {
                var clonedLayer = layer.Clone();
                clonedLayer.TimelineViewModel = this;
                Layers.Add(clonedLayer);
            }

            SelectedFrameIndex = originalTimeline.SelectedFrameIndex;

            if (originalTimeline.Layers.Where(e => e.IsActive).Count() == 0)
                ActiveLayer = Layers[0];
            else
                ActiveLayer = Layers[originalTimeline.Layers.IndexOf(originalTimeline.Layers.Where(e => e.IsActive).FirstOrDefault())];
        }

        public void InitializeTimeline()
        {
            var newLayer = new LayerViewModel(this, 0);
            var newFrame = new FrameViewModel(newLayer, 0);
            newLayer.AddFrameAtIndex(newFrame, 0);
            AddLayerAtIndex(newLayer, 0);
            FrameCount = AnimationUtilities.GetFrameCount(Layers.ToList());
            SelectedFrameIndex = 0;
            ActiveLayer = newLayer;
        }

        public void AddBlankLayer(LayerNavigation direction)
        {
            var insertAtIndex = ActiveLayerIndex;

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

            AddLayerAtIndex(newLayer, newLayer.LayerId);
            SortedLayers.Refresh();
            //PushUndoRecord(CreateUndoState("Added Layer"));
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

            AddLayerAtIndex(newLayer, newLayer.LayerId);
        }

        public void DeleteLayerFromTimeline(int index)
        {
            //TODO: This should get a confirmation dialog that can optionally be skipped permanently (e.g. "Don't show this again" checkbox)
            if (index < 0 || index >= Layers.Count)
                throw new IndexOutOfRangeException($"No layer found at index:{index}.");
            var toRemoveIndex = index;
            var toRemove = Layers[toRemoveIndex];

            Layers.Remove(toRemove);

            if (Layers.Count == 0)
            {
                InitializeTimeline();
            }
            else
            {
                var newActiveIndex = Math.Max(toRemoveIndex - 1, 0);
                ActiveLayer = Layers[newActiveIndex];
            }

            toRemove.ClearFrames();

            //PushUndoRecord(CreateUndoState("Deleted Layer"));
        }

        public void DuplicateActiveLayer(LayerNavigation direction)
        {
            var layerClone = ActiveLayer.Clone();
            var duplicateLayerIndex = ActiveLayerIndex;

            if (direction == LayerNavigation.Above)
                duplicateLayerIndex += 1;

            layerClone.SelectedFrameIndex = SelectedFrameIndex;

            AddLayerAtIndex(layerClone, layerClone.LayerId);

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
                Layers.Insert(0, layer);
            }
            else if (index < Layers.Count)
            {
                Layers.Insert(index, layer);
            }
            else
            {
                Layers.Add(layer);
            }

            ActiveLayer = layer;
            UpdateLayerIds();
            UpdateSelectedFrames();
            SortedLayers.Refresh();
        }

        public void InitializeLayerViewSource()
        {
            _SortedLayers.Source = Layers;
            SortedLayers.SortDescriptions.Add(new System.ComponentModel.SortDescription("LayerId", ListSortDirection.Descending));
            SortedLayers.Refresh();
        }

        public void UpdateLayerIds(int startIndex = 0)
        {
            foreach (var layer in Layers.Where(e => e.LayerId >= startIndex))
            {
                layer.LayerId = Layers.IndexOf(layer);
            }
        }

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

        public IMemento SaveState()
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
            SortedLayers.Refresh();
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

            destination.Layers = new ObservableCollection<LayerViewModel>();

            foreach (var layer in original.Layers)
            {
                var clonedLayer = layer.Clone();
                clonedLayer.TimelineViewModel = destination;
                destination.Layers.Add(clonedLayer);
            }

            destination.InitializeLayerViewSource();
            destination.SelectedFrameIndex = original.SelectedFrameIndex;
            destination.ActiveLayer = destination.Layers[original.Layers.IndexOf(original.ActiveLayer)];
            destination.NotifyPropertyChanged(nameof(SortedLayers));
        }

        public void LoadState(IMemento state)
        {
            var Memento = (state as TimelineState);

            CopyToTimeline(Memento.Timeline, this);
        }
    }
}
