using AnimationEditor.Interfaces;
using AnimationEditor.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;

namespace AnimationEditor.Models
{
    [Serializable]
    public class FrameModel : IFrameModel, ISerializable
    {
        private StrokeCollection _StrokeCollection;
        public StrokeCollection StrokeCollection => _StrokeCollection;

        //private int _Order;
        //public int Order => _Order;

        public FrameModel()
        {
        }

        public FrameModel(StrokeCollection strokeCollection)
        {
            _StrokeCollection = strokeCollection;
        }

        public FrameModel(Stream stream)
        {
            _StrokeCollection = new StrokeCollection(stream);
        }

        public FrameModel(SerializationInfo info, StreamingContext context)
        {

        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            //requires a Stream object, but all I have is the two args above.
        }
    }
}
