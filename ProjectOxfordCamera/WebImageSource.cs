using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOxfordCamera
{
    public class WebImageSource : IImageSource
    {
        public CaptureFrequency Frequency { get { return CaptureFrequency.Once; } }

        public Image Capture(string path)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(path);
            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            {
                return Image.FromStream(stream);
            }
        }

        public void Start()
        {

        }

        public void Stop()
        {
        }
    }
}
