using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;

namespace AnimationEditor.Interfaces
{
    public interface IFrameModel
    {
        //int Order { get; }
        //Layers
        StrokeCollection StrokeCollection { get; }
    }
}
