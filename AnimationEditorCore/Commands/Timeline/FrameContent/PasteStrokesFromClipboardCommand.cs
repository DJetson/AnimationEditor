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
    public class PasteStrokesFromClipboardCommand : RequeryBase
    {
        public override bool CanExecute(object parameter)
        {
            if (!(parameter is TimelineViewModel Parameter))
                return false;

            if (!(InternalClipboard.HasData()))
                return false;

            //if (Parameter.ActiveLayer.Frames[Parameter.SelectedFrameIndex].SelectedStrokes.Count == 0)
            //    return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            var Parameter = parameter as TimelineViewModel;

            Parameter.ActiveLayer.Frames[Parameter.SelectedFrameIndex].StrokeCollection.Add(InternalClipboard.GetData());

            Parameter.PushUndoRecord(Parameter.CreateUndoState("Paste Into Frame"));
        }
    }
}