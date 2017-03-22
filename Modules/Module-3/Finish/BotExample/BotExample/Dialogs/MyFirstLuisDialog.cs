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

        [LuisIntent("Welcome")]
        public async Task Welcome(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Hi, I´m SheldonBot");
            await context.PostAsync("I can talk about my friends or weekly night plans, what would you like to know about?");
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
                }
                else
                {
                    await context.PostAsync($"Sorry, {friend} isn´t in my friends list");
                    await context.PostAsync(CreateCharactersCarousel(context));
                }
            }
            else {
                await context.PostAsync($"Here are some of my friends");
                await context.PostAsync(CreateCharactersCarousel(context));
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
        }
    
       [LuisIntent("")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            await context.PostAsync($"Sorry, I did not understand '{result.Query}'");
            context.Wait(MessageReceived);
        }

    }
}