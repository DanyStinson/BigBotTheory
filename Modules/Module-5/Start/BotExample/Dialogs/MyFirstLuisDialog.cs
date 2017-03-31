using BotExample.Extensions;
using BotExample.Model;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Builder.Luis.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BotExample.Dialogs
{
    [Serializable]
    public class MyFirstLuisDialog : LuisDialog<object>
    {
        public MyFirstLuisDialog(LuisService service) : base(service) { }

        [LuisIntent("Welcome")]
        public async Task Welcome(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Hi, I'm SheldonBot");
            await context.PostAsync("I can talk about my friends or weekly night plans, what would you like to know about?");
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

    }

    

}