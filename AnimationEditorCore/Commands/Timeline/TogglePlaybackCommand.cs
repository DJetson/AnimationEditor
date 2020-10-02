using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.ViewModels;

namespace AnimationEditorCore.Commands.Timeline
{
    public class TogglePlaybackCommand : RequeryBase
    {
        public override bool CanExecute(object parameter)
        {
            if (!(parameter is TimelineViewModel Parameter))
            {
                return false;
            }

            return true;
        }

        public override void Execute(object parameter)
        {
            var Parameter = parameter as TimelineViewModel;

            if (!Parameter.AnimationPlaybackViewModel.IsPlaybackActive)
            {
                var flattenedFrames = Parameter.FlattenFrames();

                Parameter.AnimationPlaybackViewModel.StartPlayback(flattenedFrames, Parameter.FramesPerSecond, Parameter.SelectedFrameIndex);
            }
            else
            {
                Parameter.AnimationPlaybackViewModel.StopPlayback(false);
                Parameter.SelectedFrameIndex = Parameter.AnimationPlaybackViewModel.CurrentFrameIndex;
            }
        }
    }
}
