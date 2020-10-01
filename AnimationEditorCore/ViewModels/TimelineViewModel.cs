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
                NotifyPropertiesChanged(nameof(SelectedFrameIndex),
                                        nameof(PreviousFrameStrokes),
                                        nameof(NextFrameStrokes),
                                        nameof(CurrentIndexOutOfFrameCount));
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

        #region PlayAnimation Command
        private DelegateCommand _PlayAnimation;
        public DelegateCommand PlayAnimation
        {
            get { return _PlayAnimation; }
            set { _PlayAnimation = value; NotifyPropertyChanged(); }
        }

        public void PlayAnimation_Execute(object parameter)
        {
            //NOTE: Eventually this will be able to be set to the set of currently selected frames as well
            FlattenStrokesForPlayback();
            var playbackFrames = FlattenedFrameStrokes.ToList();
            //playbackFrames.ForEach(e => e.FlattenStrokesForPlayback());

            //NOTE: I may have to eventually do some caching up top here. Rasterize all of the InkCanvases in the series
            //      and just flip through those instead of directly showing the InkCanvases. It will depend entirely on
            //      how well it performs in it's initial, most basic form.

            if (!AnimationPlaybackViewModel.IsPlaybackActive)
                AnimationPlaybackViewModel.StartPlayback(playbackFrames, FramesPerSecond);
        }

        public bool PlayAnimation_CanExecute(object parameter)
        {
            //TODO: Eventually I need to add the conditions to return false If we're currently 
            //in a "modal" state with the ink canvas for any reason, return false. We don't 
            //need an active Scale transform going all screwy because the user accidentally hits 
            //the play button before they resolve the resize, causing the playback object to yank 
            //the target frame right out from under them
            if (AnimationPlaybackViewModel.IsPlaybackActive)
            {
                return false;
            }

            return true;
        }
        #endregion PlayAnimation Command

        #region StopAnimation Command
        private DelegateCommand _StopAnimation;
        public DelegateCommand StopAnimation
        {
            get { return _StopAnimation; }
            set { _StopAnimation = value; NotifyPropertyChanged(); }
        }

        public void StopAnimation_Execute(object parameter)
        {
            if (AnimationPlaybackViewModel.IsPlaybackActive)
            {
                AnimationPlaybackViewModel.StopPlayback();
            }
        }

        public bool StopAnimation_CanExecute(object parameter)
        {
            if (AnimationPlaybackViewModel.IsPlaybackActive == false)
                return false;

            return true;
        }
        #endregion StopAnimation Command

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
                FrameCount = Math.Max(layer.Frames.Count, FrameCount);

                layer.SelectedFrameIndex = insertAtIndex;

                //undoStates.Add(newFrame.SaveState() as FrameState);
                //undoStates.Add(layer.SaveState() as LayerState);
            }

            if (insertAtIndex <= SelectedFrameIndex)
                SelectedFrameIndex++;

            SelectedFrameIndex = insertAtIndex;

            //undoStates.Add(SaveState() as TimelineState);
            PushUndoRecord(CreateUndoState("Add Frame"/*, undoStates*/));
        }


        private DelegateCommand _AddBlankLayer;
        public DelegateCommand AddBlankLayer
        {
            get { return _AddBlankLayer; }
            set { _AddBlankLayer = value; NotifyPropertyChanged(); }
        }

        #region DuplicateCurrentFrame Command
        private DelegateCommand _DuplicateCurrentFrame;
        public DelegateCommand DuplicateCurrentFrame
        {
            get { return _DuplicateCurrentFrame; }
            set { _DuplicateCurrentFrame = value; NotifyPropertyChanged(); }
        }

        public bool DuplicateCurrentFrame_CanExecute(object parameter)
        {
            if (AnimationPlaybackViewModel.IsPlaybackActive)
                return false;

            if (Enum.TryParse<FrameNavigation>(parameter.ToString(), out FrameNavigation Parameter) == false)
                return false;

            if (Parameter == FrameNavigation.Current || Parameter == FrameNavigation.Start || Parameter == FrameNavigation.End)
                return false;

            return true;
        }

        public void DuplicateCurrentFrame_Execute(object parameter)
        {
            var Parameter = (FrameNavigation)Enum.Parse(typeof(FrameNavigation), parameter.ToString());

            int insertAtIndex = SelectedFrameIndex;
            //var newFrames = SelectedFrames;

            switch (Parameter)
            {
                case FrameNavigation.Previous:
                    insertAtIndex = SelectedFrameIndex;
                    break;
                case FrameNavigation.Next:
                    insertAtIndex = SelectedFrameIndex + 1;
                    break;
            }

            var undoStates = new List<UndoStateViewModel>();

            foreach (var frame in SelectedFrames)
            {
                var newFrame = FrameViewModel.DuplicateFrame(frame, insertAtIndex);

                newFrame.LayerViewModel.AddFrameAtIndex(newFrame, insertAtIndex);
                FrameCount = Math.Max(newFrame.LayerViewModel.Frames.Count, FrameCount);

                newFrame.LayerViewModel.SelectedFrameIndex = insertAtIndex;

                //undoStates.Add(newFrame.SaveState() as FrameState);
                //undoStates.Add(newFrame.LayerViewModel.SaveState() as LayerState);
            }

            if (insertAtIndex <= SelectedFrameIndex)
                SelectedFrameIndex++;

            SelectedFrameIndex = insertAtIndex;

            PushUndoRecord(CreateUndoState("Duplicate Frame", undoStates));
        }
        #endregion DuplicateCurrentFrame Command

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
            else
            {
                SelectedFrameIndex = Math.Max(SelectedFrameIndex - 1, 0);
            }


            PushUndoRecord(CreateUndoState("Delete Frame"/*, undoStates*/));

            //PushUndoRecord(multiState);
        }
        #endregion DeleteCurrentFrame Command

        public TimelineViewModel(List<LayerModel> layers, WorkspaceViewModel workspace)
        {
            WorkspaceViewModel = workspace;

            InitializeCommands();

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
            InitializeCommands();
        }

        public TimelineViewModel(TimelineViewModel originalTimeline)
        {
            InitializeCommands();
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

        public void InitializeCommands()
        {
            AddBlankFrame = new DelegateCommand("Add Blank Frame", AddBlankFrame_CanExecute, AddBlankFrame_Execute);
            AddBlankLayer = new DelegateCommand("Add Blank Layer", AddBlankLayer_CanExecute, AddBlankLayer_Execute);
            PlayAnimation = new DelegateCommand("Play Animation", PlayAnimation_CanExecute, PlayAnimation_Execute);
            StopAnimation = new DelegateCommand("Stop Animation", StopAnimation_CanExecute, StopAnimation_Execute);
            DuplicateCurrentFrame = new DelegateCommand("Duplicate Frame", DuplicateCurrentFrame_CanExecute, DuplicateCurrentFrame_Execute);
            DeleteCurrentFrame = new DelegateCommand("Deleted a Frame", DeleteCurrentFrame_CanExecute, DeleteCurrentFrame_Execute);

        }

        public void InitializeTimeline()
        {
            var newLayer = new LayerViewModel(this, 0);
            var newFrame = new FrameViewModel(newLayer, 0);
            newLayer.AddFrameAtIndex(newFrame, 0);
            AddLayerAtIndex(newLayer, 0);
            FrameCount = GetFrameCountOfLongestLayer();
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

                //undoStates.Add(newFrame.SaveState() as FrameState);
            }
            newLayer.SelectedFrameIndex = SelectedFrameIndex;

            AddLayerAtIndex(newLayer, newLayer.LayerId);

            //undoStates.Add(newLayer.SaveState() as LayerState);
            PushUndoRecord(CreateUndoState("Added Layer"/*, undoStates*/));
        }

        public void AddLayerAtIndex(LayerViewModel layer, int index, bool createUndoState = true)
        {
            if (String.IsNullOrWhiteSpace(layer.DisplayName))
            {
                layer.DisplayName = FileUtilities.GetUniqueNameForCollection(Layers.Select(e => e.DisplayName).ToList(), $"Layer {Layers?.Count ?? 0}");
            }

            //ForceUniqueLayerName(layer);

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
            set { _FrameWidth = value; NotifyPropertiesChanged(nameof(FrameWidth), nameof(ScrubberLength)); }
        }


        public int GetFrameCountOfLongestLayer(bool ExcludeHiddenLayers = false)
        {
            List<LayerViewModel> layers = null;
            //int currentMax = Layers.SelectMany(e => e.Frames).Select(f => f.Order).Max();
            //return currentMax;
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

        public StrokeCollection FlattenStrokesForFrameAtIndex(int frameIndex)
        {
            var flattenedStrokes = new StrokeCollection();
            if (FlattenedFrameStrokes.Count > frameIndex)
            {
                foreach (var frame in Layers.SelectMany(e => e.Frames.Where(f => f.Order == frameIndex)))
                {
                    flattenedStrokes.Add(frame.StrokeCollection);
                }
            }
            return flattenedStrokes;
        }

        public void FlattenStrokesForPlayback()
        {
            //Create an appropriate number of stroke collections for each frame to be flattened
            FlattenedFrameStrokes = new List<StrokeCollection>();
            //FlattenedFrameStrokes.ForEach(e => e = new StrokeCollection());

            //Iterate over all possible frame indexes in all layers up to the highest indexed frame in any layer
            for (int i = 0; i < FrameCount; i++)
            {
                FlattenedFrameStrokes.Add(new StrokeCollection());
                //Iterate over all visible layers
                foreach (var layer in GetVisibleLayers())
                {
                    //If the current layer contains a frame with the index currently being evaluated...
                    if (layer.Frames.Select(e => e.Order).Contains(i))
                    {
                        //Add the strokes from the frame at the current index from the current layer to the
                        //flattened stroke collection
                        FlattenedFrameStrokes[i].Add(layer.Frames[i].StrokeCollection);
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

        public TimelineState CreateUndoState(string title, List<UndoStateViewModel> additionalStates = null)
        {
            return new TimelineState(this, title);
            //var state = SaveState() as TimelineState;

            //if (additionalStates != null)
            //{
            //    var allStates = new List<UndoStateViewModel>();
            //    allStates.Add(state);
            //    allStates.AddRange(additionalStates);

            //    return new MultiState(null, title, allStates);
            //}
            //else
            //{
            //    return new MultiState(null, title, state);
            //}
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
            //ActiveLayer = null;
            //Layers = new ObservableCollection<LayerViewModel>();

            //foreach (var layer in Memento.Layers)
            //{
            //    Layers.Add(layer.Clone());
            //}

            //FrameCount = GetFrameCountOfLongestLayer();

            //ActiveLayer = Layers[Memento.Layers.IndexOf(Memento.ActiveLayer)];
            //SelectedFrameIndex = Memento.SelectedFrameIndex;
            CopyToTimeline(Memento.Timeline, this);
        }


    }
}
