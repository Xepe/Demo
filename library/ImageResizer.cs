using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace library
{
    public class ImageResizer
    {
        public enum VerticalAlign { Top, Middle, Bottom }
        public enum HorizontalAlign { Left, Middle, Right }
          
        public void Convert(string sourceFile, string targetFile, ImageFormat targetFormat, int height, int width, VerticalAlign valign, HorizontalAlign halign)
        {
            using (var img = Image.FromFile(sourceFile)) 
            {
                using (var targetImg = Convert(img, height, width, valign, halign))
                {
                    string directory = Path.GetDirectoryName(targetFile);
                    if (directory != null && !Directory.Exists(directory))
                        Directory.CreateDirec tory(directory);

                    if (Equals(ImageFormat.Jpeg, targetFormat))
                        SaveJpeg(targetFile, targetImg, 100);
                    else
                        targetImg.Save(targetFile, targetFormat);
                }
            }
        }
        /// <summary> 
        /// Saves an image as a jpeg image, with the given quality 
        /// </summary> 
        /// <param name="path">Path to which the image would be saved.</param> 
        /// <param name="img">An image type file.</param> 
        /// <param name="quality">An integer from 0 to 100, with 100 being the 
        /// highest quality</param> 
        public static void SaveJpeg(string path, Image img, int quality)
        {
            if (quality < 0 || quality > 100)
                throw new ArgumentOutOfRangeException("Quality must be between 0 and 100.", new ArgumentOutOfRangeException());

            // Encoder parameter for image quality 
            var qualityParam =
                new EncoderParameter(Encoder.Quality, quality);
            // Jpeg image codec 
            var jpegCodec = GetEncoderInfo("image/jpeg");

            var encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = qualityParam;

            img.Save(path, jpegCodec, encoderParams);
        }

        /// <summary> 
        /// Returns the image codec with the given mime type 
        /// </summary> 
        private static ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            // Get image codecs for all image formats 
            var codecs = ImageCodecInfo.GetImageEncoders();

            // Find the correct image codec 
            return codecs.FirstOrDefault(t => t.MimeType == mimeType);
        }

        public Image Convert(Image img, int height, int width, VerticalAlign valign, HorizontalAlign halign)
        {
            var result = new Bitmap(width, height);
            using (var grap = Graphics.FromImage(result))
            {
                grap.SmoothingMode = SmoothingMode.HighQuality;
                grap.InterpolationMode = InterpolationMode.HighQualityBicubic;

                var ratio = height / img.Height;
                var temp = (img.Width * ratio);

                if (temp == width)
                {
                    //no corrections are needed!
                    grap.DrawImage(img, 0, 0, width, height);
                    return result;
                }

                if (temp > width)
                {
                    //too width 
                    int overFlow = (temp - width);
                    if (halign == HorizontalAlign.Middle)
                    {
                        grap.DrawImage(img, 0 - overFlow / 2, 0, temp, height);
                    }
                    else if (halign == HorizontalAlign.Left)
                    {
                        grap.DrawImage(img, 0, 0, temp, height);
                    }
                    else if (halign == HorizontalAlign.Right)
                    {
                        grap.DrawImage(img, -overFlow, 0, temp, height);
                    }
                }
                else
                {
                    //too hight 
                    ratio = width / img.Width;
                    temp = (int)((float)img.Height * ratio);
                    int overFlow = (temp - height);
                    if (valign == VerticalAlign.Top)
                    {
                        grap.DrawImage(img, 0, 0, width, temp);
                    }
                    else if (valign == VerticalAlign.Middle)
                    {
                        grap.DrawImage(img, 0, -overFlow / 2, width, temp);
                    }
                    else if (valign == VerticalAlign.Bottom)
                    {
                        grap.DrawImage(img, 0, -overFlow, width, temp);
                    }
                }
            }
            return result;
        }
    }
}
