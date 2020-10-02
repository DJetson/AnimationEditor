using AnimationEditorCore.BaseClasses;

namespace AnimationEditorCore.Interfaces
{
    public interface IEditorTool
    {
        float BrushSize { get; set; }
        EditorToolType ToolType { get; }
    }
}
