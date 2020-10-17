using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AnimationEditorCore.Utilities
{
    public static class LayerOrdering
    {
        /// <summary>
        /// Lifts up the entire ZIndex stack beginning at the specified index. 
        /// E.g. CreateSpaceAtZIndex(3) = |1|2|3|4|5| -> |1|2| |4|5|6|
        /// </summary>
        /// <param name="layers"></param>
        /// <param name="zIndex"></param>
        public static void CreateSpaceAtZIndex(List<LayerViewModel> layers, int zIndex)
        {
            var lifted = layers.Where(e => e.ZIndex >= zIndex);

            foreach(var layer in lifted)
            {
                layer.ZIndex += 1;
            }
        }

        public static void CreateSpaceAfterZIndex(List<LayerViewModel> layers, int zIndex)
        {
            var lifted = layers.Where(e => e.ZIndex > zIndex);

            foreach (var layer in lifted)
            {
                layer.ZIndex += 1;
            }
        }

        public static int GetTopZIndex(List<LayerViewModel> layers)
        {
            return layers.Select(e => e.ZIndex).Max();
        }

        public static int GetBottomZIndex(List<LayerViewModel> layers)
        {
            return layers.Select(e => e.ZIndex).Min();
        }

        public static bool IsZIndexOccupied(List<LayerViewModel> layers, int zIndex)
        {
            if (layers.Select(e => e.ZIndex).Contains(zIndex))
                return true;

            return false;
        }

        public static int GetNextLayerZIndexAbove(List<LayerViewModel> layers, int start)
        {
            return GetAllLayersAboveZIndex(layers, start)?.Select(e => e.ZIndex)?.Min() ?? -1;
        }

        public static List<LayerViewModel> GetAllLayersAboveZIndex(List<LayerViewModel> layers, int start)
        {
            return layers.Where(e => e.ZIndex > start).ToList();
        }

        public static void ConsolidateZIndices(List<LayerViewModel> layers)
        {
            for(int i = 0; i < layers.Select(e => e.ZIndex).Distinct().Count(); i++)
            {
                if (layers.Select(e => e.ZIndex).Contains(i))
                    continue;

                int nextZ = GetNextLayerZIndexAbove(layers, i);

                if (nextZ == -1)
                    break;

                int delta = i - nextZ;

                GetAllLayersAboveZIndex(layers,i).ForEach(e => e.ZIndex += delta);
            }
        }
    }
}
