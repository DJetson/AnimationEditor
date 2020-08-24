using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationEditor.BaseClasses
{
    public class DelegateCommand : RequeryBase
    {
        private readonly Predicate<object> _CanExecute;
        private readonly Action<object> _Execute;

        public DelegateCommand(Action<object> execute)
        {
            _Execute = execute;
        }

        public DelegateCommand(Predicate<object> canExecute, Action<object> execute)
        {
            _CanExecute = canExecute;
            _Execute = execute;
        }

        public override bool CanExecute(object parameter)
        {
            return _CanExecute(parameter);
        }

        public override void Execute(object parameter)
        {
            _Execute(parameter);
        }
    }
}
