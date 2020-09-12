using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;

namespace AnimationEditor.Models
{
    public class LayerModel
    {
        public StrokeCollection StrokeCollection { get; set; }

        public LayerModel()
        {
        }

        public LayerModel(StrokeCollection strokeCollection)
        {
            StrokeCollection = strokeCollection;
        }

        public LayerModel(Stream stream)
        {
            StrokeCollection = new StrokeCollection(stream);
        }
    }
}
