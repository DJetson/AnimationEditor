using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationEditor.ViewModels.StateObjects
{
    public class AnimationTimelineState
    {
        public FrameViewModel SelectedFrame;
        public double FramesPerSecond;
        public ObservableCollection<FrameViewModel> Frames;

        public AnimationTimelineState(AnimationTimelineViewModel state)
        {
            SelectedFrame = state.SelectedFrame;
            FramesPerSecond = state.FramesPerSecond;
            Frames = new ObservableCollection<FrameViewModel>(state.Frames);
        }
    }
}
