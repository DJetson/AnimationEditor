using AnimationEditorCore.BaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationEditorCore.ViewModels
{
    public class ViewModelBase : NotifyBase
    {
        protected string _DisplayName;
        public virtual string DisplayName
        {
            get => _DisplayName;
            set { _DisplayName = value; NotifyPropertyChanged(); }
        }
    }
}
