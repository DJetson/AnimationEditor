using AnimationEditorCore.ViewModels;
using BumpKit;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Media.Imaging;

namespace AnimationEditorCore.Utilities
{
    class AnimationUtilities
    {
        public static List<LayerViewModel> GetVisibleLayers(List<LayerViewModel> layers)
        {
            return layers.Where(e => e.IsVisible).ToList();
        }

        /// <summary>
        /// Get the index of the very last frame to be shown in all frames across the specified collection of layers
        /// </summary>
        /// <param name="layers">The layers whose frame indices will be evaluated</param>
        /// <param name="excludeHiddenLayers">Whether or not to ignore any layers in the collection for which IsVisible equals false</param>
        /// <returns>The index of the final frame of the entire collection of frames across all provided layers.</returns>
        public static int GetLastFrameIndex(List<LayerViewModel> layers, bool excludeHiddenLayers = false)
        {
            var selectedLayers = excludeHiddenLayers ? GetVisibleLayers(layers) : layers;

            if (selectedLayers.Count == 0)
                return 0;

            return selectedLayers.SelectMany(layer => layer.Frames).Select(frame => frame.Order).Max();
        }

        public static int GetFrameCount(List<LayerViewModel> layers, bool excludeHiddenLayers = false)
        {
            var selectedLayers = excludeHiddenLayers ? GetVisibleLayers(layers) : layers;

            return selectedLayers.Select(layer => layer.Frames.Count).Max();
        }

        public static List<StrokeCollection> FlattenFrames(List<LayerViewModel> layers, int startIndex = 0, int count = 0, bool excludeHiddenLayers = true)
        {
            var flattenedFrameStrokes = new List<StrokeCollection>();
            var selectedLayers = excludeHiddenLayers ? GetVisibleLayers(layers) : layers;

            var LastFrameIndex = GetLastFrameIndex(selectedLayers);
            
            if (LastFrameIndex == 0)
                return null;

            var FrameCount = GetFrameCount(selectedLayers);
            //if start index is outside the existing range of frame indices...
            if (startIndex > LastFrameIndex)
                throw new IndexOutOfRangeException($"Cannot Flatten Strokes. StartIndex:{startIndex} is not valid");

            //if count is 0, set count to the total number of frames from startIndex to the last frame
            if (count == 0)
                count = FrameCount - startIndex;

            //Iterate over all possible frame indexes in all layers up to the highest indexed frame in any layer
            for (int i = startIndex; i < count; i++)
            {
                flattenedFrameStrokes.Add(new StrokeCollection());
                //Iterate over all visible layers
                foreach (var layer in selectedLayers)
                {
                    //If the current layer contains a frame with the index currently being evaluated...
                    if (layer.Frames.Select(e => e.Order).Contains(i))
                    {
                        //Add the strokes from the frame at the current index from the current layer to the
                        //flattened stroke collection
                        flattenedFrameStrokes[i].Add(layer.Frames[i].StrokeCollection);
                    }
                }
            }

            return flattenedFrameStrokes;
        }

        //public void SaveAsGif(string filepath, InkCanvas canvas, List<LayerViewModel> layers, int startIndex = 0, int count = 0, bool excludeHiddenLayers = true)
        //{
        //    try
        //    {
        //        using (FileStream fs = new FileStream(filepath, FileMode.Create))
        //        {
        //            GifEncoder gEnc = new GifEncoder(fs);
        //            var frameBitmaps = this.TimelineViewModel.RenderFrameBitmaps(canvas);

        //            foreach (var frame in frameBitmaps)
        //            {
        //                gEnc.AddFrame(frame, 0, 0, new TimeSpan(100));
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        MessageBox.Show(e.Message, "Error", MessageBoxButton.OK);
        //    }
        //}

        public static List<System.Drawing.Image> RenderFrameBitmaps(InkCanvas canvas, List<LayerViewModel> layers, int startIndex = 0, int count = 0, bool excludeHiddenLayers = true )
        {
            var selectedLayers = excludeHiddenLayers ? GetVisibleLayers(layers) : layers;
            var flattenedLayers = FlattenFrames(selectedLayers, startIndex, count, excludeHiddenLayers);

            RenderTargetBitmap rtb = new RenderTargetBitmap((int)canvas.ActualWidth, (int)canvas.ActualHeight, 96, 96, new System.Windows.Media.PixelFormat());
            List<System.Drawing.Image> frameImages = new List<System.Drawing.Image>();
            foreach (var strokes in flattenedLayers)
            {
                canvas.Strokes = strokes;
                rtb.Render(canvas);

                var bitmap = new Bitmap(rtb.PixelWidth, rtb.PixelHeight, System.Drawing.Imaging.PixelFormat.Format32bppPArgb);

                var bitmapData = bitmap.LockBits(new Rectangle(System.Drawing.Point.Empty, bitmap.Size),
                    ImageLockMode.WriteOnly, bitmap.PixelFormat);

                rtb.CopyPixels(Int32Rect.Empty, bitmapData.Scan0,
                    bitmapData.Stride * bitmapData.Height, bitmapData.Stride);

                bitmap.UnlockBits(bitmapData);

                System.Drawing.Image newImage = bitmap;
                frameImages.Add(newImage);
            }

            return frameImages;
        }

        public static List<FrameViewModel> GetAllLayerFramesAtIndex(List<LayerViewModel> layers, int index)
        {
            var frames = new List<FrameViewModel>();

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
    }
}
