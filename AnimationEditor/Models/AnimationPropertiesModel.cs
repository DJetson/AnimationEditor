using AnimationEditor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationEditor.Models
{
    [Serializable]
    public class AnimationPropertiesModel : IAnimationPropertiesModel
    {
        private double _FramesPerSecond = 24;
        public double FramesPerSecond => _FramesPerSecond;
    }
}
