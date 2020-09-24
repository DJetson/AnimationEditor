using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationEditorCore.ViewModels.StateObjects
{
    public class AnimationTimelineState : UndoStateViewModel
    {
        public FrameViewModel SelectedFrame;
        public double FramesPerSecond;
        public ObservableCollection<FrameViewModel> Frames;

        public AnimationTimelineState(AnimationTimelineViewModel viewModel, string stateName = "") : base(viewModel, stateName)
        {
            SelectedFrame = viewModel.SelectedFrame;
            FramesPerSecond = viewModel.FramesPerSecond;
            Frames = new ObservableCollection<FrameViewModel>(viewModel.Frames);
        }

        public override void LoadState()
        {
            AnimationTimelineViewModel originator = Originator as AnimationTimelineViewModel;
            originator.LoadState(this);
            //originator.StrokeCollection.StrokesChanged -= StrokeCollection_StrokesChanged;
            //originator.StrokeCollection = StrokeCollection;
            //StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;
            //CurrentState = Memento;
        }
    }
}
