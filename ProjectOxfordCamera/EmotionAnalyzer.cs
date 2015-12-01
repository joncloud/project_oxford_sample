using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOxfordCamera
{
    public class EmotionAnalyzer
    {
        private AppConfig _config;

        public EmotionAnalyzer(AppConfig config)
        {
            _config = config;
        }

        public async Task<IEnumerable<EmotionAnalysisResult>> AnalyzeAsync(Image image)
        {
            List<EmotionAnalysisResult> results = new List<EmotionAnalysisResult>();

            using (Stream buffer = new MemoryStream())
            {
                image.Save(buffer, ImageFormat.Jpeg);
                buffer.Seek(0, SeekOrigin.Begin);

                using (HttpClient client = new HttpClient())
                {
                    client.BaseAddress = new Uri("https://api.projectoxford.ai/emotion/v1.0/");
                    client.DefaultRequestHeaders.Add("Ocp-Apim-Subscription-Key", _config.OxfordSubscriptionKey);

                    var requestContent = new StreamContent(buffer);

                    requestContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                    HttpResponseMessage message = await client.PostAsync(
                        "recognize",
                        requestContent);

                    message.EnsureSuccessStatusCode();

                    string content = await message.Content.ReadAsStringAsync();

                    JArray array = JsonConvert.DeserializeObject<JArray>(content);

                    foreach (JObject jobject in array)
                    {
                        JObject faceRectangle = jobject.Value<JObject>("faceRectangle");
                        EmotionAnalysisResult result = new EmotionAnalysisResult
                        {
                            Hitbox = new Rectangle(
                                faceRectangle.Value<int>("left"),
                                faceRectangle.Value<int>("top"),
                                faceRectangle.Value<int>("width"),
                                faceRectangle.Value<int>("height"))
                        };
                        JObject scores = jobject.Value<JObject>("scores");
                        foreach (var property in scores.Properties())
                        {
                            result.Indexes.Add(property.Name, property.Value.Value<float>());
                        }
                        results.Add(result);
                    }
                    
                }
            }

            return results;
        }
    }
}
