using AnimationEditorCore.Commands.BaseClasses;
using AnimationEditorCore.Properties;
using AnimationEditorCore.Utilities;
using AnimationEditorCore.ViewModels;
using System.Linq;

namespace AnimationEditorCore.Commands.Timeline
{
    public class TogglePlaybackCommand : RequeryBase
    {
        public override string Description => Resources.TogglePlaybackDescription;
        public override string ToolTip => Resources.TogglePlaybackToolTip;
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
                var flattenedFrames = AnimationUtilities.FlattenFrames(Parameter.Layers.ToList());

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
