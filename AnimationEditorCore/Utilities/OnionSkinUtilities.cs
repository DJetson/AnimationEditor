using AnimationEditorCore.Interfaces;
using AnimationEditorCore.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Ink;

namespace AnimationEditorCore.Utilities
{
    public static class OnionSkinUtilities
    {
        public static List<IFrameViewModel> GetAllLayerFramesAtIndex(List<LayerViewModel> layers, int index)
        {
            var frames = new List<IFrameViewModel>();

            foreach (var layer in layers)
            {
                foreach (var frame in layer.Frames)
                {
                    if (frame.Order == index)
                        frames.Add(frame);
                }
            }
            return frames;
        }

        public static StrokeCollection FlattenFrames(List<IFrameViewModel> frames, bool ExcludeHiddenLayers = false)
        {
            var strokes = new StrokeCollection();
            List<IFrameViewModel> flattenFrames = frames;
            if (ExcludeHiddenLayers)
            {
                flattenFrames = frames.Where(e => e.LayerViewModel.IsVisible).ToList();
            }

            foreach (var frame in flattenFrames.OfType<KeyFrameViewModel>())
            {
                strokes.Add(frame.StrokeCollection);
            }

            return strokes;
        }

        public static List<StrokeCollection> GetAllSucceedingOnionSkins(List<LayerViewModel> layers, int currentIndex, int onionSkinCount = 1)
        {
            var nextOnionSkins = new List<StrokeCollection>();
            double opacityFalloff = Properties.Settings.Default.NextFrameSkinOpacityFalloff;
            double currentOpacity = 1;

            for (int i = 0; i < onionSkinCount; i++)
            {
                currentOpacity *= opacityFalloff;
                //Start at the frame after the current one
                var onionSkinFrameIndex = currentIndex + 1 + i;
                var nextFrame = GetAllLayerFramesAtIndex(layers, onionSkinFrameIndex);

                if (nextFrame == null)
                    return nextOnionSkins;

                var strokes = FlattenFrames(nextFrame, true).Clone();
                foreach (var item in strokes)
                {
                    item.DrawingAttributes.IsHighlighter = false;
                    item.DrawingAttributes.Color = System.Windows.Media.Color.FromArgb((byte)(255 * currentOpacity), 0, 255, 0);
                }
                nextOnionSkins.Add(strokes);
            }


            return nextOnionSkins;
        }

        public static List<StrokeCollection> GetAllPrecedingOnionSkins(List<LayerViewModel> layers, int currentIndex, int onionSkinCount = 1)
        {
            var previousOnionSkins = new List<StrokeCollection>();
            double opacityFalloff = Properties.Settings.Default.PreviousFrameSkinOpacityFalloff;
            double currentOpacity = 1;

            for (int i = 0; i < onionSkinCount; i++)
            {
                currentOpacity *= opacityFalloff;

                //Start at the frame before the current one
                var onionSkinFrameIndex = currentIndex - 1 - i;

                var previousFrame = GetAllLayerFramesAtIndex(layers, onionSkinFrameIndex);

                if (previousFrame == null)
                    return previousOnionSkins;

                var strokes = FlattenFrames(previousFrame, true).Clone();
                foreach (var item in strokes)
                {
                    item.DrawingAttributes.IsHighlighter = false;
                    item.DrawingAttributes.Color = System.Windows.Media.Color.FromArgb((byte)(255 * currentOpacity), 255, 0, 0);
                }
                previousOnionSkins.Add(strokes);
            }

            return previousOnionSkins;
        }
    }
}
