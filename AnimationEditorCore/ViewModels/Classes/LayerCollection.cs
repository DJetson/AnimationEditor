//using System;
//using System.Collections.Generic;
//using System.Collections.ObjectModel;
//using System.Text;
//using System.Windows.Data;

//namespace AnimationEditorCore.ViewModels.Classes
//{
//    public class LayerCollection : ViewModelBase
//    {
//        private ObservableCollection<LayerViewModel> _Layers = new ObservableCollection<LayerViewModel>();
//        public ObservableCollection<LayerViewModel> Layers
//        {
//            get { return _Layers; }
//            set { _Layers = value; NotifyPropertyChanged(); }
//        }

//        private CollectionViewSource _SortedLayers = new CollectionViewSource();
//        public CollectionView SortedLayers
//        {
//            get => (CollectionView)_SortedLayers.View;
//        }

//        private LayerViewModel _ActiveLayer;
//        public LayerViewModel ActiveLayer
//        {
//            get { return _ActiveLayer; }
//            set
//            {
//                _ActiveLayer = value;
//                NotifyPropertyChanged(nameof(ActiveLayer), nameof(ActiveLayerIndex));
//                foreach (var layer in Layers)
//                {
//                    if (layer == ActiveLayer)
//                        layer.IsActive = true;
//                    else
//                        layer.IsActive = false;
//                }
//            }
//        }
//        public int ActiveLayerIndex
//        {
//            get => Layers.IndexOf(ActiveLayer);
//        }
//    }
//}
