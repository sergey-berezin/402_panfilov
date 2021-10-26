using System;
using System.Windows;
using System.Windows.Media.Imaging;
using YOLO;

namespace RecognizerVM
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
    }
}
