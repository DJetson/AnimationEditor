using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.Views;
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
