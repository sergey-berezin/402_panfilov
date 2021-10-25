namespace YOLO
{
    public class YoloResult
    {
        protected string filename;
        float[] bBox;
        string label;
        float confidence;

        /// <summary>
        /// x1, y1, x2, y2 in page coordinates.
        /// <para>left, top, right, bottom.</para>
        /// </summary>
        public float[] BBox => bBox;

        /// <summary>
        /// The Bbox category.
        /// </summary>
        public string Label => label;

        /// <summary>
        /// Confidence level.
        /// </summary>
        public float Confidence => confidence;

        /// <summary>
        /// 
        /// </summary>
        public string Filename => filename;


        public YoloResult(float[] _bBox, string _label, float _confidence)
        {
            bBox = _bBox;
            label = _label;
            confidence = _confidence;
        }

        public void SetFilename(string _filename)
        {
            filename = _filename;
        }
    }
}
