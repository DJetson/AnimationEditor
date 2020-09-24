using AnimationEditorCore.Interfaces;
using AnimationEditorCore.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Ink;

namespace AnimationEditorCore.Models
{
    public class FrameModel
    {
        public List<LayerModel> Layers { get; set; }
        public int Order { get; set; }
        public int ActiveLayerIndex { get; set; }

        public FrameModel()
        {
        }

        public FrameModel(FrameViewModel frame)
        {
            Order = frame.Order;
            Layers = new List<LayerModel>(frame.Layers.Select(e => new LayerModel(e)));
            ActiveLayerIndex = frame.Layers.IndexOf(frame.ActiveLayer);
        }

        public FrameModel(Stream stream, int order, List<LayerViewModel> layers, int activeLayerIndex)
        {
            Order = order;
            Layers = new List<LayerModel>(layers.Select(e => new LayerModel(stream, e.LayerId, e.IsVisible)));
            ActiveLayerIndex = activeLayerIndex;
        }
    }
}
