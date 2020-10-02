using System;

namespace AnimationEditorCore.ViewModels.StateObjects
{
    public class FrameState : UndoStateViewModel
    {
        public FrameViewModel Frame;

        public FrameState(FrameViewModel frame, string stateName = "") : base(frame, stateName)
        {
            Frame = new FrameViewModel(frame);
            DisplayName = stateName;
        }

        public override void LoadState()
        {
            Console.WriteLine("Loading FrameState");
            FrameViewModel originator = Originator as FrameViewModel;
            originator.LoadState(this);
        }
    }
}
