using System;
using System.Windows;
using System.Windows.Media.Imaging;

namespace YOLO
{
    public class YoloResult
    {
        CroppedBitmap image;
        string filename;

        /// <summary>
        /// x1, y1, x2, y2 in page coordinates.
        /// <para>left, top, right, bottom.</para>
        /// </summary>
        public float[] BBox { get; }

        /// <summary>
        /// The Bbox category.
        /// </summary>
        public string Label { get; }

        /// <summary>
        /// Confidence level.
        /// </summary>
        public float Confidence { get; }

        /// <summary>
        /// 
        /// </summary>
        public CroppedBitmap Image => image;

        /// <summary>
        /// 
        /// </summary>
        public string Filename => filename;


        public YoloResult(float[] bbox, string label, float confidence)
        {
            BBox = bbox;
            Label = label;
            Confidence = confidence;
        }


        public void SetImage(string _filename, BitmapSource source)
        {
            filename = _filename;

            var rect = new Int32Rect((int)BBox[0], (int)BBox[1], (int)(BBox[2] - BBox[0]), (int)(BBox[3] - BBox[1]));
            image = new CroppedBitmap(source, rect);
            image.Freeze();
        }
    }
}
