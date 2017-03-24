using Microsoft.ProjectOxford.Vision;
using System;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;

namespace BotExample.Services
{
    public class VisionService
    {
        private static readonly string ApiKey = ConfigurationManager.AppSettings["ComputerVisionAPIKey"];
        private VisionServiceClient client = new VisionServiceClient(ApiKey);

        public async Task<string> GetDescriptionAsync(Uri uri)
        {
            var stream = await new HttpService().GetStreamAsync(uri);

            VisualFeature[] visualFeatures = new VisualFeature[] 
            {
                VisualFeature.Adult,        //recognize adult content
                VisualFeature.Description   //generate image caption
            };

            var result = await client.AnalyzeImageAsync(stream, visualFeatures);

            if (result.Adult.IsAdultContent)
            {
                return "I don't like images with adult content!";
            }
            else
            {
                var description = result?.Description?.Captions.FirstOrDefault()?.Text;

                return !string.IsNullOrEmpty(description) ?
                    $"I see the following: {description}" : "Couldn't find a description for it";
            }
        }
    }
}