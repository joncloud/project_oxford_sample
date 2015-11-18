using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOxfordCamera
{
    public class OpenCVImageSource : IImageSource
    {
        private Capture _capture;

        public OpenCVImageSource()
        {
            _capture = new Emgu.CV.Capture();
        }

        public CaptureFrequency Frequency { get { return CaptureFrequency.Interval; } }

        public Image Capture(string path)
        {
            var frame = _capture.RetrieveBgrFrame();
            return frame.Bitmap;
        }

        public void Start()
        {
            _capture.Start();
        }

        public void Stop()
        {
            _capture.Stop();
        }
    }
}
