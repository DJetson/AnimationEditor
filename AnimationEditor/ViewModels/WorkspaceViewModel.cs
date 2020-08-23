using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Ink;

namespace AnimationEditor.ViewModels
{
    public class WorkspaceViewModel : ViewModelBase
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

        private bool _UnsavedChanges = false;
        public bool UnsavedChanged
        {
            get => _UnsavedChanges;
            set { _UnsavedChanges = value; NotifyPropertyChanged(); }
        }

        public override string DisplayName
        {
            get => $"{_DisplayName}{(_UnsavedChanges ? "*" : "")}";
            set { _DisplayName = value; NotifyPropertyChanged(); }
        }

        public WorkspaceViewModel()
        {
            StrokeCollection = new StrokeCollection();
            StrokeCollection.StrokesChanged += StrokeCollection_StrokesChanged;
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
