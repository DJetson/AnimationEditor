using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;

namespace AnimationEditorCore.ViewModels.StateObjects
{
    public class LayerState : UndoStateViewModel
    {
        public StrokeCollection StrokeCollection;
        public int LayerId;
        public bool IsVisible;

        public LayerState(LayerViewModel layer, string stateName = "") : base(layer, stateName)
        {
            DisplayName = layer.DisplayName;
            LayerId = layer.LayerId;
            IsVisible = layer.IsVisible;
            StrokeCollection = new StrokeCollection(layer.StrokeCollection);
            StrokeCollection.Clear();
            foreach (var stroke in layer.StrokeCollection)
            {
                StrokeCollection.Add(stroke.Clone());
            }
        }

        public override void LoadState()
        {
            LayerViewModel originator = Originator as LayerViewModel;
            originator.LoadState(this);
        }
    }
}
