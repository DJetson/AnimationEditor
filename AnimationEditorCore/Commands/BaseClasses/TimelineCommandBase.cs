using AnimationEditorCore.BaseClasses;
using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace AnimationEditorCore.Commands.BaseClasses
{
    public abstract class TimelineCommandBase : RequeryBase
    {
        protected int GetIndexForFrameNavigation(int currentIndex, int indexCount, FrameNavigation navigation)
        {
            var insertAtIndex = 0;

            switch (navigation)
            {
                case FrameNavigation.Start:
                    insertAtIndex = 0;
                    break;
                case FrameNavigation.Previous:
                    insertAtIndex = currentIndex;
                    break;
                case FrameNavigation.Next:
                    insertAtIndex = currentIndex + 1;
                    break;
                case FrameNavigation.End:
                    insertAtIndex = indexCount;
                    break;
            }

            return insertAtIndex;
        }
    }
}
