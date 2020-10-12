﻿using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Utilities;
using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Ink;

namespace AnimationEditorCore.Commands.Timeline.FrameContent
{
    public class CutSelectedStrokesToClipboardCommand : RequeryBase
    {
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

            var strokes = Parameter.ActiveLayer.Frames[Parameter.SelectedFrameIndex].SelectedStrokes;
            InternalClipboard.SetData(strokes);
            Parameter.ActiveLayer.Frames[Parameter.SelectedFrameIndex].RemoveStrokes(strokes);
            //Parameter.PushUndoRecord(Parameter.CreateUndoState("Cut Selected Strokes"));
        }
    }
}