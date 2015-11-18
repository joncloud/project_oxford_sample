using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOxfordCamera
{
    public interface IImageSource
    {
        CaptureFrequency Frequency { get; }

        Image Capture(string path);

        void Start();
        void Stop();
    }
}
