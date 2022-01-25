using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Yetibyte.Twitch.ChatNx.QueueReceiver.Models
{
    [Serializable]
    public record OptionsData
    {
        public string BackgroundColor { get; init; }
        public string DividerColor { get; init; }
        public string HeaderTextColor { get; init; }
        public string CurrentItemBorderColor { get; init; }
        public string HistoryItemTextColor { get; init; }
        public string QueueItemTextColor { get; init; }
        public float TimeStampScalePercentage { get; init; }
        public int HeaderTextSize { get; init; }
        public int QueueItemTextSize { get; init; }
        public int HistoryItemTextSize { get; init; }
        public int Port { get; init; }

        public void Save(Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            string json = JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true });

            using StreamWriter streamWriter = new StreamWriter(stream);
            streamWriter.Write(json);

        }

        public static OptionsData Load(Stream stream)
        {
            if (stream is null)
            {
                throw new ArgumentNullException(nameof(stream));
            }

            using StreamReader streamReader = new StreamReader(stream);

            string json = streamReader.ReadToEnd();

            return JsonSerializer.Deserialize<OptionsData>(json);
        }
    }
}
