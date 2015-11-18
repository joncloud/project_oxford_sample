using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOxfordCamera
{
    public class EmotionAnalysisResult
    {
        public EmotionAnalysisResult()
        {
            Indexes = new Dictionary<string, float>();
        }

        public Rectangle Hitbox { get; set; }
        public Dictionary<string, float> Indexes { get; private set; }
    }
}
