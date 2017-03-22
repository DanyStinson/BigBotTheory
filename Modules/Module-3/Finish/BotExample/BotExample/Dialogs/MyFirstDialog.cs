using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;

namespace BotExample.Dialogs
{
    [Serializable]
    public class Character
    {
        public Character() { }

        public Character(string name, string profession, string information, string imageurl) {
            Name = name;
            Profession = profession;
            Information = information;
            Imageurl = imageurl;
        }
        public string Name { get; set; }
        public string Profession { get; set; }
        public string Information { get; set; }
        public string Imageurl { get; set; }
    }

    [Serializable]
    public class MyFirstDialog : IDialog<object>
    {
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

        private Dictionary<string, Character> characters = new Dictionary<string, Character>()
        {
            {"leonard", new Character("Leonard Hofstadter", "Experimental Physicist" ,"Leonard is my friend" ,"http://vignette3.wikia.nocookie.net/thebigbangtheory/images/b/bf/250px-Leonard.jpg/revision/latest?cb=20120917154638&path-prefix=es")},
            {"penny", new Character("Penny", "Aspiring Actress" ,"Leonard is my friend" ,"https://upload.wikimedia.org/wikipedia/en/4/41/Penny_bigbangtheory.jpg")},
            {"raj", new Character("Rajesh Koothrappali", "Particle Astrophysicist" ,"Leonard is my friend" ,"http://vignette2.wikia.nocookie.net/bigbangtheory/images/9/97/Raj.jpg/revision/20110809180135")},
            {"howard", new Character("Howard Wolowitz", "Aerospace Engineer" ,"Leonard is my friend" ,"http://vignette2.wikia.nocookie.net/bigbangtheory/images/6/6a/Howardwolowitz.jpg/revision/latest/top-crop/width/240/height/240?cb=20100425200930")}
        };

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Welcome, I´m Sheldon Bot");
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            ShowOptions(context);
        }

        private void ShowOptions(IDialogContext context)
        {
            var choices = new[] { "My Friends", "Tonight´s Plan" };
            PromptDialog.Choice(context, ChoiceSelectedAsync, choices,
                "What do you want me to talk to you about?", "Please select a valid option");
        }

        private async Task ChoiceSelectedAsync(IDialogContext context, IAwaitable<string> argument)
        {
            var choice = await argument;
            switch (choice)
            {
                case "My Friends":
                    var choices = new[] { "Leonard", "Penny", "Howard", "Raj" };
                    PromptDialog.Choice(context, FriendSelectedAsync, choices, "Who do you want to know about?", "Please select a valid option");
                    break;
                case "Tonight´s Plan":
                    PromptDialog.Text(context, ReturnPlanAsync, "What day of the week is today?", "Please enter the name of the day", 3);
                    break;
                default:
                    ShowOptions(context);
                    break;
            }
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
        
        private async Task FriendSelectedAsync(IDialogContext context, IAwaitable<string> argument)
        {
            var choice = await argument;
            if (characters.ContainsKey(choice.ToLower()))
            {
                var reply = context.MakeMessage();
                reply.Attachments = new List<Attachment>
                {
                   CreateCharacterCard(context, characters[choice.ToLower()])
                };
                await context.PostAsync(reply);
            }
            else
            {
                await context.PostAsync($"Sorry, {choice} isn´t in my friends list");
            }

            ShowOptions(context);
        }

        private async Task ReturnPlanAsync(IDialogContext context, IAwaitable<string> argument)
        {
            var choice = await argument;
            if (plans.ContainsKey(choice))
            {
                await context.PostAsync($"If today is {choice} you should {plans[choice.ToLower()]}");
            }
            else
            {
                await context.PostAsync($"Sorry, {choice} isn´t a day of my week");
            }

            ShowOptions(context);
        }
    }
}