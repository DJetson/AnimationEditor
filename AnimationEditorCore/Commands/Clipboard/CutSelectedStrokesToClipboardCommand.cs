using AnimationEditorCore.Commands.BaseClasses;
using AnimationEditorCore.Properties;
using AnimationEditorCore.Utilities;
using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Ink;

namespace AnimationEditorCore.Commands.Clipboard
{
    public class CutSelectedStrokesToClipboardCommand : RequeryBase
    {
        public override string Description => Resources.CutSelectedStrokesToClipboardDescription;
        public override string ToolTip => Resources.CutSelectedStrokesToClipboardToolTip;
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
            var strokes = keyFrame.SelectedStrokes;
            InternalClipboard.SetData(strokes);
            keyFrame.RemoveStrokes(strokes);
            //Parameter.PushUndoRecord(Parameter.CreateUndoState("Cut Selected Strokes"));
        }
    }
}
