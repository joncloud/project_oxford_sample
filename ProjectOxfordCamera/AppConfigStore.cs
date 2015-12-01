using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectOxfordCamera
{
    public class AppConfigStore
    {
        private static readonly string _configPath;

        static AppConfigStore()
        {
            string directory = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "ProjectOxfordCamera");

            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            _configPath = Path.Combine(directory, $"{typeof(AppConfig).Name}.json");
        }

        public AppConfig Load()
        {
            if (!File.Exists(_configPath))
            {
                return Save(new AppConfig());
            }

            try
            { 
                using (StreamReader reader = new StreamReader(_configPath))
                using (JsonTextReader jsonReader = new JsonTextReader(reader))
                {
                    JsonSerializer serializer = new JsonSerializer();
                    return serializer.Deserialize<AppConfig>(jsonReader);
                }
            }
            catch
            {
                string target = Path.ChangeExtension(".json", ".json.bak");
                if (File.Exists(target)) { File.Delete(target); }
                File.Move(_configPath, target);

                return Save(new AppConfig());
            }
        }

        public AppConfig Save(AppConfig config)
        {
            using (StreamWriter writer = new StreamWriter(_configPath))
            using (JsonTextWriter jsonWriter = new JsonTextWriter(writer))
            {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Serialize(jsonWriter, config);
                return config;
            }
        }
    }
}
