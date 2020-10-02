using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace AnimationEditorCore.BaseClasses
{
    public class NotifyBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        public void NotifyPropertyChanged([CallerMemberName]string property = "", params string[] dependentProperties)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
            foreach (var dependentProperty in dependentProperties)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(dependentProperty));
            }
        }

        //public void NotifyPropertiesChanged([CallerMemberName] string originatingProperty = "", params string[] dependentProperties)
        //{
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(originatingProperty));
        //    foreach(var property in dependentProperties)
        //    {
        //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        //    }
        //}
    }
}
