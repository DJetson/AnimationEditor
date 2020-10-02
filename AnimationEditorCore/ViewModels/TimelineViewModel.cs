using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Interfaces;
using AnimationEditorCore.Models;
using AnimationEditorCore.Utilities;
using AnimationEditorCore.ViewModels.StateObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media.Imaging;

namespace AnimationEditorCore.ViewModels
{
    public enum FrameNavigation { Start, Previous, Current, Next, End };
    public class TimelineViewModel : ViewModelBase, IMementoOriginator
    {
        private ObservableCollection<LayerViewModel> _Layers = new ObservableCollection<LayerViewModel>();
        public ObservableCollection<LayerViewModel> Layers
        {
            get { return _Layers; }
            set { _Layers = value; NotifyPropertyChanged(); }
        }

        private AnimationPlaybackViewModel _AnimationPlaybackViewModel = new AnimationPlaybackViewModel();
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
                NotifyPropertyChanged();
                foreach (var layer in Layers)
                {
                    if (layer == ActiveLayer)
                        layer.IsActive = true;
                    else
                        layer.IsActive = false;
                }
            }
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
                                      nameof(NextFrameStrokes),
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

        public StrokeCollection NextFrameStrokes
        {
            get
            {
                if (NextFrame == null)
                    return new StrokeCollection();

                var strokes = FlattenFrames(NextFrame.ToList(), true).Clone();
                foreach (var item in strokes)
                {
                    item.DrawingAttributes.IsHighlighter = true;
                    item.DrawingAttributes.Color = System.Windows.Media.Color.FromArgb(128, 0, 255, 0);
                }
                return strokes;
            }
        }

        public StrokeCollection PreviousFrameStrokes
        {
            get
            {
                if (PreviousFrame == null)
                    return new StrokeCollection();

                var strokes = FlattenFrames(PreviousFrame.ToList(), true).Clone();
                foreach (var item in strokes)
                {
                    item.DrawingAttributes.IsHighlighter = true;
                    item.DrawingAttributes.Color = System.Windows.Media.Color.FromArgb(128, 255, 0, 0);
                }
                return strokes;
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

        public ObservableCollection<FrameViewModel> NextFrame => new ObservableCollection<FrameViewModel>(GetAllLayerFramesAtIndex(SelectedFrameIndex + 1));
        public ObservableCollection<FrameViewModel> PreviousFrame => new ObservableCollection<FrameViewModel>(GetAllLayerFramesAtIndex(SelectedFrameIndex - 1));

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

        public List<FrameViewModel> GetAllLayerFramesAtIndex(int index)
        {
            var frames = new List<FrameViewModel>();

            foreach (var layer in Layers)
            {
                foreach (var frame in layer.Frames)
                {
                    if (frame.Order == index)
                        frames.Add(frame);
                }
            }
            return frames;
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
            set { _FramesPerSecond = value; NotifyPropertyChanged(); }
        }

        public void AddBlankFrameToTimeline(int index)
        {
            foreach (var layer in Layers)
            {
                var newFrame = new FrameViewModel(layer, index);

                layer.AddFrameAtIndex(newFrame, index);
                FrameCount = Math.Max(layer.Frames.Count, FrameCount);

                layer.SelectedFrameIndex = index;
            }

            if (index <= SelectedFrameIndex)
                SelectedFrameIndex++;

            SelectedFrameIndex = index;

            PushUndoRecord(CreateUndoState($"Insert Frame {index}"));
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

            PushUndoRecord(CreateUndoState("Duplicate Frame"));
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

                    SelectedFrameIndex = 0;
                }
            }
            else
            {
                SelectedFrameIndex = Math.Max(SelectedFrameIndex - 1, 0);
            }

            PushUndoRecord(CreateUndoState("Delete Frame"));
        }

        public TimelineViewModel(List<LayerModel> layers, WorkspaceViewModel workspace)
        {
            WorkspaceViewModel = workspace;

            //InitializeCommands();

            Layers = new ObservableCollection<LayerViewModel>();
            foreach (var item in layers)
            {
                var newLayer = new LayerViewModel(item, this);
                Layers.Add(newLayer);
                FrameCount = Math.Max(FrameCount, newLayer.Frames.Count);
            }

            SelectedFrameIndex = Layers.FirstOrDefault().SelectedFrameIndex;
            PushUndoRecord(CreateUndoState("Opened Workspace"));
        }

        public TimelineViewModel(WorkspaceViewModel workspace)
        {
            WorkspaceViewModel = workspace;
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

            Layers = new ObservableCollection<LayerViewModel>();
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
            PushUndoRecord(CreateUndoState("New Workspace"));
        }

        public void AddBlankLayer()
        {
            var newLayer = new LayerViewModel(this, ActiveLayer.LayerId + 1, "");
            newLayer.Frames = new ObservableCollection<FrameViewModel>();

            for (int i = 0; i < FrameCount; i++)
            {
                var newFrame = new FrameViewModel(newLayer, i);
                newLayer.AddFrameAtIndex(newFrame, i);
            }
            newLayer.SelectedFrameIndex = SelectedFrameIndex;

            AddLayerAtIndex(newLayer, newLayer.LayerId);

            PushUndoRecord(CreateUndoState("Added Layer"));
        }

        public void AddLayerAtIndex(LayerViewModel layer, int index, bool createUndoState = true)
        {
            if (String.IsNullOrWhiteSpace(layer.DisplayName))
            {
                layer.DisplayName = FileUtilities.GetUniqueNameForCollection(Layers.Select(e => e.DisplayName).ToList(), $"Layer {Layers?.Count ?? 0}");
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

            destination.Layers = new ObservableCollection<LayerViewModel>();
            foreach (var layer in original.Layers)
            {
                var clonedLayer = layer.Clone();
                clonedLayer.TimelineViewModel = destination;
                destination.Layers.Add(clonedLayer);
            }

            destination.SelectedFrameIndex = original.SelectedFrameIndex;
            destination.ActiveLayer = destination.Layers[original.Layers.IndexOf(original.ActiveLayer)];
        }

        public void LoadState(IMemento state)
        {
            var Memento = (state as TimelineState);

            CopyToTimeline(Memento.Timeline, this);
        }
    }
}
