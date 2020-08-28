using AnimationEditor.Interfaces;
using System.IO;
using System.Windows.Ink;

namespace AnimationEditor.Models
{
    public class FrameModel
    {
        public StrokeCollection StrokeCollection { get; set; }

        //private int _Order;
        //public int Order => _Order;

        public FrameModel()
        {
        }

        public FrameModel(StrokeCollection strokeCollection)
        {
            StrokeCollection = strokeCollection;
        }

        public FrameModel(Stream stream)
        {
            StrokeCollection = new StrokeCollection(stream);
        }
    }
}
