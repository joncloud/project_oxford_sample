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
            pictureBox.Image = image;

            if (_imageSource.Frequency == CaptureFrequency.Once)
            {
                buttonCapture.Text = "Capture";
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
                            var scores = jobject.Value<JObject>("scores");
                            StringBuilder index = new StringBuilder();
                            foreach (var property in scores.Properties())
                            {
                                index.AppendLine($"{property.Name}: {property.Value}");
                            }
                            indexes.Add(index.ToString());
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

        private void buttonCapture_Click(object sender, EventArgs e)
        {
            if (buttonCapture.Text == "Capture")
            {
                if (_imageSource.Frequency == CaptureFrequency.Interval)
                {
                    buttonCapture.Text = "Stop";
                }
                _imageSource.Start();
                _timer.Start();
            }
            else
            {
                buttonCapture.Text = "Capture";
                _imageSource.Stop();
                _timer.Stop();
            }
        }
    }
}
