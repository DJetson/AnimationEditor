﻿using AnimationEditor.BaseClasses;
using AnimationEditor.Interfaces;
using AnimationEditor.ViewModels.StateObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Image = System.Windows.Controls.Image;

namespace AnimationEditor.ViewModels
{
    public enum FrameNavigation { Start, Previous, Current, Next, End };

    public class AnimationTimelineViewModel : ViewModelBase, IMementoOriginator
    {
        private WorkspaceViewModel _WorkspaceViewModel;
        public WorkspaceViewModel WorkspaceViewModel
        {
            get { return _WorkspaceViewModel; }
            set { _WorkspaceViewModel = value; NotifyPropertyChanged(); }
        }

        private AnimationPlaybackViewModel _AnimationPlaybackViewModel = new AnimationPlaybackViewModel();
        public AnimationPlaybackViewModel AnimationPlaybackViewModel
        {
            get { return _AnimationPlaybackViewModel; }
            set { _AnimationPlaybackViewModel = value; NotifyPropertyChanged(); }
        }

        private ObservableCollection<FrameViewModel> _Frames;
        public ObservableCollection<FrameViewModel> Frames
        {
            get => _Frames;
            set { _Frames = value; NotifyPropertyChanged(); SelectFrameWithoutUndoBuffer(Frames.FirstOrDefault()); }
        }

        private FrameViewModel _SelectedFrame;
        public FrameViewModel SelectedFrame
        {
            get => _SelectedFrame;
            set
            {
                _SelectedFrame = value;
                NotifySelectedFrameAndDependents();
            }
        }

        public void SelectFrameWithoutUndoBuffer(FrameViewModel frame)
        {
            _SelectedFrame = frame;
            NotifySelectedFrameAndDependents();
        }

        public void PushUndoRecord(UndoStateViewModel nextState, bool raiseChangedFlag = true)
        {
            WorkspaceViewModel.WorkspaceHistoryViewModel.AddHistoricalState(nextState, raiseChangedFlag);
        }

        private double _FramesPerSecond = 24;
        public double FramesPerSecond
        {
            get { return _FramesPerSecond; }
            set { _FramesPerSecond = value; NotifyPropertyChanged(); }
        }

        public StrokeCollection NextFrameStrokes
        {
            get
            {
                if (NextFrame == null)
                    return new StrokeCollection();

                var strokes = NextFrame.StrokeCollection.Clone();
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

                var strokes = PreviousFrame.StrokeCollection.Clone();
                foreach (var item in strokes)
                {
                    item.DrawingAttributes.IsHighlighter = true;
                    item.DrawingAttributes.Color = System.Windows.Media.Color.FromArgb(128, 255, 0, 0);
                }
                return strokes;
            }
        }

        public FrameViewModel NextFrame => Frames.ElementAtOrDefault(Frames.IndexOf(SelectedFrame) + 1);
        public FrameViewModel PreviousFrame => Frames.ElementAtOrDefault(Frames.IndexOf(SelectedFrame) - 1);

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
            var playbackFrames = Frames.ToList();
            playbackFrames.ForEach(e => e.FlattenStrokesForPlayback());

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

        #region AddBlankFrame Command
        private DelegateCommand _AddBlankFrame;
        public DelegateCommand AddBlankFrame
        {
            get => _AddBlankFrame;
            set { _AddBlankFrame = value; NotifyPropertyChanged(); }
        }

        public bool AddBlankFrame_CanExecute(object parameter)
        {
            if (AnimationPlaybackViewModel.IsPlaybackActive)
                return false;

            if (Enum.TryParse<FrameNavigation>(parameter.ToString(), out FrameNavigation Parameter) == false)
                return false;

            if (Parameter == FrameNavigation.Current)
                return false;

            return true;
        }

        public void AddBlankFrame_Execute(object parameter)
        {
            var newFrame = new FrameViewModel(WorkspaceViewModel);

            int insertAtIndex = Frames.Count;
            int selectedFrameIndex = Frames.IndexOf(SelectedFrame);
            var Parameter = (FrameNavigation)Enum.Parse(typeof(FrameNavigation), parameter.ToString());
            switch (Parameter)
            {
                case FrameNavigation.Start:
                    insertAtIndex = 0;
                    break;
                case FrameNavigation.Previous:
                    insertAtIndex = selectedFrameIndex;
                    break;
                case FrameNavigation.Next:
                    insertAtIndex = selectedFrameIndex + 1;
                    break;
                case FrameNavigation.End:
                    insertAtIndex = Frames.Count;
                    break;
            }

            AddFrameAtIndex(newFrame, insertAtIndex);
            //var firstLayer = new LayerViewModel(newFrame);
            //newFrame.AddNewLayerAtIndex(0, "Layer 0", false);
            var layerState = newFrame.SaveLayerStates();
            var frameState = newFrame.SaveState() as FrameState;
            SelectFrameWithoutUndoBuffer(newFrame);
            var timelineState = SaveState() as AnimationTimelineState;
            var multiState = new MultiState(this, $"Added Frame {newFrame.Order}", layerState, frameState, timelineState);

            PushUndoRecord(multiState);
        }

