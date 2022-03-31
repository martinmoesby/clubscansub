using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Text;

namespace ClubScansub.Utility
{
    public class ImageUtils
    {

        public static Bitmap ResizeImage(Image image, ImageOptions options)
        {
            return ResizeImage(image, options.MaxWidth, options.MaxHeight);
        }

        public static Bitmap ResizeImage(Image image, int maxWidth, int maxHeight)
        {
            //calculate the ratio
            double dbl = 0.0; // (double)image.Width / (double)image.Height;
            int newWidth = 0, newHeight = 0;

            if (image.Width > image.Height)
            {
                // Landscape
                dbl = (double)maxWidth / (double)image.Width;

                newWidth = maxWidth;
                newHeight = (int)(image.Height * dbl);
            } else

            {
                // portrait

                dbl = (double)(maxHeight) / (double)image.Height;

                newWidth = (int)(image.Width * dbl); 
                newHeight = maxHeight;
            }


            var destRect = new Rectangle(0, 0, newWidth, newHeight);

            Bitmap destImage = new Bitmap(image,newWidth, newHeight);

            //set height of image to boxHeight and check if resulting width is less than boxWidth, 
            //else set width of image to boxWidth and calculate new height

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    //wrapMode.SetWrapMode(WrapMode.Clamp);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
    }
}
