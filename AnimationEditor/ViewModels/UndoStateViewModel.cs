using AnimationEditor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationEditor.ViewModels
{
    public class UndoStateViewModel<T> : ViewModelBase, IMemento
    {
        public override string DisplayName
        {
            get => _DisplayName;
            set { _DisplayName = value; NotifyPropertyChanged(); }
        }

        private IMementoOriginator _Originator;
        public IMementoOriginator Originator
        {
            get => _Originator;
            set { _Originator = value; NotifyPropertyChanged(); }
        }

        private T _State;
        public T State
        {
            get => _State;
            set { _State = value; NotifyPropertyChanged(); }
        }
    }
}