        #endregion AddBlankFrame Command

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

            if (Parameter == FrameNavigation.Current)
                return false;

            return true;
        }

        public void DuplicateCurrentFrame_Execute(object parameter)
        {
            var Parameter = (FrameNavigation)Enum.Parse(typeof(FrameNavigation), parameter.ToString());

            int selectedFrameIndex = Frames.IndexOf(SelectedFrame);
            int DuplicateCurrentFrameToIndex = selectedFrameIndex;

            var newFrame = SelectedFrame.Clone();

            switch (Parameter)
            {
                case FrameNavigation.Previous:
                    DuplicateCurrentFrameToIndex = selectedFrameIndex;
                    break;
                case FrameNavigation.Next:
                    DuplicateCurrentFrameToIndex = selectedFrameIndex + 1;
                    break;
            }

            AddFrameAtIndex(newFrame, DuplicateCurrentFrameToIndex);
            //var firstLayer = new LayerViewModel(newFrame);
            //newFrame.AddLayer(firstLayer, false);

            var layerState = newFrame.SaveLayerStates();
            var frameState = newFrame.SaveState() as FrameState;
            SelectFrameWithoutUndoBuffer(newFrame);
            var timelineState = SaveState() as AnimationTimelineState;
            var multiState = new MultiState(this, $"Insert Duplicate {(Parameter == FrameNavigation.Previous ? "Before" : "After")} Frame {selectedFrameIndex}", layerState, frameState, timelineState);

            PushUndoRecord(multiState);
        }

