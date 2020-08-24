using AnimationEditor.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnimationEditor.Models
{
    [Serializable]
    public class CanvasPropertiesModel : ICanvasPropertiesModel
    {
        private double _Width = 640;
        public double Width => _Width;

        private double _Height = 480;
        public double Height => _Height;
    }
}
