using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace AnimationEditorCore.Utilities
{
    public class ImageUtilities
    {
        public static byte[] ImagePathToByteArray(string filepath)
        {
            return ImageToByteArray(Image.FromFile(filepath));
        }

        public static byte[] ImagePathToThumbnail(string filepath)
        {
            Image img = Image.FromFile(filepath);

            double maxSize = 640 * 480;
            double actSize = img.Width * img.Height;
            double sf = Math.Sqrt(maxSize / actSize);
            if (sf < 1.0)
            {
                int newWidth = (int)Math.Round(sf * img.Width);
                int newHeight = (int)Math.Round(sf * img.Height);

                Image resized = new System.Drawing.Bitmap(img, new Size(newWidth, newHeight));
                img.Dispose();
                img = resized;
            }

            return ImageToByteArray(img);
        }

        public static byte[] ImageToByteArray(Image imageIn)
        {
            MemoryStream ms = new MemoryStream();
            imageIn.Save(ms, System.Drawing.Imaging.ImageFormat.Gif);
            return ms.ToArray();
        }

        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            MemoryStream ms = new MemoryStream(byteArrayIn);
            Image returnImage = Image.FromStream(ms);
            return returnImage;
        }

        public static BitmapImage ByteArrayToBitmapImage(byte[] byteArrayIn)
        {
            if (byteArrayIn == null)
                return new BitmapImage();
            using (MemoryStream ms = new MemoryStream(byteArrayIn))
            {
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();

                return bitmapImage;
            }
        }
    }
}
