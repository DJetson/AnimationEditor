using AnimationEditorCore.BaseClasses;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Ink;
using System.Windows.Threading;

namespace AnimationEditorCore.ViewModels
{
    public class AnimationPlaybackViewModel : ViewModelBase
    {
        private ObservableCollection<StrokeCollection> _Frames = null;
        public ObservableCollection<StrokeCollection> Frames
        {
            get { return _Frames; }
            set { _Frames = value; NotifyPropertyChanged(); }
        }

        private double _AnimationFps;
        public double AnimationFps
        {
            get => _AnimationFps;
            set { _AnimationFps = value; NotifyPropertyChanged(nameof(AnimationFps), nameof(PlaybackFps)); }
        }
        private int _OriginalIndex;
        private DispatcherTimer _PlaybackTimer;

        private StrokeCollection _CurrentFrame;
        public StrokeCollection CurrentFrame
        {
            get { return _CurrentFrame; }
            set { _CurrentFrame = value; NotifyPropertyChanged(); }
        }


        private PlaybackSpeed _SelectedPlaybackSpeed = PlaybackSpeed.Normal;
        public PlaybackSpeed SelectedPlaybackSpeed
        {
            get { return _SelectedPlaybackSpeed; }
            set { _SelectedPlaybackSpeed = value; NotifyPropertyChanged(); }
        }

        private bool _IsPlaybackActive = false;
        public bool IsPlaybackActive
        {
            get { return _IsPlaybackActive; }
            set { _IsPlaybackActive = value; NotifyPropertyChanged(); }
        }

        public double PlaybackFps
        {
            get { return _AnimationFps * PlaybackFpsMultiplier; }
        }

        private double _PlaybackFpsMultiplier = 1.0f;
        public double PlaybackFpsMultiplier
        {
            get { return _PlaybackFpsMultiplier; }
            set { _PlaybackFpsMultiplier = value; NotifyPropertyChanged(nameof(PlaybackFpsMultiplier), nameof(PlaybackFps)); }
        }

        private TimelineViewModel _TimelineViewModel;
        public TimelineViewModel TimelineViewModel
        {
            get { return _TimelineViewModel; }
            set { _TimelineViewModel = value; NotifyPropertyChanged(); }
        }

        public AnimationPlaybackViewModel(TimelineViewModel timeline)
        {
            TimelineViewModel = timeline;
            AnimationFps = timeline.FramesPerSecond;
            _PlaybackTimer = new DispatcherTimer(DispatcherPriority.Render);
            _PlaybackTimer.Tick += DispatcherTimer_Elapsed;
        }

        public void SetPlaybackSpeed(PlaybackSpeed playbackSpeed)
        {
            SelectedPlaybackSpeed = playbackSpeed;

            switch (SelectedPlaybackSpeed)
            {
                case PlaybackSpeed.Half:
                    PlaybackFpsMultiplier = 0.5f;
                    break;
                case PlaybackSpeed.Normal:
                    PlaybackFpsMultiplier = 1.0f;
                    break;
                case PlaybackSpeed.Double:
                    PlaybackFpsMultiplier = 2.0f;
                    break;
            }

            UpdatePlaybackTimer();
        }

        public void UpdatePlaybackTimer()
        {
            if (IsPlaybackActive == true)
            {
                _PlaybackTimer.Stop();
                _PlaybackTimer.Interval = new TimeSpan((int)(TimeSpan.TicksPerSecond * (1.0f / (PlaybackFps))));
                _PlaybackTimer.Start();
            }
        }

        public void StartPlayback(List<StrokeCollection> playbackFrames, double animationFps, int startFromIndex = 0)
        {
            if (IsPlaybackActive)
            {
                Debug.WriteLine("Playback has already Started. This, and any further calls to StartPlayback() on this instance are unnecessary and will have no effect while it remains in the Play state.");
                return;
            }
            else
            {
                if (playbackFrames == null || playbackFrames.Count < 2)
                    return;

                if (startFromIndex < 0 || startFromIndex >= playbackFrames.Count)
                    startFromIndex = 0;

                _Frames = new ObservableCollection<StrokeCollection>(playbackFrames);
                _OriginalIndex = startFromIndex;
                CurrentFrame = _Frames[_OriginalIndex];

                _AnimationFps = animationFps;

                _PlaybackTimer.Interval = new TimeSpan((int)(TimeSpan.TicksPerSecond * (1.0 / (PlaybackFps))));

                IsPlaybackActive = true;
                _PlaybackTimer.Start();
            }
        }

        public void StopPlayback(bool resetToOriginalIndex = true)
        {
            if (!IsPlaybackActive)
            {
                Console.WriteLine("Playback has already been Stopped. This, and any further calls to StopPlayback() on this instance are unnecessary and will have no effect while it remains in the Stop state.");
            }
            else if (IsPlaybackActive)
            {
                _PlaybackTimer.Stop();
            }

            if (resetToOriginalIndex)
            {
                CurrentFrameIndex = _OriginalIndex;
            }

            CurrentFrame = _Frames[CurrentFrameIndex];
            _OriginalIndex = 0;
            _Frames.Clear();
            IsPlaybackActive = false;
        }

        private int _CurrentFrameIndex;
        public int CurrentFrameIndex
        {
            get { return _CurrentFrameIndex; }
            set { _CurrentFrameIndex = value; NotifyPropertyChanged(nameof(CurrentFrameIndex), nameof(CurrentIndexOutOfFrameCount)); }
        }

        public string CurrentIndexOutOfFrameCount
        {
            get => $"{CurrentFrameIndex}/{(Frames?.Count ?? 0) - 1}";
        }

        private void DispatcherTimer_Elapsed(object sender, EventArgs e)
        {
            var lastFrame = CurrentFrame;
            CurrentFrameIndex = (CurrentFrameIndex + 1) % _Frames.Count;
            CurrentFrame = _Frames[CurrentFrameIndex];
        }
    }
}
