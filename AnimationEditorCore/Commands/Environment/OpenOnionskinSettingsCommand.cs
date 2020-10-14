using AnimationEditorCore.Commands.BaseClasses;
using AnimationEditorCore.Properties;
using AnimationEditorCore.Views;
using System.Windows;

namespace AnimationEditorCore.Commands.Environment
{
    public class OpenOnionskinSettingsCommand : RequeryBase
    {
        public override string Description => Resources.OpenOnionskinSettingsDescription;
        public override string ToolTip => Resources.OpenOnionskinSettingsToolTip;
        public override string DisplayName => "Show Onion Skin Settings";

        public override void Execute(object parameter)
        {
            OnionskinSettingsWindow newWindow = new OnionskinSettingsWindow() { Owner = parameter as Window };
            newWindow.ShowDialog();
        }
    }
}
