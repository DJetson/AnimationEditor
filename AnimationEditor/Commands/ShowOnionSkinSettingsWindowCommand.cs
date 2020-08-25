using AnimationEditor.BaseClasses;
using AnimationEditor.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AnimationEditor.Commands
{
    public class ShowOnionSkinSettingsWindowCommand : RequeryBase
    {
        public override void Execute(object parameter)
        {
            OnionSkinSettingsWindow newWindow = new OnionSkinSettingsWindow() { Owner = parameter as Window };
            newWindow.ShowDialog();
        }
    }
}
