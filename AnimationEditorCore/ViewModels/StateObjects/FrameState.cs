﻿using AnimationEditorCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;

namespace AnimationEditorCore.ViewModels.StateObjects
{
    public class FrameState : UndoStateViewModel
    {
        public int Order;
        public StrokeCollection StrokeCollection;
        public FrameState(FrameViewModel frame, string stateName = "") : base(frame, stateName)
        {
            Order = frame.Order;
            StrokeCollection = new StrokeCollection();
            //StrokeCollection.Clear();
            foreach (var stroke in frame.StrokeCollection)
            {
                StrokeCollection.Add(stroke.Clone());
            }
        }

        public override void LoadState()
        {
            Console.WriteLine("Loading FrameState");
            FrameViewModel originator = Originator as FrameViewModel;
            originator.LoadState(this);
        }
    }
}
