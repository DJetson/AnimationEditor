using AnimationEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;

namespace AnimationEditor.Utilities
{
    public class AnimationPlaybackState
    {
        public enum PlaybackStates { Stop, Play, Pause };

        private AnimationTimelineViewModel _AnimationTimelineViewModel;
        private int _OriginalSelectedFrameIndex;
        private DispatcherTimer _PlaybackTimer;
        private int _CurrentFrameIndex;
        private FrameViewModel _CurrentFrame;
        private List<FrameViewModel> _Frames;
        private int _FrameIndexOffset;
        private double _FramesPerSecond;

        private PlaybackStates _CurrentState = PlaybackStates.Stop;
        public PlaybackStates PlaybackState => _CurrentState;

        public delegate void AnimationPlaybackStateChangeRequestedEvent(object requestedBy, PlaybackStates requestedState);
        private event AnimationPlaybackStateChangeRequestedEvent PlaybackStateChangeRequested;

        public AnimationPlaybackState(AnimationTimelineViewModel animationTimelineViewModel, List<FrameViewModel> playbackFrames = null)
        {
            _AnimationTimelineViewModel = animationTimelineViewModel;
            _OriginalSelectedFrameIndex = animationTimelineViewModel.Frames.IndexOf(animationTimelineViewModel.SelectedFrame);

            if (playbackFrames != null)
            {
                _FrameIndexOffset = animationTimelineViewModel.Frames.IndexOf(playbackFrames.FirstOrDefault());
                _Frames = playbackFrames;
            }
            else
            {
                _FrameIndexOffset = 0;
                _Frames = animationTimelineViewModel.Frames.ToList();
            }

            _FramesPerSecond = animationTimelineViewModel.FramesPerSecond;

            _PlaybackTimer = new DispatcherTimer(DispatcherPriority.Render)
            {
                Interval = new TimeSpan((int)(TimeSpan.TicksPerSecond * (1.0 / animationTimelineViewModel.FramesPerSecond)))
            };

            _PlaybackTimer.Tick += (sender, e) =>
            {
                _CurrentFrameIndex = (_CurrentFrameIndex + 1) % _Frames.Count;
                _AnimationTimelineViewModel.SelectedFrame = _AnimationTimelineViewModel.Frames[_CurrentFrameIndex + _FrameIndexOffset];
            };

            PlaybackStateChangeRequested += (requestedBy, state) =>
            {
                _PlaybackTimer.Stop();

                if (state == PlaybackStates.Stop)
                {
                    _AnimationTimelineViewModel.SelectedFrame = _AnimationTimelineViewModel.Frames[_OriginalSelectedFrameIndex];
                    _CurrentState = PlaybackStates.Stop;
                }
                else if(state == PlaybackStates.Pause)
                {
                    _CurrentState = PlaybackStates.Pause;
                }
            };

            _CurrentState = PlaybackStates.Play;
            _AnimationTimelineViewModel.IsInPlaybackMode = true;
            _PlaybackTimer.Start();
        }

        public void RequestPlaybackState(object requestedBy, PlaybackStates state)
            => PlaybackStateChangeRequested?.Invoke(requestedBy, state);
    }
}
