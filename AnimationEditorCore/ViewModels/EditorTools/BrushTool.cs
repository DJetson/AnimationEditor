using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
