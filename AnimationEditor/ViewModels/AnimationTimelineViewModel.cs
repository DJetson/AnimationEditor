﻿using AnimationEditor.BaseClasses;
using AnimationEditor.Interfaces;
using AnimationEditor.Utilities;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Threading;

namespace AnimationEditor.ViewModels
{
    public class AnimationTimelineViewModel : ViewModelBase
    {
        private ObservableCollection<FrameViewModel> _Frames;
        public ObservableCollection<FrameViewModel> Frames
        {
            get => _Frames;
            set
            {
                _Frames = value;
                NotifyPropertyChanged();

                SelectedFrame = Frames.FirstOrDefault();
            }
        }

        private FrameViewModel _SelectedFrame;
        public FrameViewModel SelectedFrame
        {
            get => _SelectedFrame;
            set
            {
                _SelectedFrame = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged(nameof(NextFrame));
                NotifyPropertyChanged(nameof(PreviousFrame));
                NotifyPropertyChanged(nameof(NextFrameStrokes));
                NotifyPropertyChanged(nameof(PreviousFrameStrokes));
            }
        }

        private double _FramesPerSecond = 24;
        public double FramesPerSecond
        {
            get { return _FramesPerSecond; }
            set { _FramesPerSecond = value; NotifyPropertyChanged(); }
        }

        private DelegateCommand _AppendFrame;
        public DelegateCommand AppendFrame
        {
            get => _AppendFrame;
            set { _AppendFrame = value; NotifyPropertyChanged(); }
        }

        public void AppendFrame_Execute(object parameter)
        {
            var newFrame = new FrameViewModel();
            AddFrameAtIndex(newFrame, Frames.Count);
            SelectedFrame = newFrame;
        }

        public bool AppendFrame_CanExecute(object parameter)
        {
            return true;
        }

        private DelegateCommand _PlayAnimation;
        public DelegateCommand PlayAnimation
        {
            get { return _PlayAnimation; }
            set { _PlayAnimation = value; NotifyPropertyChanged(); }
        }

        private DelegateCommand _StopAnimation;
        public DelegateCommand StopAnimation
        {
            get { return _StopAnimation; }
            set { _StopAnimation = value; NotifyPropertyChanged(); }
        }

        private bool _IsInPlaybackMode = false;
        public bool IsInPlaybackMode
        {
            get { return _IsInPlaybackMode; }
            set { _IsInPlaybackMode = value; NotifyPropertyChanged(); }
        }

        private AnimationPlaybackState _PlaybackState;

        public void PlayAnimation_Execute(object parameter)
        {
            //TODO: Turn off onionskins. This will require a minor refactor so that the binding used for onionskin
            //      visibility is an AND between some flag that gets raised by this method, and the actual visibility
            //      setting. So that it turns off when the animation is playing and turns back on when it stops.

            //NOTE: I may have to eventually do some caching up top here. Rasterize all of the InkCanvases in the series
            //      and just flip through those instead of directly showing the InkCanvases. It will depend entirely on
            //      how well it performs in it's initial, most basic form.

            if (IsInPlaybackMode == false)
            {
                _PlaybackState = new AnimationPlaybackState(this);
            }
            else
            {
                _PlaybackState.RequestPlaybackState(this, AnimationPlaybackState.PlaybackStates.Pause);
                IsInPlaybackMode = false;
            }

            //Timer t = new Timer(AnimationTimerIntervalElapsed,null,0, (int)(1000 * (1.0/FramesPerSecond)));

        }
        public void StopAnimation_Execute(object parameter)
        {
            _PlaybackState.RequestPlaybackState(this, AnimationPlaybackState.PlaybackStates.Stop);

            if (_PlaybackState.PlaybackState == AnimationPlaybackState.PlaybackStates.Stop)
                _PlaybackState = null;

            IsInPlaybackMode = false;
        }

        //private void PauseAnimationPlayback()
        //{
        //    throw new NotImplementedException();
        //}

        //private void BeginAnimationPlayback()
        //{
        //    DispatcherTimer timer = new DispatcherTimer();
        //    timer.Interval = new TimeSpan((int)(1000 * (1.0 / FramesPerSecond)));

        //    int selectedFrameIndex = Frames.IndexOf(SelectedFrame);

        //    timer.Tick += (sender, e) =>
        //    {
        //        int nextFrameIndex = (selectedFrameIndex + 1) % Frames.Count;

        //        SelectedFrame = Frames[nextFrameIndex];
        //    };

        //    IsInPlaybackMode = true;
        //    timer.Start();
        //}

        //private void AnimationTimerIntervalElapsed(object state)
        //{
        //}

        public bool PlayAnimation_CanExecute(object parameter)
        {
            return true;
        }

        public bool StopAnimation_CanExecute(object parameter)
        {
            if(_PlaybackState == null)
                return false;
            if (_PlaybackState.PlaybackState != AnimationPlaybackState.PlaybackStates.Play)
                return false;

            return true;
        }

        //private ObservableCollection<FrameViewModel> _NextFrames = new ObservableCollection<FrameViewModel>();
        //public ObservableCollection<FrameViewModel> NextFrames
        //{
        //    get => _NextFrames;
        //    set { _NextFrames = value; NotifyPropertyChanged(); }
        //}

        //private FrameViewModel _NextFrame;
        public StrokeCollection NextFrameStrokes
        {
            get
            {
                if (NextFrame == null)
                    return null;
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
                    return null;
                var strokes = PreviousFrame.StrokeCollection.Clone();
                foreach (var item in strokes)
                {
                    item.DrawingAttributes.IsHighlighter = true;
                    item.DrawingAttributes.Color = Color.FromArgb(128, 255, 0, 0);
                }
                return strokes;
            }
        }

        public FrameViewModel NextFrame
        {
            get => Frames.ElementAtOrDefault(Frames.IndexOf(SelectedFrame) + 1);
        }

        public FrameViewModel PreviousFrame
        {
            get => Frames.ElementAtOrDefault(Frames.IndexOf(SelectedFrame) - 1);
        }

        public void InitializeCommands()
        {
            AppendFrame = new DelegateCommand(AppendFrame_CanExecute, AppendFrame_Execute);
            PlayAnimation = new DelegateCommand(PlayAnimation_CanExecute, PlayAnimation_Execute);
            StopAnimation = new DelegateCommand(StopAnimation_CanExecute, StopAnimation_Execute);
        }

        public AnimationTimelineViewModel(List<IFrameModel> frames)
        {
            InitializeCommands();

            Frames = new ObservableCollection<FrameViewModel>();
            foreach (var item in frames)
            {
                Frames.Add(new FrameViewModel(item));
            }

            SelectedFrame = Frames.FirstOrDefault();
        }

        public AnimationTimelineViewModel()
        {
            InitializeCommands();

            Frames = new ObservableCollection<FrameViewModel>();
            Frames.Add(new FrameViewModel());
            SelectedFrame = Frames.FirstOrDefault();

        }

        public void AddFrameAtIndex(FrameViewModel frame, int index)
        {
            if (index < Frames.Count)
            {
                Frames.Insert(index, frame);
            }
            else if (index == Frames.Count)
            {
                Frames.Add(frame);
            }
            else
            {
                Console.WriteLine($"WorkspaceViewModel.InsertFrame ERROR: Attempted to insert a frame at an invalid index = {index}");
            }
        }
    }
}
