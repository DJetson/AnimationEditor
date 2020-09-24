using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;
using System.Windows.Input;

namespace AnimationEditorCore.ViewModels.StateObjects
{
    public class StrokeState
    {
        public class FrameState
        {
            public DrawingAttributes DrawingAttributes;
            public StylusPointCollection StylusPoints;

            public FrameState(Stroke stroke)
            {
                DrawingAttributes = stroke.DrawingAttributes.Clone();
                StylusPoints = new StylusPointCollection(stroke.StylusPoints);
            }
        }
    }
}
