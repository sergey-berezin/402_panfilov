using System;
using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using YOLO;
using Contract;

namespace WebServer
{
    public class YoloImageResult : YoloResult
    {
        CroppedBitmap image;

        /// <summary>
        /// 
        /// </summary>
        public CroppedBitmap Image => image;


        public YoloImageResult(YoloResult item) : base(item.BBox, item.Label, item.Confidence)
        {
            filename = item.Filename;
            SetImage();
        }

        void SetImage()
        {
            var uri = new Uri(filename, UriKind.RelativeOrAbsolute);
            var fileImage = new BitmapImage(uri);
            fileImage.Freeze();
            var rect = new Int32Rect((int)BBox[0], (int)BBox[1], (int)(BBox[2] - BBox[0]), (int)(BBox[3] - BBox[1]));
            image = new CroppedBitmap(fileImage, rect);
            image.Freeze();
        }

        public static byte[] ConvertBitmapSourceToByte(BitmapSource bitmapSource)
        {
            byte[] byteImage;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();

            encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
            using (MemoryStream ms = new MemoryStream())
            {
                encoder.Save(ms);
                byteImage = ms.ToArray();
            }

            return byteImage;
        }
    }

    public class YoloItem : Item
    {
        public YoloItem(YoloResult res)
        {
            var imageRes = new YoloImageResult(res);

            X1 = imageRes.BBox[0];
            X2 = imageRes.BBox[1];
            Y1 = imageRes.BBox[2];
            Y2 = imageRes.BBox[3];
            Image = YoloImageResult.ConvertBitmapSourceToByte(imageRes.Image);
            Label = imageRes.Label;
            Confidence = imageRes.Confidence;
        }
    }
}
