using Emgu.CV;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProjectOxfordCamera
{
    public partial class Form1 : Form
    {
        private Dictionary<string, IImageSource> _imageSources;
        private IImageSource _imageSource;
        private Timer _timer;
        private Image _copy;

        private Rectangle[] _rectangles;
        private string[] _indexes;

        public Form1()
        {
            InitializeComponent();
            Load += Form1_Load;
            pictureBox.Paint += PictureBox1_Paint;
            pictureBox.MouseUp += PictureBox1_MouseUp;
        }

        private void comboBoxMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_imageSource != null)
            {
                _imageSource.Stop();
            }
            _rectangles = new Rectangle[0];
            _imageSource = _imageSources[comboBoxMode.Text];
            _imageSource.Start();
            _timer.Start();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (DesignMode) { return; }

            _imageSources = new Dictionary<string, IImageSource>
            {
                { "Camera", new OpenCVImageSource() },
                { "File", new FileImageSource() },
                { "Web", new WebImageSource() }
            };
            
            _timer = new Timer { Interval = 300 };
            _timer.Tick += timer_Tick;
        }

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            for (int i = 0; i < _rectangles.Length; i++)
            {
                Rectangle rectangle = _rectangles[i];
                if (rectangle.Contains(e.X, e.Y))
                {
                    MessageBox.Show(_indexes[i]);
                }
            }
        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (_rectangles == null) { return; }
            using (Pen pen = new Pen(Brushes.Yellow, 2))
            {
                foreach (Rectangle rectangle in _rectangles)
                {

                    e.Graphics.DrawRectangle(pen, rectangle);
                }
            }
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            if (pictureBox.Image != null)
            {
                pictureBox.Image.Dispose();
            }

            Image image = _imageSource.Capture(textBoxPath.Text);
            _copy = (Image)image.Clone();
            //using (Stream stream = _imageSource.Capture(textBoxPath.Text))
            {
                //_copy = Image.FromStream(stream);

                pictureBox.Image = image;
            }

            if (_imageSource.Frequency == CaptureFrequency.Once)
            {
                _timer.Stop();
            }
        }

        private async void buttonAnalyze_Click(object sender, EventArgs e)
        {
            try
            {
                _timer.Stop();


                using (Stream buffer = new MemoryStream())
                {
                    _copy.Save(buffer, ImageFormat.Jpeg);
                    buffer.Seek(0, SeekOrigin.Begin);

                    using (HttpClient client = new HttpClient())
                    {
                        client.BaseAddress = new Uri("https://api.projectoxford.ai/emotion/v1.0/");
                        client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", "");

                        var requestContent = new StreamContent(buffer);

                        requestContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                        HttpResponseMessage message = await client.PostAsync(
                            "recognize",
                            requestContent);

                        message.EnsureSuccessStatusCode();

                        string content = await message.Content.ReadAsStringAsync();

                        JArray array = JsonConvert.DeserializeObject<JArray>(content);
                        List<Rectangle> rectangles = new List<Rectangle>();
                        List<string> indexes = new List<string>();
                        foreach (JObject jobject in array)
                        {
                            var r = jobject.Value<JObject>("faceRectangle");
                            rectangles.Add(new Rectangle(r.Value<int>("left"), r.Value<int>("top"), r.Value<int>("width"), r.Value<int>("height")));
                            indexes.Add(jobject.Value<JObject>("scores").ToString());
                        }

                        _rectangles = rectangles.ToArray();
                        _indexes = indexes.ToArray();
                        pictureBox.Invalidate();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        

        private enum CaptureFrequency
        {
            Once,
            Interval
        }

        private interface IImageSource
        {
            CaptureFrequency Frequency { get; }

            Image Capture(string path);

            void Start();
            void Stop();
        }

        private class FileImageSource : IImageSource
        {
            public CaptureFrequency Frequency { get { return CaptureFrequency.Once; } }

            public Image Capture(string path)
            {
                using (var stream = new FileStream(path, FileMode.Open, FileAccess.ReadWrite))
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

        private class OpenCVImageSource : IImageSource
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

        private class WebImageSource : IImageSource
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
}
