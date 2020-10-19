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

namespace AnimationEditorCore.Commands.Clipboard
{
    public class CopySelectedStrokesToClipboardCommand : RequeryBase
    {
        public override string Description => Resources.CopySelectedStrokesToClipboardDescription;
        public override string ToolTip => Resources.CopySelectedStrokesToClipboardToolTip;

        public override bool CanExecute(object parameter)
        {
            if (!(parameter is TimelineViewModel Parameter))
                return false;

            if (!(Parameter.Layers.ActiveLayer.Frames[Parameter.SelectedFrameIndex] is KeyFrameViewModel keyFrame))
                return false;

            if (keyFrame.SelectedStrokes.Count == 0)
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            var Parameter = parameter as TimelineViewModel;

            var keyFrame = Parameter.Layers.ActiveLayer.Frames[Parameter.SelectedFrameIndex] as KeyFrameViewModel;

            InternalClipboard.SetData(keyFrame.SelectedStrokes);
        }
    }
}
