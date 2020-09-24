﻿using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace AnimationEditorCore.Commands
{
    public class ShowOnionSkinSettingsWindowCommand : RequeryBase
    {
        public override string DisplayName => "Show Onion Skin Settings";

        public override void Execute(object parameter)
        {
            OnionSkinSettingsWindow newWindow = new OnionSkinSettingsWindow() { Owner = parameter as Window };
            newWindow.ShowDialog();
        }
    }
}
