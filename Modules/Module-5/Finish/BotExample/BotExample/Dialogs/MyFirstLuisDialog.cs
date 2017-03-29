using BotExample.Extensions;
using BotExample.Model;
using BotExample.Services;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Connector;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace BotExample.Dialogs
{
    [Serializable]
    public class MyFirstLuisDialog : LuisDialog<object>
    {
        private enum ProcessingChoice { Emotions, Description }

        public MyFirstLuisDialog(LuisService service) : base(service) { }

        [LuisIntent("Welcome")]
        public async Task Welcome(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Hi, I'm SheldonBot");
            await context.PostAsync("I can talk about my friends, weekly night plans, recognize pictures or emotions, what would you like me to?");
            context.Wait(MessageReceived);
        }

        [LuisIntent("Friends")]
        public async Task Friends(IDialogContext context, LuisResult result)
        {
            var client = new BigBangTheoryClient();

            // Did we get a friend name?
            EntityRecommendation friendEntRec;
            if (result.TryFindEntity("Friend", out friendEntRec))
            {
                // We got a name
                string friend = friendEntRec.Entity;
                var character = client.GetCharacter(friend);
                if (character != null)
                {
                    // We know the friend
                    await context.PostAsync($"This is what I can tell you about {character.Name}");
                    await context.PostAsync(character.ToMessage(context));
                }
                else
                {
                    // We don't know the friend
                    await context.PostAsync($"Sorry, {friend} isn't in my friends list");
                    var characters = client.GetAllCharacters();
                    await context.PostAsync(characters.ToMessage(context));
                }
            }
            else
            {
                // We weren't provided with any friend name
                await context.PostAsync($"Here are some of my friends");
                var characters = client.GetAllCharacters();
                await context.PostAsync(characters.ToMessage(context));
            }

            context.Wait(MessageReceived);
        }

        [LuisIntent("Plans")]
        public async Task Plans(IDialogContext context, LuisResult result)
        {
            string datetime = string.Empty;
            EntityRecommendation dateEntRec;
            if (result.TryFindEntity("builtin.datetime.date", out dateEntRec))
            {
                datetime = dateEntRec.Resolution["date"];
            }
            else if (result.TryFindEntity("builtin.datetime.time", out dateEntRec))
            {
                datetime = dateEntRec.Resolution["time"];
            }

            var dayOfWeek = datetime.GetDayOfWeek();

            var plan = new BigBangTheoryClient().GetPlan(dayOfWeek.ToString());
            await context.PostAsync($"On a {dayOfWeek} you should {plan} ");
            context.Wait(MessageReceived);
        }

        [LuisIntent("RecognizeEmotion")]
        public async Task Emotion(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Send me a picture please");
            context.Wait((c, a) => ProcessImageAsync(c, a, ProcessingChoice.Emotions));
        }

        [LuisIntent("DescribePicture")]
        public async Task Description(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Send me a picture please");
            context.Wait((c, a) => ProcessImageAsync(c, a, ProcessingChoice.Description));
        }

        private async Task ProcessImageAsync(IDialogContext context, IAwaitable<IMessageActivity> argument, ProcessingChoice choice)
        {
            var activity = await argument;
            Uri uri = null;

            if (activity.Attachments?.Any() == true)
            {
                uri = new Uri(activity.Attachments[0].ContentUrl);
            }
            else
            {
                uri = new Uri(activity.Text);
            }

            try
            {
                string reply = string.Empty;
                switch (choice)
                {
                    case ProcessingChoice.Description:
                        reply = await new VisionService().GetDescriptionAsync(uri);
                        break;
                    case ProcessingChoice.Emotions:
                        reply = await new EmotionService().GetEmotionsAsync(uri);
                        break;
                }
                await context.PostAsync(reply);
            }
            catch (Exception)
            {
                await context.PostAsync("Something went wrong while analyzing the image!");
            }

            context.Wait(MessageReceived);
        }

        [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Sorry, I did not understand '{result.Query}'");
            await context.PostAsync("I can talk about my friends, weekly night plans, recognize pictures or emotions, what would you like me to?");
            context.Wait(MessageReceived);
        }
    }
}