using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;

namespace AnimationEditor.ViewModels.StateObjects
{
    public class FrameState
    {
        public StrokeCollection StrokeCollection;

        public FrameState(FrameViewModel frame)
        {
            StrokeCollection = new StrokeCollection(frame.StrokeCollection);
        }
    }
}
