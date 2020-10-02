using System;

namespace AnimationEditorCore.ViewModels.StateObjects
{
    public class LayerState : UndoStateViewModel
    {
        public LayerViewModel Layer;
        public LayerState(LayerViewModel layer, string stateName = "") : base(layer, stateName)
        {
            Layer = new LayerViewModel(layer);
            DisplayName = stateName;
        }

        public override void LoadState()
        {
            Console.WriteLine("Loading LayerState");
            LayerViewModel originator = Originator as LayerViewModel;
            originator.LoadState(this);
        }
    }
}
