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

        private double _AnimationFps = 24;
        private int _OriginalIndex;
        private DispatcherTimer _PlaybackTimer;

        private StrokeCollection _CurrentFrame;
        public StrokeCollection CurrentFrame
        {
            get { return _CurrentFrame; }
            set { _CurrentFrame = value; NotifyPropertyChanged(); }
        }

        private bool _IsPlaybackActive = false;
        public bool IsPlaybackActive
        {
            get { return _IsPlaybackActive; }
            set { _IsPlaybackActive = value; NotifyPropertyChanged(); }
        }

        private double _PlaybackFps;
        public double PlaybackFps
        {
            get { return _PlaybackFps; }
            set { _PlaybackFps = value; NotifyPropertyChanged(); }
        }

        private double _PlaybackFpsMultiplier = 1.0f;
        public double PlaybackFpsMultiplier
        {
            get { return _PlaybackFpsMultiplier; }
            set { _PlaybackFpsMultiplier = value; NotifyPropertyChanged(); }
        }

        public AnimationPlaybackViewModel()
        {
            _PlaybackTimer = new DispatcherTimer(DispatcherPriority.Render);
            _PlaybackTimer.Tick += DispatcherTimer_Elapsed;
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
                if (startFromIndex < 0 || startFromIndex >= playbackFrames.Count)
                    startFromIndex = 0;

                _Frames = new ObservableCollection<StrokeCollection>(playbackFrames);
                _OriginalIndex = startFromIndex;
                CurrentFrame = _Frames[_OriginalIndex];

                _AnimationFps = animationFps;
                PlaybackFps = animationFps * _PlaybackFpsMultiplier;
                _PlaybackTimer.Interval = new TimeSpan((int)(TimeSpan.TicksPerSecond * (1.0 / PlaybackFps)));

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

            if(resetToOriginalIndex)
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
            get => $"{CurrentFrameIndex}/{Frames.Count - 1}";
        }

        private void DispatcherTimer_Elapsed(object sender, EventArgs e)
        {
            var lastFrame = CurrentFrame;
            CurrentFrameIndex = (CurrentFrameIndex + 1) % _Frames.Count;
            CurrentFrame = _Frames[CurrentFrameIndex];
        }
    }
}
