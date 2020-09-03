using AnimationEditor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;

namespace AnimationEditor.ViewModels.StateObjects
{
    public class FrameState : UndoStateViewModel
    {
        public StrokeCollection StrokeCollection;

        public FrameState(FrameViewModel frame, string stateName = "") : base(frame, stateName)
        {
            StrokeCollection = new StrokeCollection(frame.StrokeCollection);
        }

        public override void LoadState()
        {
            FrameViewModel originator = Originator as FrameViewModel;
            originator.LoadState(this);
        }
    }
}
