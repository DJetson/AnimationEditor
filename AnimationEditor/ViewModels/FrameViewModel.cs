using AnimationEditor.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;

namespace AnimationEditor.ViewModels
{
    public class FrameViewModel : ViewModelBase
    {
        private StrokeCollection _StrokeCollection = new StrokeCollection();
        public StrokeCollection StrokeCollection
        {
            get => _StrokeCollection;
            set { _StrokeCollection = value; }
        }

        private ObservableCollection<Stroke> _Strokes = new ObservableCollection<Stroke>();
        public ObservableCollection<Stroke> Strokes
        {
            get => _Strokes;
            set { _Strokes = value; NotifyPropertyChanged(); }
        }

        //private int _Order;
        //public int Order
        //{
        //    get => _Order;
        //    set { _Order = value; NotifyPropertyChanged(); }
        //}

        public FrameViewModel()
        {
            StrokeCollection = new StrokeCollection();
            StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;
        }

        public FrameViewModel(IFrameModel model)
        {
            StrokeCollection = model.StrokeCollection;

            foreach (var item in StrokeCollection)
                Strokes.Add(item);
        }

        private void StrokeCollection_StrokesChanged(object sender, StrokeCollectionChangedEventArgs e)
        {
            foreach (var stroke in e.Added)
            {
                Strokes.Add(stroke);
            }
            foreach (var stroke in e.Removed)
            {
                Strokes.Remove(stroke);
            }
        }
    }
}
