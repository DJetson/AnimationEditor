using System.ComponentModel;
using System.Runtime.CompilerServices;

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
    }
}
