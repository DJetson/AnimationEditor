﻿using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Interfaces;
using AnimationEditorCore.ViewModels.StateObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
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
                foreach(var layer in Layers)
                {
                    if (layer == ActiveLayer)
                        layer.IsActive = true;
                    else
                        layer.IsActive = false;
                }
            }
        }

        private int _SelectedFrameIndex = 0;
        public int SelectedFrameIndex
        {
            get { return _SelectedFrameIndex; }
            set
            {
                _SelectedFrameIndex = value;
                NotifyPropertyChanged();
                UpdateSelectedFrames();
                //foreach(var layer in Layers)
                //{
                //    //SelectedFrameIndex = value;
                //    //layer.NotifyPropertyChanged(nameof(LayerViewModel.SelectedFrameIndex));
                //    //layer.NotifyPropertyChanged(nameof(LayerViewModel.SelectedFrame));
                //    layer.SelectedFrameIndex = value;
                //    layer.SelectedFrame = layer.Frames.Where(e => e.Order == SelectedFrameIndex).FirstOrDefault();
                //}
            }
        }

        private ObservableCollection<FrameViewModel> _SelectedFrames;
        public ObservableCollection<FrameViewModel> SelectedFrames
        {
            get { return _SelectedFrames; }
            set { _SelectedFrames = value; NotifyPropertyChanged(); }
        }

        public void UpdateSelectedFrames()
        {
            var selected = new List<FrameViewModel>();

            foreach (var layer in Layers)
            {
                foreach (var frame in layer.Frames)
                {
                    if (frame.Order == SelectedFrameIndex)
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

        private DelegateCommand _AddBlankFrame;
        public DelegateCommand AddBlankFrame
        {
            get { return _AddBlankFrame; }
            set { _AddBlankFrame = value; NotifyPropertyChanged(); }
        }

        public bool AddBlankFrame_CanExecute(object parameter)
        {
            //if (AnimationPlaybackViewModel.IsPlaybackActive)
            //    return false;

            if (Enum.TryParse<FrameNavigation>(parameter.ToString(), out FrameNavigation Parameter) == false)
                return false;

            if (Parameter == FrameNavigation.Current)
                return false;

            return true;
        }

        public void AddBlankFrame_Execute(object parameter)
        {
            int insertAtIndex = ActiveLayer.Frames.Count;
            var Parameter = (FrameNavigation)Enum.Parse(typeof(FrameNavigation), parameter.ToString());
            switch (Parameter)
            {
                case FrameNavigation.Start:
                    insertAtIndex = 0;
                    break;
                case FrameNavigation.Previous:
                    insertAtIndex = SelectedFrameIndex;
                    break;
                case FrameNavigation.Next:
                    insertAtIndex = SelectedFrameIndex + 1;
                    break;
                case FrameNavigation.End:
                    insertAtIndex = ActiveLayer.Frames.Count;
                    break;
            }

            var undoStates = new List<UndoStateViewModel>();

            foreach (var layer in Layers)
            {
                var newFrame = new FrameViewModel(layer, insertAtIndex);

                layer.AddFrameAtIndex(newFrame, insertAtIndex);
                layer.SelectedFrameIndex = insertAtIndex;

                undoStates.Add(newFrame.SaveState() as FrameState);
                undoStates.Add(layer.SaveState() as LayerState);
            }

            SelectedFrameIndex = insertAtIndex;
            FrameCount = GetFrameCountOfLongestLayer();

            PushUndoRecord(CreateUndoState("Add Frame", undoStates));
        }


        private DelegateCommand _AddBlankLayer;
        public DelegateCommand AddBlankLayer
        {
            get { return _AddBlankLayer; }
            set { _AddBlankLayer = value; NotifyPropertyChanged(); }
        }

        #region DeleteCurrentFrame Command
        private DelegateCommand _DeleteCurrentFrame;
        public DelegateCommand DeleteCurrentFrame
        {
            get { return _DeleteCurrentFrame; }
            set { _DeleteCurrentFrame = value; NotifyPropertyChanged(); }
        }

        public bool DeleteCurrentFrame_CanExecute(object parameter)
        {
            if (AnimationPlaybackViewModel.IsPlaybackActive)
                return false;

            return true;
        }

        public void RemoveFrame(int frameIndex)
        {
            foreach (var layer in Layers)
            {
                layer.Frames.Remove(layer.Frames[frameIndex]);
                layer.UpdateFrameOrderIds();
            }
            FrameCount = GetFrameCountOfLongestLayer();
        }

        public void DeleteCurrentFrame_Execute(object parameter)
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
            //var timelineState = SaveState() as AnimationTimelineState;

            //MultiState multiState = null;

            //if (newFrame != null)
            //{
            //    var frameState = newFrame.SaveState() as FrameState;
            //    multiState = new MultiState(this, DeleteCurrentFrame.DisplayName, frameState, timelineState);
            //}
            //else
            //{
            //    multiState = new MultiState(this, DeleteCurrentFrame.DisplayName, timelineState);
            //}

            //PushUndoRecord(multiState);
        }
        #endregion DeleteCurrentFrame Command


        public TimelineViewModel(WorkspaceViewModel workspace)
        {
            WorkspaceViewModel = workspace;
            InitializeTimeline();
            InitializeCommands();
        }

        public void InitializeCommands()
        {
            AddBlankFrame = new DelegateCommand("Add Blank Frame", AddBlankFrame_CanExecute, AddBlankFrame_Execute);
            AddBlankLayer = new DelegateCommand("Add Blank Layer", AddBlankLayer_CanExecute, AddBlankLayer_Execute);
        }

        public void InitializeTimeline()
        {
            var newLayer = new LayerViewModel(this, 0);
            var newFrame = new FrameViewModel(newLayer, 0);
            newLayer.AddFrameAtIndex(newFrame, 0);
            AddLayerAtIndex(newLayer, 0);
            SelectedFrameIndex = 0;
            ActiveLayer = newLayer;
            var undoStates = new List<UndoStateViewModel>();
            undoStates.Add(newFrame.SaveState() as FrameState);
            undoStates.Add(newLayer.SaveState() as LayerState);
            undoStates.Add(SaveState() as TimelineState);
            PushUndoRecord(CreateUndoState("New Workspace", undoStates));
        }

        private bool AddBlankLayer_CanExecute(object parameter)
        {
            return true;
        }

        private void AddBlankLayer_Execute(object parameter)
        {
            var newLayer = new LayerViewModel(this, ActiveLayer.LayerId + 1, "");
            newLayer.Frames = new ObservableCollection<FrameViewModel>();

            var undoStates = new List<UndoStateViewModel>();

            for (int i = 0; i < FrameCount; i++)
            {
                var newFrame = new FrameViewModel(newLayer, i);
                newLayer.AddFrameAtIndex(newFrame, i);

                undoStates.Add(newFrame.SaveState() as FrameState);
            }
            newLayer.SelectedFrameIndex = SelectedFrameIndex;

            AddLayerAtIndex(newLayer, newLayer.LayerId);

            undoStates.Add(newLayer.SaveState() as LayerState);
            PushUndoRecord(CreateUndoState("Added Layer", undoStates));
        }

        public void AddLayerAtIndex(LayerViewModel layer, int index, bool createUndoState = true)
        {
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
            //TODO: Reimplement this after the rest of the refactor is stabilized
            //if (createUndoState)
            //PushUndoRecord(CreateUndoState($"Added New {newLayer.DisplayName} to Frame {Order}"));
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
                NotifyPropertiesChanged(nameof(FrameCount),
                                        nameof(LastFrameIndex),
                                        nameof(ScrubberLength));
            }
        }

        public int LastFrameIndex => Math.Max(0, FrameCount - 1);
        public double ScrubberLength => FrameCount * FrameWidth;

        private double _FrameWidth = 20.0d;
        public double FrameWidth
        {
            get { return _FrameWidth; }
            set { _FrameWidth = value; NotifyPropertiesChanged(nameof(FrameWidth), nameof(ScrubberLength)); }
        }


        public int GetFrameCountOfLongestLayer(bool ExcludeHiddenLayers = false)
        {
            List<LayerViewModel> layers = null;

            if (ExcludeHiddenLayers)
            {
                layers = GetVisibleLayers();
            }
            else
            {
                layers = Layers.ToList();
            }

            int currentMax = 0;
            foreach (var layer in layers)
            {
                currentMax = Math.Max(layer.Frames.Count, currentMax);
            }

            return currentMax;
        }


        public int GetLastFrameIndex(bool ExcludeHiddenLayers = true)
        {
            List<LayerViewModel> layers = null;

            if (ExcludeHiddenLayers)
            {
                layers = GetVisibleLayers();
            }
            else
            {
                layers = Layers.ToList();
            }

            int currentMax = 0;
            foreach (var layer in layers)
            {
                currentMax = Math.Max(layer.Frames.Count, currentMax);
            }

            return currentMax;
        }

        public List<LayerViewModel> GetVisibleLayers()
        {
            return Layers.Where(e => e.IsVisible).ToList();
        }

        public void FlattenStrokesForFrameAtIndex(int frameIndex)
        {
            if (FlattenedFrameStrokes.Count > frameIndex)
            {
                foreach (var frame in Layers.SelectMany(e => e.Frames.Where(f => f.Order == frameIndex)))
                {
                    FlattenedFrameStrokes[frameIndex].Clear();
                    FlattenedFrameStrokes[frameIndex].Add(frame.StrokeCollection);
                }
            }
        }

        public void FlattenStrokesForPlayback()
        {
            //Create an appropriate number of stroke collections for each frame to be flattened
            FlattenedFrameStrokes = new List<StrokeCollection>(FrameCount);
            FlattenedFrameStrokes.ForEach(e => e = new StrokeCollection());

            //Iterate over all possible frame indexes in all layers up to the highest indexed frame in any layer
            for (int i = 0; i < FlattenedFrameStrokes.Count; i++)
            {
                //Iterate over all visible layers
                foreach (var layer in GetVisibleLayers())
                {
                    //If the current layer contains a frame with the index currently being evaluated...
                    if (layer.Frames.Select(e => e.Order).Contains(i))
                    {
                        //Add the strokes from the frame at the current index from the current layer to the
                        //flattened stroke collection
                        FlattenedFrameStrokes[i].Add(layer.StrokeCollection);
                    }
                }
            }
        }

        public List<System.Drawing.Image> RenderFrameBitmaps(InkCanvas canvas)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)canvas.ActualWidth, (int)canvas.ActualHeight, 96, 96, new System.Windows.Media.PixelFormat());
            List<System.Drawing.Image> frameImages = new List<System.Drawing.Image>();
            foreach (var strokes in FlattenedFrameStrokes)
            {
                canvas.Strokes = strokes;
                rtb.Render(canvas);

                var bitmap = new Bitmap(rtb.PixelWidth, rtb.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                var bitmapData = bitmap.LockBits(new Rectangle(System.Drawing.Point.Empty, bitmap.Size),
                    ImageLockMode.WriteOnly, bitmap.PixelFormat);

                rtb.CopyPixels(Int32Rect.Empty, bitmapData.Scan0,
                    bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

                bitmap.UnlockBits(bitmapData);

                System.Drawing.Image newImage = bitmap;
                frameImages.Add(newImage);
            }

            return frameImages;
        }

        public MultiState CreateUndoState(string title, List<UndoStateViewModel> additionalStates = null)
        {
            var state = SaveState() as TimelineState;

            if (additionalStates != null)
            {
                var allStates = new List<UndoStateViewModel>();
                allStates.Add(state);
                allStates.AddRange(additionalStates);

                return new MultiState(null, title, allStates);
            }
            else
            {
                return new MultiState(null, title, state);
            }
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

        public void LoadState(IMemento state)
        {
            var Memento = (state as TimelineState);
            ActiveLayer = null;
            Layers = new ObservableCollection<LayerViewModel>();

            foreach (var layer in Memento.Layers)
            {
                Layers.Add(layer.Clone());
            }

            ActiveLayer = Layers[Memento.Layers.IndexOf(Memento.ActiveLayer)];
            SelectedFrameIndex = Memento.SelectedFrameIndex;

            FrameCount = GetFrameCountOfLongestLayer();
        }
    }
}
