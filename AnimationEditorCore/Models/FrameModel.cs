using AnimationEditorCore.ViewModels;
using System.IO;
using System.Windows.Ink;

namespace AnimationEditorCore.Models
{
    public class FrameModel
    {
        public int Order { get; set; }
        public StrokeCollection StrokeCollection { get; set; }

        public FrameModel()
        {
        }

        public FrameModel(FrameViewModel frame)
        {
            Order = frame.Order;
            StrokeCollection = new StrokeCollection(frame.StrokeCollection);
        }

        public FrameModel(Stream stream, int order)
        {
            Order = order;
            StrokeCollection = new StrokeCollection(stream);
        }
    }
}
