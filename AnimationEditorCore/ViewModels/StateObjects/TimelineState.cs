using System;

namespace AnimationEditorCore.ViewModels.StateObjects
{
    public class TimelineState : UndoStateViewModel
    {
        public TimelineViewModel Timeline;

        public TimelineState(TimelineViewModel timeline, string stateName = "") : base(timeline, stateName)
        {
            Timeline = new TimelineViewModel(timeline);
            DisplayName = stateName;
        }

        public override void LoadState()
        {
            Console.WriteLine("Loading TimelineState");
            TimelineViewModel originator = Originator as TimelineViewModel;
            originator.LoadState(this);
        }
    }
}
