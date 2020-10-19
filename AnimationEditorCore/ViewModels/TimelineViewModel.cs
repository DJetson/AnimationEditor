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

        private ObservableCollection<IFrameViewModel> _SelectedFrames;
        public ObservableCollection<IFrameViewModel> SelectedFrames
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

        public StrokeCollection FlattenFrames(List<KeyFrameViewModel> frames, bool ExcludeHiddenLayers = false)
        {
            var strokes = new StrokeCollection();
            List<KeyFrameViewModel> flattenFrames = frames;
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

        public ObservableCollection<IFrameViewModel> NextFrame => new ObservableCollection<IFrameViewModel>(OnionSkinUtilities.GetAllLayerFramesAtIndex(Layers.ToList(), SelectedFrameIndex + 1));
        public ObservableCollection<IFrameViewModel> PreviousFrame => new ObservableCollection<IFrameViewModel>(OnionSkinUtilities.GetAllLayerFramesAtIndex(Layers.ToList(), SelectedFrameIndex - 1));

        public void UpdateSelectedFrames()
        {
            var selected = new List<IFrameViewModel>();

            foreach (var layer in Layers)
            {
                layer.SelectedFrameIndex = SelectedFrameIndex;

                foreach (var frame in layer.Frames)
                {
                    if (frame.Order == layer.SelectedFrameIndex)
                        selected.Add(frame);
                }
            }
            SelectedFrames = new ObservableCollection<IFrameViewModel>(selected);
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

        public void AddBlankKeyFrameToTimeline(int index, bool updateSelected = true)
        {
            foreach (var layer in Layers)
            {
                var newFrame = new KeyFrameViewModel(layer, index);

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
                var newFrame = frame.Duplicate(index);

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

        public IFrameViewModel GetActiveFrameAtIndex(int index)
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
                    KeyFrameViewModel newFrame = null;
                    if (layer.Frames.Count == 0)
                    {
                        newFrame = new KeyFrameViewModel(layer, 0);
                        layer.AddFrameAtIndex(newFrame, 0);
                        continue;
                    }

                }
            }
            SelectedFrameIndex = Math.Max(SelectedFrameIndex - 1, 0);
        }

        public TimelineViewModel(List<LayerModel> layers, WorkspaceViewModel workspace)
        {
            WorkspaceViewModel = workspace;
            AnimationPlaybackViewModel = new AnimationPlaybackViewModel(this);

            Layers = new LayerCollection(this);

            foreach (var item in layers)
            {
                var newLayer = new LayerViewModel(item, this);
                Layers.Add(newLayer);
                FrameCount = Math.Max(FrameCount, newLayer.Frames.Count);

                if (newLayer.IsActive)
                    Layers.ActiveLayer = newLayer;
            }

            SelectedFrameIndex = Layers.FirstOrDefault().SelectedFrameIndex;
        }

        public TimelineViewModel(WorkspaceViewModel workspace)
        {
            WorkspaceViewModel = workspace;
            AnimationPlaybackViewModel = new AnimationPlaybackViewModel(this);

            InitializeTimeline();
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

            foreach (var layer in originalTimeline.Layers)
            {
                var clonedLayer = layer.Clone();
                clonedLayer.TimelineViewModel = this;
                Layers.Add(clonedLayer);
            }

            SelectedFrameIndex = originalTimeline.SelectedFrameIndex;

            if (originalTimeline.Layers.ActiveLayer == null)
                Layers.ActiveLayer = Layers[Layers.BottomZIndex];
            else
                Layers.ActiveLayer = Layers[originalTimeline.Layers.ActiveLayerIndex];
        }

        public void InitializeTimeline()
        {
            Layers = new LayerCollection(this);
            var newLayer = new LayerViewModel(this, 0);
            var newFrame = new KeyFrameViewModel(newLayer, 0);
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

            Layers.AddBlankLayerAtIndex(insertAtIndex);
        }

        public void DuplicateActiveLayer(LayerNavigation direction)
        {
            var duplicateLayerIndex = Layers.ActiveLayerIndex;

            if (direction == LayerNavigation.Above)
                duplicateLayerIndex += 1;

            Layers.DuplicateLayer(Layers.ActiveLayer, duplicateLayerIndex);
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

        //public TimelineState CreateUndoState(string title, List<UndoStateViewModel> additionalStates = null)
        //{
        //    return new TimelineState(this, title);
        //}

        //public void PushUndoRecord(UndoStateViewModel nextState, bool raiseChangedFlag = true)
        //{
        //    WorkspaceViewModel.WorkspaceHistoryViewModel.AddHistoricalState(nextState, raiseChangedFlag);
        //}

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
