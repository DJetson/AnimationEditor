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
    public class PasteStrokesFromClipboardCommand : RequeryBase
    {
        public override string Description => Resources.PasteStrokesFromClipboardDescription;
        public override string ToolTip => Resources.PasteStrokesFromClipboardToolTip;
        public override string UndoStateTitle => Resources.PasteStrokesFromClipboardUndoStateTitle;
        public override bool CanExecute(object parameter)
        {
            if (!(parameter is TimelineViewModel Parameter))
                return false;

            if (!InternalClipboard.HasData())
                return false;

            //if (Parameter.Layers.ActiveLayer.Frames[Parameter.SelectedFrameIndex].SelectedStrokes.Count == 0)
            //    return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            var Parameter = parameter as TimelineViewModel;

            var copiedStrokes = InternalClipboard.GetData();
            var copyToFrame = Parameter.Layers.ActiveLayer.Frames[Parameter.SelectedFrameIndex];

            copyToFrame.StrokeCollection.Add(copiedStrokes);

            //Reselect the copied Strokes
            copyToFrame.SelectedStrokes.Add(copiedStrokes);

            WorkspaceHistoryViewModel.PushUndoRecord(UndoStateTitle, false);
        }
    }
}
