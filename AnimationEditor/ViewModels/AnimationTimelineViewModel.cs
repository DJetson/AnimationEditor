﻿using AnimationEditor.BaseClasses;
using AnimationEditor.Interfaces;
using AnimationEditor.ViewModels.StateObjects;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Ink;
using System.Windows.Media;

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

                if (_SelectedFrame != null && value != null)
                {
                    var state = SaveState() as AnimationTimelineState;
                    state.DisplayName = "Navigate to Frame";
                    PushUndoRecord(state);
                    //UndoStateViewModel<AnimationTimelineState> state = SaveState() as UndoStateViewModel<AnimationTimelineState>;
                    //state.DisplayName = "Navigate to Frame";
                    //MainWindowViewModel.WorkspaceManager.AddHistoricalState(state);
                }

                //_HasUnrecordedFrameSelectionChange = true;
            }
        }

        public void SelectFrameWithoutUndoBuffer(FrameViewModel frame)
        {
            _SelectedFrame = frame;
            NotifySelectedFrameAndDependents();
        }

        public void PushUndoRecord(AnimationTimelineState nextState)
        {
            //var mainWindowViewModel = App.Current.MainWindow.DataContext as MainWindowViewModel;
            //if (mainWindowViewModel != null)
            //{ 
            //if (CurrentState != null)
            //{
            //    WorkspaceViewModel.WorkspaceHistoryViewModel.AddHistoricalState(CurrentState);
            //    WorkspaceViewModel.WorkspaceHistoryViewModel.SelectedCurrentState = nextState;
            //}
            //}
            WorkspaceViewModel.WorkspaceHistoryViewModel.AddHistoricalState(nextState);
            //WorkspaceViewModel.WorkspaceHistoryViewModel.CurrentState = nextState;
            //CurrentState = nextState;
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
                    item.DrawingAttributes.Color = Color.FromArgb(128, 0, 255, 0);
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
                    item.DrawingAttributes.Color = Color.FromArgb(128, 255, 0, 0);
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

            //NOTE: I may have to eventually do some caching up top here. Rasterize all of the InkCanvases in the series
            //      and just flip through those instead of directly showing the InkCanvases. It will depend entirely on
            //      how well it performs in it's initial, most basic form.

            switch (AnimationPlaybackViewModel.CurrentState)
            {
                case PlaybackStates.Pause:
                    AnimationPlaybackViewModel.ResumePlayback();
                    break;
                case PlaybackStates.Play:
                    AnimationPlaybackViewModel.PausePlayback();
                    break;
                case PlaybackStates.Stop:
                    AnimationPlaybackViewModel.StartPlayback(playbackFrames, FramesPerSecond);
                    break;
            }
        }

        public bool PlayAnimation_CanExecute(object parameter)
        {
            //TODO: Eventually I need to add the conditions to return false If we're currently 
            //in a "modal" state with the ink canvas for any reason, return false. We don't 
            //need an active Scale transform going all screwy because the user accidentally hits 
            //the play button before they resolve the resize, causing the playback object to yank 
            //the target frame right out from under them

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
            switch (AnimationPlaybackViewModel.CurrentState)
            {
                case PlaybackStates.Pause:
                    AnimationPlaybackViewModel.StopPlayback();
                    break;
                case PlaybackStates.Play:
                    AnimationPlaybackViewModel.StopPlayback();
                    break;
            }
        }

        public bool StopAnimation_CanExecute(object parameter)
        {
            if (AnimationPlaybackViewModel.CurrentState == PlaybackStates.Stop)
                return false;

            return true;
        }
        #endregion StopAnimation Command

        #region NavigateToFrame Command
        private DelegateCommand _NavigateToFrame;
        public DelegateCommand NavigateToFrame
        {
            get { return _NavigateToFrame; }
            set { _NavigateToFrame = value; NotifyPropertyChanged(); }
        }

        public void NavigateToFrame_Execute(object parameter)
        {
            var Parameter = (FrameNavigation)Enum.Parse(typeof(FrameNavigation), parameter.ToString());

            int selectedFrameIndex = Frames.IndexOf(SelectedFrame);
            int navigateToFrameIndex = selectedFrameIndex;

            switch (Parameter)
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

            SelectedFrame = Frames[navigateToFrameIndex];
        }

        public bool NavigateToFrame_CanExecute(object parameter)
        {
            if (AnimationPlaybackViewModel.CurrentState == PlaybackStates.Play)
                return false;

            if (Enum.TryParse<FrameNavigation>(parameter.ToString(), out FrameNavigation Parameter) == false)
                return false;

            if (Parameter == FrameNavigation.Current)
                return false;

            return true;
        }
        #endregion NavigateToFrame Command

        #region AddBlankFrame Command
        private DelegateCommand _AddBlankFrame;
        public DelegateCommand AddBlankFrame
        {
            get => _AddBlankFrame;
            set { _AddBlankFrame = value; NotifyPropertyChanged(); }
        }

        public bool AddBlankFrame_CanExecute(object parameter)
        {
            if (AnimationPlaybackViewModel.CurrentState == PlaybackStates.Play)
                return false;

            if (AnimationPlaybackViewModel.CurrentState == PlaybackStates.Pause)
                return false;

            if (Enum.TryParse<FrameNavigation>(parameter.ToString(), out FrameNavigation Parameter) == false)
                return false;

            if (Parameter == FrameNavigation.Current)
                return false;

            return true;
        }

        public void AddBlankFrame_Execute(object parameter)
        {
            //var state = SaveState() as UndoStateViewModel<AnimationTimelineState>;
            //state.DisplayName = AddBlankFrame.DisplayName;
            //MainWindowViewModel.WorkspaceManager.AddHistoricalState(state);

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
            SelectFrameWithoutUndoBuffer(newFrame);

            var state = SaveState() as AnimationTimelineState;
            state.DisplayName = AddBlankFrame.DisplayName;
            PushUndoRecord(state);
        }

        //private void UnbufferedFrameChange(FrameViewModel newFrame)
        //{
        //    _SelectedFrame = newFrame;
        //    NotifySelectedFrameAndDependents();
        //}

        private void NotifySelectedFrameAndDependents()
        {
            NotifyPropertyChanged(nameof(SelectedFrame));
            NotifyPropertyChanged(nameof(NextFrame));
            NotifyPropertyChanged(nameof(PreviousFrame));
            NotifyPropertyChanged(nameof(NextFrameStrokes));
            NotifyPropertyChanged(nameof(PreviousFrameStrokes));
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
            if (AnimationPlaybackViewModel.CurrentState == PlaybackStates.Play)
                return false;

            if (AnimationPlaybackViewModel.CurrentState == PlaybackStates.Pause)
                return false;

            if (Enum.TryParse<FrameNavigation>(parameter.ToString(), out FrameNavigation Parameter) == false)
                return false;

            if (Parameter == FrameNavigation.Current)
                return false;

            return true;
        }

        public void DuplicateCurrentFrame_Execute(object parameter)
        {
            //var state = SaveState() as UndoStateViewModel<AnimationTimelineState>;
            //state.DisplayName = DuplicateCurrentFrame.DisplayName;
            //MainWindowViewModel.WorkspaceManager.AddHistoricalState(state);

            var Parameter = (FrameNavigation)Enum.Parse(typeof(FrameNavigation), parameter.ToString());

            int selectedFrameIndex = Frames.IndexOf(SelectedFrame);
            int DuplicateCurrentFrameToIndex = selectedFrameIndex;

            var newFrame = new FrameViewModel(SelectedFrame.WorkspaceViewModel, SelectedFrame.StrokeCollection);
            //{
            //    StrokeCollection = SelectedFrame.StrokeCollection.Clone(),
            //    //Strokes = new ObservableCollection<Stroke>(SelectedFrame.Strokes),
            //};

            switch (Parameter)
            {
                //case FrameNavigation.Start:
                //    DuplicateCurrentFrameToIndex = 0;
                //    break;
                case FrameNavigation.Previous:
                    DuplicateCurrentFrameToIndex = selectedFrameIndex;
                    break;
                case FrameNavigation.Next:
                    DuplicateCurrentFrameToIndex = selectedFrameIndex + 1;
                    break;
                    //case FrameNavigation.End:
                    //    DuplicateCurrentFrameToIndex = Frames.Count - 1;
                    //    break;
            }

            AddFrameAtIndex(newFrame, DuplicateCurrentFrameToIndex);
            SelectFrameWithoutUndoBuffer(Frames[DuplicateCurrentFrameToIndex]);

            var state = SaveState() as AnimationTimelineState;
            state.DisplayName = DuplicateCurrentFrame.DisplayName;
            PushUndoRecord(state);
        }
        #endregion DuplicateCurrentFrame Command

        #region DeleteCurrentFrame Command
        private DelegateCommand _DeleteCurrentFrame;
        public DelegateCommand DeleteCurrentFrame
        {
            get { return _DeleteCurrentFrame; }
            set { _DeleteCurrentFrame = value; NotifyPropertyChanged(); }
        }

        //private AnimationTimelineState _CurrentState;
        //public IMemento CurrentState
        //{
        //    get => _CurrentState;
        //    set { _CurrentState = value as AnimationTimelineState; NotifyPropertyChanged(); }
        //}

        public bool DeleteCurrentFrame_CanExecute(object parameter)
        {
            if (AnimationPlaybackViewModel.CurrentState == PlaybackStates.Play)
                return false;

            if (AnimationPlaybackViewModel.CurrentState == PlaybackStates.Pause)
                return false;

            if (SelectedFrame == null)
                return false;

            return true;
        }

        public void DeleteCurrentFrame_Execute(object parameter)
        {
            int selectedFrameIndex = Frames.IndexOf(SelectedFrame);

            Frames.Remove(SelectedFrame);

            if (Frames.Count == 0)
            {
                var newFrame = new FrameViewModel(WorkspaceViewModel);
                AddFrameAtIndex(newFrame, 0);
                SelectedFrame = newFrame;
                return;
            }

            if (selectedFrameIndex >= Frames.Count)
                selectedFrameIndex = Frames.Count - 1;

            //SelectedFrame = Frames[selectedFrameIndex];
            SelectFrameWithoutUndoBuffer(Frames[selectedFrameIndex]);

            var state = SaveState() as AnimationTimelineState;
            state.DisplayName = DeleteCurrentFrame.DisplayName;
            PushUndoRecord(state);
        }
        #endregion DeleteCurrentFrame Command

        public void InitializeCommands()
        {
            PlayAnimation = new DelegateCommand(PlayAnimation_CanExecute, PlayAnimation_Execute);
            StopAnimation = new DelegateCommand(StopAnimation_CanExecute, StopAnimation_Execute);

            DeleteCurrentFrame = new DelegateCommand("Delete Frame", DeleteCurrentFrame_CanExecute, DeleteCurrentFrame_Execute);
            AddBlankFrame = new DelegateCommand("Add Blank Frame", AddBlankFrame_CanExecute, AddBlankFrame_Execute);
            NavigateToFrame = new DelegateCommand(NavigateToFrame_CanExecute, NavigateToFrame_Execute);
            DuplicateCurrentFrame = new DelegateCommand("Duplicate Frame", DuplicateCurrentFrame_CanExecute, DuplicateCurrentFrame_Execute);
        }

        public AnimationTimelineViewModel(List<Models.FrameModel> frames)
        {
            InitializeCommands();

            Frames = new ObservableCollection<FrameViewModel>();
            foreach (var item in frames)
            {
                Frames.Add(new FrameViewModel(item));
            }

            SelectFrameWithoutUndoBuffer(Frames.FirstOrDefault());

            var state = SaveState() as AnimationTimelineState;
            state.DisplayName = "Created AnimationTimeline from Frame Models";
            PushUndoRecord(state);
        }

        public AnimationTimelineViewModel(WorkspaceViewModel workspace)
        {
            WorkspaceViewModel = workspace;

            InitializeCommands();

            Frames = new ObservableCollection<FrameViewModel>();
            Frames.Add(new FrameViewModel(workspace));
            SelectFrameWithoutUndoBuffer(Frames.FirstOrDefault());

            var state = SaveState() as AnimationTimelineState;
            state.DisplayName = "Created AnimationTimeline";
            PushUndoRecord(state);
        }

        public void AddFrameAtIndex(FrameViewModel frame, int index)
        {

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
        }

        public IMemento SaveState()
        {
            var memento = new AnimationTimelineState(this);

            memento.Originator = this;
            //memento.State = new AnimationTimelineState(this);

            return memento;
        }

        public void LoadState(IMemento state)
        {
            var Memento = (state as AnimationTimelineState);

            Frames = Memento.Frames;
            FramesPerSecond = Memento.FramesPerSecond;
            SelectFrameWithoutUndoBuffer(Memento.SelectedFrame);

            //CurrentState = Memento;
        }
    }
}
