using AnimationEditorCore.ViewModels;
using System;
using System.Windows.Input;

namespace AnimationEditorCore.BaseClasses
{
    public abstract class RequeryBase : ViewModelBase, ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        public RequeryBase(string displayName = "")
        {
            DisplayName = displayName;
        }

        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        public abstract void Execute(object parameter);
    }
}
