using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOxfordCamera
{
    public class FileImageSource : IImageSource
    {
        private Stream _stream;

        public CaptureFrequency Frequency { get { return CaptureFrequency.Once; } }

        public Image Capture(string path)
        {
            if (_stream != null)
            {
                _stream.Dispose();
                _stream = null;
            }

            using (var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite))
            {
                _stream = new MemoryStream();
                stream.CopyTo(_stream);
                _stream.Seek(0, SeekOrigin.Begin);
                return Image.FromStream(_stream);
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
