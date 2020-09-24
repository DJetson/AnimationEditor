﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationEditorCore.Interfaces
{
    public interface IMementoOriginator
    {
        //IMemento CurrentState { get; }

        IMemento SaveState();
        void LoadState(IMemento state);
    }
}
