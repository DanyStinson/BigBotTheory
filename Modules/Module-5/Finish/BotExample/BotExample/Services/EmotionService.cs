using Microsoft.ProjectOxford.Emotion;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BotExample.Services
{
    public class EmotionService
    {
        private static readonly string ApiKey = ConfigurationManager.AppSettings["EmotionAPIKey"];
        private EmotionServiceClient client = new EmotionServiceClient(ApiKey);

        private static Dictionary<string, string> adjectives = new Dictionary<string, string>()
        {
            { "Anger", "angry" },
            { "Contempt", "contemptuous" },
            { "Disgust", "disgusted" },
            { "Fear", "scared" },
            { "Happiness", "happy" },
            { "Neutral", "neutral" },
            { "Sadness", "sad" },
            { "Surprise", "surprised" }
        };

        public async Task<string> GetEmotionsAsync(Uri uri)
        {
            var stream = await new HttpService().GetStreamAsync(uri);
            var result = await client.RecognizeAsync(stream);

            var emotions = result.Select(e => adjectives[e.Scores.ToRankedList().First().Key]);

            var count = emotions.Count();
            switch (count)
            {
                case 0:
                    return "I couldn't find a single person in the image";
                case 1:
                    return $"I see one person in the image looking {emotions.First()}";
                default:
                    var builder = new StringBuilder($"I see {count} people in the image and they look: ");
                    for (int i = 0; i < count; i++)
                    {
                        if (i == count - 1)
                        {
                            builder.Append(" & ");
                        }
                        else if (i != 0)
                        {
                            builder.Append(", ");
                        }
                        builder.Append(emotions.ElementAt(i));
                    }
                    return builder.ToString();
            }
        }
    }
}