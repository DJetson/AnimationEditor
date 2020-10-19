using AnimationEditorCore.Interfaces;
using AnimationEditorCore.ViewModels;
using System.IO;
using System.Windows.Ink;

namespace AnimationEditorCore.Models
{
    public class FrameModel
    {
        public bool IsKeyFrame { get; set; }
        public int Order { get; set; }
        public StrokeCollection StrokeCollection { get; set; }

        public FrameModel()
        {
        }

        public FrameModel(FrameViewModel frame)
        {
            Order = frame.Order;
        }

        public FrameModel(IFrameViewModel frame)
        {
            IsKeyFrame = false;
            Order = frame.Order;

            if(frame is KeyFrameViewModel keyFrame)
            {
                IsKeyFrame = true;
                StrokeCollection = new StrokeCollection(keyFrame.StrokeCollection);
            }
        }

        public FrameModel(Stream stream, int order, bool isKeyFrame = true)
        {
            Order = order;
            IsKeyFrame = isKeyFrame;

            if (isKeyFrame)
                StrokeCollection = new StrokeCollection(stream);
        }
    }
}
