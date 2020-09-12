using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;

namespace AnimationEditor.ViewModels.StateObjects
{
    public class LayerState : UndoStateViewModel
    {
        public StrokeCollection StrokeCollection;

        public LayerState(LayerViewModel layer, string stateName = "") : base(layer, stateName)
        {
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
