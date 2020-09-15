using AnimationEditor.Interfaces;
using AnimationEditor.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Ink;

namespace AnimationEditor.Models
{
    public class FrameModel
    {
        public List<LayerModel> Layers { get; set; }
        public int Order { get; set; }

        public FrameModel()
        {
        }

        public FrameModel(FrameViewModel frame)
        {
            Order = frame.Order;
            Layers = new List<LayerModel>(frame.Layers.Select(e => new LayerModel(e)));
        }

        public FrameModel(Stream stream, int order, List<LayerViewModel> layers)
        {
            Order = order;
            Layers = new List<LayerModel>(layers.Select(e => new LayerModel(stream, e.LayerId, e.IsVisible)));
        }
    }
}
