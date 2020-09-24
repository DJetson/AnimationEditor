using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;

namespace AnimationEditorCore.Models
{
    public class LayerModel
    {
        public StrokeCollection StrokeCollection { get; set; }
        public bool IsVisible { get; set; }
        public int LayerId { get; set; }
        public string DisplayName { get; set; }

        public LayerModel()
        {
        }

        public LayerModel(LayerViewModel layer)
        {
            DisplayName = layer.DisplayName;
            IsVisible = layer.IsVisible;
            LayerId = layer.LayerId;
            StrokeCollection = new StrokeCollection(layer.StrokeCollection);
        }

        public LayerModel(Stream stream, int layerId, bool isVisible)
        {
            LayerId = layerId;
            IsVisible = isVisible;
            StrokeCollection = new StrokeCollection(stream);
        }
    }
}
