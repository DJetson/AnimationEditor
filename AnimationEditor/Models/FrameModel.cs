using AnimationEditor.Interfaces;
using AnimationEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;

namespace AnimationEditor.Models
{
    [Serializable]
    public class FrameModel : IFrameModel
    {
        private StrokeCollection _StrokeCollection;
        public StrokeCollection StrokeCollection => _StrokeCollection;

        public FrameModel()
        {
        }

        public FrameModel(StrokeCollection strokeCollection)
        {
            _StrokeCollection = strokeCollection;
        }

    }
}
