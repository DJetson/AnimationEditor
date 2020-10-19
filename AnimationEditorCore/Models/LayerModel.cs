using AnimationEditorCore.ViewModels;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AnimationEditorCore.Models
{
    public class LayerModel
    {
        public List<FrameModel> Frames { get; set; }
        public bool IsVisible { get; set; }
        public int LayerId { get; set; }
        public int ArrangedZIndex { get; set; }
        public string DisplayName { get; set; }
        public int SelectedFrameIndex { get; set; }
        public bool IsActive { get; set; }

        public LayerModel()
        {
        }

        public LayerModel(LayerViewModel layer)
        {
            DisplayName = layer.DisplayName;
            IsVisible = layer.IsVisible;
            LayerId = layer.ZIndex;
            ArrangedZIndex = layer.ZIndex;
            SelectedFrameIndex = layer.SelectedFrameIndex;
            IsActive = layer.IsActive;
            Frames = new List<FrameModel>(layer.Frames.Select(e => new FrameModel(e)));
        }

        public LayerModel(Stream stream, string displayName, bool isVisible, int layerId, int arrangedZIndex, int selectedFrameIndex, bool isActive, List<KeyFrameViewModel> frames)
        {
            DisplayName = displayName;
            IsVisible = isVisible;
            LayerId = layerId;
            ArrangedZIndex = arrangedZIndex;
            SelectedFrameIndex = selectedFrameIndex;
            IsActive = IsActive;
            Frames = new List<FrameModel>(frames.Select(e => new FrameModel(stream, e.Order)));
        }
    }
}
