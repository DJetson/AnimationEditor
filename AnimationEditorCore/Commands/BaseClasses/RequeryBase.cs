using AnimationEditorCore.ViewModels;
using System;
using System.Windows.Input;

namespace AnimationEditorCore.Commands.BaseClasses
{
    public abstract class RequeryBase : ViewModelBase, ICommand
    {
        protected string _UndoStateTitle;
        public virtual string UndoStateTitle
        {
            get { return _UndoStateTitle; }
            set { _UndoStateTitle = value; NotifyPropertyChanged(); }
        }

        protected string _Description;
        public virtual string Description
        {
            get { return _Description; }
            set { _Description = value; NotifyPropertyChanged(); }
        }

        protected string _ToolTip;
        public virtual string ToolTip
        {
            get { return _ToolTip; }
            set { _ToolTip = value; NotifyPropertyChanged(); }
        }

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
