using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Utilities;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace AnimationEditorCore.Commands
{
    public class SetStatusBarMessageCommand : RequeryBase
    {
        public override bool CanExecute(object parameter)
        {
            if (parameter is Button button && !(IsValidExecutionSource(button)))
                return false;

            if (parameter is ToggleButton toggleButton && !(IsValidExecutionSource(toggleButton)))
                return false;

            if (parameter is RadioButton radioButton && !(IsValidExecutionSource(radioButton)))
                return false;

            return true;
        }

        public bool IsValidExecutionSource(Button source)
        {
            if (!(source.Command is RequeryBase attachedCommand))
                return false;

            if (String.IsNullOrWhiteSpace(attachedCommand.Description))
                return false;

            return true;
        }

        public bool IsValidExecutionSource(ToggleButton source)
        {
            if (!(source.Command is RequeryBase attachedCommand))
                return false;

            if (String.IsNullOrWhiteSpace(attachedCommand.Description))
                return false;

            return true;
        }

        public bool IsValidExecutionSource(RadioButton source)
        {
            if (!(source.Command is RequeryBase attachedCommand))
                return false;

            if (String.IsNullOrWhiteSpace(attachedCommand.Description))
                return false;

            return true;
        }

        public override void Execute(object parameter)
        {
            //var Parameter = parameter as Button;
            //var attachedCommand = Parameter.Command as RequeryBase;

            //StatusBarMessaging.SetStatusBarMessage(attachedCommand.Description);

            if(parameter is Button button)
            {
                ExecuteForSource(button);
            }

            else if(parameter is ToggleButton toggleButton)
            {
                ExecuteForSource(toggleButton);
            }

            else if(parameter is RadioButton radioButton)
            {
                ExecuteForSource(radioButton);
            }
        }

        public void ExecuteForSource(Button parameter)
        {
            var Parameter = parameter as Button;
            var attachedCommand = Parameter.Command as RequeryBase;

            StatusBarMessaging.SetStatusBarMessage(attachedCommand.Description);
        }

        public void ExecuteForSource(ToggleButton parameter)
        {
            var Parameter = parameter as ToggleButton;
            var attachedCommand = Parameter.Command as RequeryBase;

            StatusBarMessaging.SetStatusBarMessage(attachedCommand.Description);
        }

        public void ExecuteForSource(RadioButton parameter)
        {
            var Parameter = parameter as RadioButton;
            var attachedCommand = Parameter.Command as RequeryBase;

            StatusBarMessaging.SetStatusBarMessage(attachedCommand.Description);
        }
    }
}