        public List<System.Drawing.Image> RenderFrameBitmaps(InkCanvas canvas)
        {
            RenderTargetBitmap rtb = new RenderTargetBitmap((int)canvas.ActualWidth, (int)canvas.ActualHeight, 96, 96, new System.Windows.Media.PixelFormat());
            List<System.Drawing.Image> frameImages = new List<System.Drawing.Image>();
            foreach (var frame in Frames)
            {
                canvas.Strokes = frame.StrokeCollection;
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

            if (SelectedFrame == null)
                return false;

            return true;
        }

        public void RemoveFrame(FrameViewModel frame)
        {
            var framesToUpdate = Frames.Where(e => e.Order > frame.Order);
            foreach (var item in framesToUpdate)
            {
                item.Order -= 1;
            }

            Frames.Remove(frame);

            UpdateFrameOrderIds();
        }

        public void DeleteCurrentFrame_Execute(object parameter)
        {
            int selectedFrameIndex = Frames.IndexOf(SelectedFrame);

            RemoveFrame(SelectedFrame);
            FrameViewModel newFrame = null;
            if (Frames.Count == 0)
            {
                newFrame = new FrameViewModel(WorkspaceViewModel);
                AddFrameAtIndex(newFrame, 0);
                SelectedFrame = newFrame;
                return;
            }

            if (selectedFrameIndex >= Frames.Count)
                selectedFrameIndex = Frames.Count - 1;

            SelectFrameWithoutUndoBuffer(Frames[selectedFrameIndex]);

            var timelineState = SaveState() as AnimationTimelineState;

            MultiState multiState = null;

            if (newFrame != null)
            {
                var frameState = newFrame.SaveState() as FrameState;
                multiState = new MultiState(this, DeleteCurrentFrame.DisplayName, frameState, timelineState);
            }
            else
            {
                multiState = new MultiState(this, DeleteCurrentFrame.DisplayName, timelineState);
            }

            PushUndoRecord(multiState);
        }
        #endregion DeleteCurrentFrame Command

        public void InitializeCommands()
        {
            PlayAnimation = new DelegateCommand(PlayAnimation_CanExecute, PlayAnimation_Execute);
            StopAnimation = new DelegateCommand(StopAnimation_CanExecute, StopAnimation_Execute);

            DeleteCurrentFrame = new DelegateCommand("Delete Frame", DeleteCurrentFrame_CanExecute, DeleteCurrentFrame_Execute);
            AddBlankFrame = new DelegateCommand("Add Blank Frame", AddBlankFrame_CanExecute, AddBlankFrame_Execute);
            //NavigateToFrame = new DelegateCommand(NavigateToFrame_CanExecute, NavigateToFrame_Execute);
            DuplicateCurrentFrame = new DelegateCommand("Duplicate Frame", DuplicateCurrentFrame_CanExecute, DuplicateCurrentFrame_Execute);
        }

        private void NotifySelectedFrameAndDependents()
        {
            NotifyPropertyChanged(nameof(SelectedFrame));
            NotifyPropertyChanged(nameof(NextFrame));
            NotifyPropertyChanged(nameof(PreviousFrame));
            NotifyPropertyChanged(nameof(NextFrameStrokes));
            NotifyPropertyChanged(nameof(PreviousFrameStrokes));
        }

        public AnimationTimelineViewModel(List<Models.FrameModel> frames, WorkspaceViewModel workspace)
        {
            WorkspaceViewModel = workspace;

            InitializeCommands();

            Frames = new ObservableCollection<FrameViewModel>();
            foreach (var item in frames)
            {
                var newFrame = new FrameViewModel(item, workspace);
                newFrame.Order = Frames.Count;
                Frames.Add(newFrame);

            }

            SelectFrameWithoutUndoBuffer(Frames.FirstOrDefault());

            var state = SaveState() as AnimationTimelineState;
            var frameState = SelectedFrame.SaveState() as FrameState;
            var multiState = new MultiState(null, "Opened File", state, frameState);
            WorkspaceViewModel.WorkspaceHistoryViewModel.InitialState = multiState;
            //state.DisplayName = "Opened File";
            PushUndoRecord(multiState, false);
        }

        public AnimationTimelineViewModel(WorkspaceViewModel workspace)
        {
            WorkspaceViewModel = workspace;

            InitializeCommands();

            Frames = new ObservableCollection<FrameViewModel>();
            var firstFrame = new FrameViewModel(workspace, false);
            //var firstLayer = new LayerViewModel(firstFrame);
            firstFrame.AddNewLayerAtIndex(0, "Layer 0", false);
            var firstLayer = firstFrame.Layers.FirstOrDefault();
            Frames.Add(firstFrame);
            SelectFrameWithoutUndoBuffer(Frames.FirstOrDefault());

            var layerState = firstLayer.SaveState() as LayerState;
            var frameState = firstFrame.SaveState() as FrameState;
            var timelineState = SaveState() as AnimationTimelineState;

            var multiState = new MultiState(null, "New Animation", layerState, frameState, timelineState);
            WorkspaceViewModel.WorkspaceHistoryViewModel.InitialState = multiState;

            PushUndoRecord(multiState, false);
        }

        public void NavigateToFrame(FrameNavigation navigation)
        {
            int selectedFrameIndex = Frames.IndexOf(SelectedFrame);
            int navigateToFrameIndex = selectedFrameIndex;

            switch (navigation)
            {
                case FrameNavigation.Start:
                    navigateToFrameIndex = 0;
                    break;
                case FrameNavigation.Previous:
                    navigateToFrameIndex = Math.Max(selectedFrameIndex - 1, 0);
                    break;
                case FrameNavigation.Next:
                    navigateToFrameIndex = Math.Min(selectedFrameIndex + 1, Frames.Count - 1);
                    break;
                case FrameNavigation.End:
                    navigateToFrameIndex = Frames.Count - 1;
                    break;
            }

            SelectFrameWithoutUndoBuffer(Frames[navigateToFrameIndex]);
        }

        public void UpdateFrameOrderIds()
        {
            foreach(var frame in Frames)
            {
                frame.Order = Frames.IndexOf(frame);
            }
        }

        public void AddFrameAtIndex(FrameViewModel frame, int index)
        {
            frame.Order = index;
            if (index < Frames.Count)
            {
                Frames.Insert(index, frame);
            }
            else if (index >= Frames.Count)
            {
                Frames.Add(frame);
            }
            else
            {
                Console.WriteLine($"WorkspaceViewModel.InsertFrame ERROR: Attempted to insert a frame at an invalid index = {index}");
            }

            UpdateFrameOrderIds();
        }

        public IMemento SaveState()
        {
            var memento = new AnimationTimelineState(this);

            memento.Originator = this;

            return memento;
        }

        public void LoadState(IMemento state)
        {
            var Memento = (state as AnimationTimelineState);

            Frames = new ObservableCollection<FrameViewModel>(Memento.Frames);
            FramesPerSecond = Memento.FramesPerSecond;
            SelectFrameWithoutUndoBuffer(Memento.SelectedFrame);
        }
    }
}
