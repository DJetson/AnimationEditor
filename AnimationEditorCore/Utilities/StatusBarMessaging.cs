using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationEditorCore.Utilities
{
    public class StatusBarMessaging : ViewModelBase
    {
        private static StatusBarMessaging _Instance = null;
        public static StatusBarMessaging Instance
        {
            get
            {
                if (_Instance == null)
                    _Instance = new StatusBarMessaging();
                return _Instance;
            }
        }

        private string _CurrentMessage;
        public string CurrentMessage
        {
            get { return _CurrentMessage; }
            set { _CurrentMessage = value; NotifyPropertyChanged(); }
        }

        public static void ClearStatusBarMessage()
        {
            Instance.CurrentMessage = String.Empty;
        }

        public static void SetStatusBarMessage(string message)
        {
            Instance.CurrentMessage = message;
        }
    }
}
