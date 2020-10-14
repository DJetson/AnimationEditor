using AnimationEditorCore.Commands.BaseClasses;
using AnimationEditorCore.Properties;
using AnimationEditorCore.Utilities;
using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Ink;

namespace AnimationEditorCore.Commands.Timeline.FrameContent
{
    public class CopySelectedStrokesToClipboardCommand : RequeryBase
    {
        public override string Description => Resources.CopySelectedStrokesToClipboardDescription;
        public override string ToolTip => Resources.CopySelectedStrokesToClipboardToolTip;

        public override bool CanExecute(object parameter)
        {
            if (!(parameter is TimelineViewModel Parameter))
                return false;

            if (Parameter.ActiveLayer.Frames[Parameter.SelectedFrameIndex].SelectedStrokes.Count == 0)
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            var Parameter = parameter as TimelineViewModel;

            InternalClipboard.SetData(Parameter.ActiveLayer.Frames[Parameter.SelectedFrameIndex].SelectedStrokes);
        }
    }
}
