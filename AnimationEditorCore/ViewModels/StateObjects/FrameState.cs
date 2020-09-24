using AnimationEditorCore.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;

namespace AnimationEditorCore.ViewModels.StateObjects
{
    public class FrameState : UndoStateViewModel
    {
        public StrokeCollection StrokeCollection;
        public ObservableCollection<LayerViewModel> Layers;
        public int ActiveLayerIndex;
        public FrameState(FrameViewModel frame, string stateName = "") : base(frame, stateName)
        {
            Layers = new ObservableCollection<LayerViewModel>();
            foreach(var layer in frame.Layers)
            {
                Layers.Add(new LayerViewModel(layer));
            }
            ActiveLayerIndex = frame.Layers.IndexOf(frame.ActiveLayer);
            //StrokeCollection = new StrokeCollection(frame.StrokeCollection);
            //StrokeCollection.Clear();
            //foreach(var stroke in frame.StrokeCollection)
            //{
            //    StrokeCollection.Add(stroke.Clone());
            //}
        }

        public override void LoadState()
        {
            FrameViewModel originator = Originator as FrameViewModel;
            originator.LoadState(this);
        }
    }
}
