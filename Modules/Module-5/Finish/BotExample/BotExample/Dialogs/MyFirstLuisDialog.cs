using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Bot.Builder.Location;
using System.Web.Configuration;
using System.Net.Http;
using Newtonsoft.Json;
using System.Globalization;
using Microsoft.ProjectOxford.Emotion;
using Microsoft.ProjectOxford.Emotion.Contract;
using Microsoft.ProjectOxford.Vision;
using Microsoft.ProjectOxford.Vision.Contract;

namespace BotExample.Dialogs
{
    [Serializable]
    [LuisModel("d3acb5ba-a994-4f04-b0a4-d4edb43283e0", "f7c4805b5b9b4a8eb902c02b3b7482c2")]
    public class MyFirstLuisDialog : LuisDialog<object>
    {
        private Dictionary<string, Character> characters = new Dictionary<string, Character>()
        {
            {"leonard", new Character("Leonard Hofstadter", "Experimental Physicist" ,"Leonard is my friend" ,"http://vignette3.wikia.nocookie.net/thebigbangtheory/images/b/bf/250px-Leonard.jpg/revision/latest?cb=20120917154638&path-prefix=es")},
            {"penny", new Character("Penny", "Aspiring Actress" ,"Leonard is my friend" ,"https://upload.wikimedia.org/wikipedia/en/4/41/Penny_bigbangtheory.jpg")},
            {"raj", new Character("Rajesh Koothrappali", "Particle Astrophysicist" ,"Leonard is my friend" ,"http://vignette2.wikia.nocookie.net/bigbangtheory/images/9/97/Raj.jpg/revision/20110809180135")},
            {"howard", new Character("Howard Wolowitz", "Aerospace Engineer" ,"Leonard is my friend" ,"http://vignette2.wikia.nocookie.net/bigbangtheory/images/6/6a/Howardwolowitz.jpg/revision/latest/top-crop/width/240/height/240?cb=20100425200930")}
        };
        private Dictionary<string, string> plans = new Dictionary<string, string>()
        {
            { "monday", "get a Thai takeout!" },
            { "tuesday", "have a cheeseburger at the Cheesecake Factory!" },
            { "wednesday", "play Halo with your friends!" },
            { "thursday", "have a nice slice of pizza!" },
            { "friday", "get a chinese takeaway!" },
            { "saturday", "do some of your laundry!" },
            { "sunday", "relax at home and do some physics!" }
        };
        [LuisIntent("Welcome")]
        public async Task Welcome(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Hi, I´m SheldonBot");
            await context.PostAsync("I can talk about my friends, weekly night plans, recognize pictures or emotions, what would you like to know about?");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Friends")]
        public async Task Friends(IDialogContext context, LuisResult result)
        {
            string friend = "";
            EntityRecommendation friendEntRec;

            if (result.TryFindEntity("Friend", out friendEntRec))
            {
                friend = friendEntRec.Entity;
                context.PrivateConversationData.SetValue("friend", friend);

                if (characters.ContainsKey(friend.ToLower()))
                {
                    var reply = context.MakeMessage();
                    reply.Attachments = new List<Attachment>
                {
                   CreateCharacterCard(context, characters[friend.ToLower()])
                };
                    await context.PostAsync(reply);
                    context.Wait(MessageReceived);
                }
                else
                {
                    await context.PostAsync($"Sorry, {friend} isn´t in my friends list");
                    await context.PostAsync(CreateCharactersCarousel(context));
                    context.Wait(MessageReceived);
                }
            }
            else {
                await context.PostAsync($"Here are some of my friends");
                await context.PostAsync(CreateCharactersCarousel(context));
                context.Wait(MessageReceived);
            }
        }

        public IMessageActivity CreateCharactersCarousel(IDialogContext context) {
            var reply = context.MakeMessage();
            reply.AttachmentLayout = "carousel";
            reply.Attachments = characters.Select(c => CreateCharacterCard(context, c.Value)).ToList();

            return reply;
        }

        public Attachment CreateCharacterCard(IDialogContext context, Character character)
        {
            HeroCard hc = new HeroCard()
            {
                Title = character.Name,
                Subtitle = character.Profession,
                Images = new List<CardImage>()
                {
                    new CardImage()
                    {
                        Url = character.Imageurl
                    }
                }
            };

            return hc.ToAttachment();
        }

        [LuisIntent("Plans")]
        public async Task Plans(IDialogContext context, LuisResult result) {
            string datetime = "";
            EntityRecommendation dateEntRec;
            DateTime day = new DateTime();

            if (result.TryFindEntity("builtin.datetime.time", out dateEntRec))
            {
                datetime = dateEntRec.Resolution["time"];
                day = DateTime.Parse(datetime.Remove(10, 3));
            }

            else if(result.TryFindEntity("builtin.datetime.date", out dateEntRec))
            {
                datetime = dateEntRec.Resolution["date"];
                day = DateTime.Parse(datetime);
            }

            else
            {
                await context.PostAsync($"Sorry I can´t find the day you are looking to have a plan");
                context.Done("");
            }

            await context.PostAsync($"On a {day.DayOfWeek.ToString().ToLower()} you should {plans[day.DayOfWeek.ToString().ToLower()]} ");
            context.Done("");

        }

        [LuisIntent("RecognizeEmotion")]
        public async Task Emotion(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Send me a picture please");
            context.Wait(AnalyzeEmotion);
        }

        public async Task AnalyzeEmotion(IDialogContext context, IAwaitable<IMessageActivity> argument) {

            const string emotionApiKey = "13fae55675884e98b4b9f013d2340c7b";
            var activity = await argument;
            var received = activity.Text;
            
            //Emotion SDK objects that take care of the hard work
            EmotionServiceClient emotionServiceClient = new EmotionServiceClient(emotionApiKey);
            Emotion[] emotionResult = null;

            if (activity.Attachments?.Any() == true)
            {
                var photoUrl = activity.Attachments[0].ContentUrl;
                var client = new HttpClient();
                var photoStream = await client.GetStreamAsync(photoUrl);
                
                try
                {
                    emotionResult = await emotionServiceClient.RecognizeAsync(photoStream);
                }

                catch (Exception e)
                {
                    emotionResult = null;
                }
            }
            else
            {
                try
                {
                    emotionResult = await emotionServiceClient.RecognizeAsync(activity.Text);
                }

                catch (Exception e)
                {
                    emotionResult = null;
                }
            }

            var reply = context.MakeMessage();
            reply.Text = "Could not find a face, or something went wrong. " + "Try sending me a photo with a face";

            if (emotionResult != null)
            {
                Microsoft.ProjectOxford.Common.Contract.EmotionScores emotionScores = new Microsoft.ProjectOxford.Common.Contract.EmotionScores();
                emotionScores = emotionResult[0].Scores;

                //Retrieve list of emotions for first face detected and sort by emotion score (desc)
                IEnumerable<KeyValuePair<string, float>> emotionList = new Dictionary<string, float>()
                {
                    { "angry", emotionScores.Anger},
                    { "contemptuous", emotionScores.Contempt },
                    { "disgusted", emotionScores.Disgust },
                    { "frightened", emotionScores.Fear },
                    { "happy", emotionScores.Happiness},
                    { "neutral", emotionScores.Neutral},
                    { "sad", emotionScores.Sadness },
                    { "surprised", emotionScores.Surprise}
                }
                .OrderByDescending(kv => kv.Value)
                .ThenBy(kv => kv.Key)
                .ToList();

                KeyValuePair<string, float> topEmotion = emotionList.ElementAt(0);
                string topEmotionKey = topEmotion.Key;
                float topEmotionScore = topEmotion.Value;


                reply.Text = "I found a face! I am " + (int)(topEmotionScore * 100) + "% sure the person seems " + topEmotionKey;
                
            }

            await context.PostAsync(reply);
            context.Wait(MessageReceived);
        }

        [LuisIntent("DescribePicture")]
        public async Task Description(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Send me a picture please");
            context.Wait(DescribeImage);
        }

        public async Task DescribeImage(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            const string visionApiKey = "292eb1a043b6420fb56b2026f4cacd2a";

            //Vision SDK classes
            VisionServiceClient visionClient = new VisionServiceClient(visionApiKey);
            VisualFeature[] visualFeatures = new VisualFeature[] {
                                        VisualFeature.Adult, //recognize adult content
                                        VisualFeature.Categories, //recognize image features
                                        VisualFeature.Description //generate image caption
                                        };
            AnalysisResult analysisResult = null;


            
            var activity = await argument;
            var received = activity.Text;

            

            if (activity.Attachments?.Any() == true)
            {
                var photoUrl = activity.Attachments[0].ContentUrl;
                var client = new HttpClient();
                var photoStream = await client.GetStreamAsync(photoUrl);

                try
                {
                    analysisResult = await visionClient.AnalyzeImageAsync(photoStream, visualFeatures);
                }

                catch (Exception e)
                {
                    analysisResult = null;
                }
            }
            else
            {
                try
                {
                    analysisResult = await visionClient.AnalyzeImageAsync(activity.Text, visualFeatures);
                }

                catch (Exception e)
                {
                    analysisResult = null;
                }
            }

            var reply = context.MakeMessage();
            reply.Text = "Did you upload an image? I'm more of a visual person. " + "Try sending me an image or an image url";

            if (analysisResult != null)
            {

                if (analysisResult.Adult.IsAdultContent)
                {
                    reply.Text = "I don´t like adult content images";
                }
                else
                {
                    string imageCaption = analysisResult.Description.Captions[0].Text;
                    reply.Text = "I think it's " + imageCaption;
                }
            }

            await context.PostAsync(reply);
            context.Wait(MessageReceived);
        }
        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Sorry, I did not understand '{result.Query}'");
            context.Wait(MessageReceived);
        }




    }
}