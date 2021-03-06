﻿using Emgu.CV;
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

        private EmotionAnalysisResult[] _results;
        private EmotionAnalyzer _analyzer;

        public Form1()
        {
            InitializeComponent();
            Load += Form1_Load;
            pictureBox.Paint += PictureBox1_Paint;
            pictureBox.MouseUp += PictureBox1_MouseUp;
        }

        private IEnumerable<Control> WorkingControls
        {
            get
            {
                yield return buttonAnalyze;
                yield return buttonCapture;
                yield return comboBoxMode;
                yield return textBoxPath;
            }
        }

        private void comboBoxMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_imageSource != null)
            {
                _imageSource.Stop();
            }
            _results = new EmotionAnalysisResult[0];
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

            AppConfigStore store = new AppConfigStore();
            AppConfig config = store.Load();
            if (string.IsNullOrWhiteSpace(config.OxfordSubscriptionKey))
            {
                buttonConfig_Click(sender, e);
                if (_analyzer == null)
                {
                    MessageBox.Show("Settings are required.");
                    Environment.Exit(1);
                }
            }
            else
            {
                _analyzer = new EmotionAnalyzer(config);
            }
        }

        private void PictureBox1_MouseUp(object sender, MouseEventArgs e)
        {
            if (_results == null)
            {
                return;
            }

            foreach (EmotionAnalysisResult result in _results)
            {
                if (result.Hitbox.Contains(e.X, e.Y))
                {
                    MessageBox.Show(string.Join(Environment.NewLine, result.Indexes.OrderByDescending(p => p.Value).Select(p => $"{p.Key}: {p.Value}")));
                }
            }
        }

        private void PictureBox1_Paint(object sender, PaintEventArgs e)
        {
            if (_results == null) { return; }
            using (Pen pen = new Pen(Brushes.Yellow, 2))
            {
                foreach (Rectangle rectangle in _results.Select(r => r.Hitbox))
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
            foreach (Control control in WorkingControls)
            {
                control.Enabled = false;
            }
            _timer.Stop();

            try
            {
                IEnumerable<EmotionAnalysisResult> results = await _analyzer.AnalyzeAsync(_copy);

                _results = results.ToArray();
                
                pictureBox.Invalidate();

                buttonSave.Enabled = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                foreach (Control control in WorkingControls)
                {
                    control.Enabled = true;
                }
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

        private void buttonConfig_Click(object sender, EventArgs e)
        {
            using (ConfigEditor editor = new ConfigEditor())
            {
                DialogResult result = editor.ShowDialog();

                if (result == DialogResult.OK)
                {
                    _analyzer = new EmotionAnalyzer(editor.Config);
                }
            }
        }

        private void buttonSave_Click(object sender, EventArgs e)
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    DateTime date = DateTime.Now;
                    string name = Path.Combine(dialog.SelectedPath, $"{date.Year:0000}{date.Month:00}{date.Day:00}{date.Hour:00}{date.Minute:00}{date.Second:00}");

                    _copy.Save($"{name}.png", ImageFormat.Png);
                    string results = JsonConvert.SerializeObject(_results);
                    File.WriteAllText($"{name}.json", results);
                }
            }
        }

        private void buttonLoad_Click(object sender, EventArgs e)
        {
            using (OpenFileDialog dialog = new OpenFileDialog())
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string json = Path.ChangeExtension(dialog.FileName, ".json");
                    string png = Path.ChangeExtension(dialog.FileName, ".png");

                    if (!File.Exists(json) && !File.Exists(png))
                    {
                        MessageBox.Show("Missing JSON or PNG");
                        return;
                    }

                    string contents = File.ReadAllText(json);
                    _results = JsonConvert.DeserializeObject<EmotionAnalysisResult[]>(contents);

                    _copy = Image.FromFile(png);
                    pictureBox.Image = (Image)_copy.Clone();
                    pictureBox.Invalidate();
                    buttonSave.Enabled = true;
                }
            }
        }
    }
}
