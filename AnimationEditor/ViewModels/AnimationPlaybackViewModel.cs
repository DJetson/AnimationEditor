using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace AnimationEditor.ViewModels
{
    //TODO: I need to do something to make it easier for other ViewModels to signal this one to fully Stop, 
    //      including from a Pause state, whenever they're preparing to execute some operation that could 
    //      cause problems or erratic behavior if attempted while the selected frame (and thus also the 
    //      binding source for the InkCanvas) is rapidly changing. We don't want people accidentally drawing 
    //      a big ugly stroke across dozens of frames of an animation they're flashing by on the InkCanvas 
    //      during playback. 
    //      Alternatively, or perhaps additionally, I might need to consider having some way to lock out
    //      a particular category of operations. So that they automatically bail out if this object isn't in
    //      the Stop state. Perhaps I could accomplish this lock out functionality by subclassing one or both 
    //      of the two main Command types (The StaticResource Commands derived from RequeryBase, and the 
    //      DelegateCommands defined directly on the ViewModels) and making a version of each, or a single 
    //      common ancestor that acknowledges playback states, and automatically returns a false from 
    //      CanExecute if the playback state is anything but Stop.
    public enum PlaybackStates { Stop, Play, Pause };

    public class AnimationPlaybackViewModel : ViewModelBase
    {
        private List<FrameViewModel> _Frames = null;
        private double _AnimationFps;
        private FrameViewModel _OriginalFrame;
        private DispatcherTimer _PlaybackTimer;

        private FrameViewModel _CurrentFrame;
        public FrameViewModel CurrentFrame
        {
            get { return _CurrentFrame; }
            set { _CurrentFrame = value; NotifyPropertyChanged(); }
        }

        private PlaybackStates _CurrentState = PlaybackStates.Stop;
        public PlaybackStates CurrentState
        {
            get { return _CurrentState; }
            set { _CurrentState = value; NotifyPropertyChanged(); }
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

        public void StartPlayback(List<FrameViewModel> playbackFrames, double animationFps)
        {
            if (CurrentState == PlaybackStates.Play)
            {
                Console.WriteLine("Playback has already Started. This, and any further calls to StartPlayback() on this instance are unnecessary and will have no effect while it remains in the Play state.");
                return;
            }
            else if (CurrentState == PlaybackStates.Pause)
            {
                throw new InvalidOperationException("CurrentState = Pause. But playback can only be Started when CurrentState == Stop. Try calling ResumePlayback instead.");
            }
            else
            {
                _Frames = playbackFrames;
                _OriginalFrame = playbackFrames.First();
                CurrentFrame = _OriginalFrame;

                _AnimationFps = animationFps;
                PlaybackFps = animationFps * _PlaybackFpsMultiplier;
                _PlaybackTimer.Interval = new TimeSpan((int)(TimeSpan.TicksPerSecond * (1.0 / PlaybackFps)));

                CurrentState = PlaybackStates.Play;
                _PlaybackTimer.Start();
            }
        }

        public void ResumePlayback()
        {
            if (CurrentState == PlaybackStates.Play)
            {
                Console.WriteLine("Playback has already Resumed. This, and any further calls to ResumePlayback() on this instance are unnecessary and will have no effect while it remains in the Play state.");
                return;
            }
            else if (CurrentState == PlaybackStates.Stop)
            {
                throw new InvalidOperationException("CurrentState = Stop. But playback can only be Resumed when CurrentState == Pause. Try calling StartPlayback instead.");
            }
            else
            {
                CurrentState = PlaybackStates.Play;
                _PlaybackTimer.Start();
            }
        }

        public void PausePlayback()
        {
            if (CurrentState == PlaybackStates.Pause || CurrentState == PlaybackStates.Stop)
            {
                Console.WriteLine($"CurrentState is {CurrentState}. This, and any further calls to PausePlayback() on this instance are unnecessary and will have no effect while it remains in the {CurrentState} state.");
            }
            else
            {
                _PlaybackTimer.Stop();
                CurrentState = PlaybackStates.Pause;
            }
        }

        public void StopPlayback()
        {
            if (CurrentState == PlaybackStates.Stop)
            {
                Console.WriteLine("Playback has already been Stopped. This, and any further calls to StopPlayback() on this instance are unnecessary and will have no effect while it remains in the Stop state.");
            }
            else if (CurrentState == PlaybackStates.Play)
            {
                _PlaybackTimer.Stop();
            }

            CurrentFrame = _OriginalFrame;
            _OriginalFrame = null;
            _Frames.Clear();
            CurrentState = PlaybackStates.Stop;
        }

        private void DispatcherTimer_Elapsed(object sender, EventArgs e)
        {
            var enumerator = _Frames.GetEnumerator();

            while (enumerator.Current != _CurrentFrame)
            {
                if (enumerator.MoveNext() == false)
                {
                    throw new IndexOutOfRangeException("Error: Frame not found in Playback Frames collection.");
                }
            }

            if (enumerator.MoveNext() == false)
            {
                CurrentFrame = _Frames.First();
            }
            else
            {
                CurrentFrame = enumerator.Current;
            }
        }
    }
}
