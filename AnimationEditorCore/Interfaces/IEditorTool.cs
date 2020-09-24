using AnimationEditorCore.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationEditorCore.Interfaces
{
    public interface IEditorTool
    {
        float BrushSize { get; set; }
        EditorToolType ToolType { get; }
    }
}
