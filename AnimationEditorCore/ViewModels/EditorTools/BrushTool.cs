using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Interfaces;

namespace AnimationEditorCore.ViewModels.EditorTools
{
    public class BrushTool : ViewModelBase, IEditorTool
    {
        private float _BrushSize = 1;
        public float BrushSize
        {
            get => _BrushSize;
            set { _BrushSize = value; NotifyPropertyChanged(); }
        }

        public EditorToolType ToolType
        {
            get => EditorToolType.Brush;
        }
    }
}
