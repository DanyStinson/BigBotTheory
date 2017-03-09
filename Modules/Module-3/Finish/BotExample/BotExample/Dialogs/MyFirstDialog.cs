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
    public class MyFirstDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Welcome, I´m Sheldon Bot");
            OptionsMenuAsync(context);
        }

        private void OptionsMenuAsync(IDialogContext context)
        {
            var choices = new[] {"My Friends", "Tonight´s Plan" };
            PromptDialog.Choice(context, ChoiceSelected, choices,
            "What do you want me to talk to you about?","Please select a valid option");
        }

        private async Task ChoiceSelected(IDialogContext context, IAwaitable<string> argument)
        {
            var choice = await argument;
            switch (choice)
            {
                case "My Friends":
                    var choices = new[] { "Leonard", "Penny", "Howard", "Raj" };
                    PromptDialog.Choice(context, FriendSelected, choices, "Who do you want to know about?", "Please select a valid option");
                    break;
                case "Tonight´s Dinner":
                    PromptDialog.Text(context, ReturnDinner, "What day of the week is today?", "Please enter the name of the day", 3);
                    break;
                default:
                    OptionsMenuAsync(context);
                    break;
            }
        }

        public void createHeroCard(IMessageActivity reply, string name)
        {
            reply.Attachments = new List<Attachment>();
            HeroCard hc = new HeroCard();
            List<CardImage> images = new List<CardImage>();
            CardImage ci = new CardImage();

            if (name == "Leonard")
            {
                hc.Title = "Leonard Hofstadter";
                hc.Subtitle = "Experimental Physicist";
                ci.Url = "http://vignette3.wikia.nocookie.net/thebigbangtheory/images/b/bf/250px-Leonard.jpg/revision/latest?cb=20120917154638&path-prefix=es";
            }
            else if (name == "Penny")
            {
                hc.Title = "Penny";
                hc.Subtitle = "Aspiring Actress";
                ci.Url = "https://upload.wikimedia.org/wikipedia/en/4/41/Penny_bigbangtheory.jpg";
            }

            else if (name == "Raj")
            {
                hc.Title = "Rajesh Koothrappali";
                hc.Subtitle = "Particle Astrophysicist";
                ci.Url = "http://vignette2.wikia.nocookie.net/bigbangtheory/images/9/97/Raj.jpg/revision/20110809180135";
            }

            else if (name == "Howard")
            {
                hc.Title = "Howard Wolowitz";
                hc.Subtitle = "Aerospace Engineer";
                ci.Url = "http://vignette2.wikia.nocookie.net/bigbangtheory/images/6/6a/Howardwolowitz.jpg/revision/latest/top-crop/width/240/height/240?cb=20100425200930";
            }

            images.Add(ci);
            hc.Images = images;
            reply.Attachments.Add(hc.ToAttachment());

        }
        
        private async Task FriendSelected(IDialogContext context, IAwaitable<string> argument)
        {
            var choice = await argument;
            var reply = context.MakeMessage();
            createHeroCard(reply, choice);
            await context.PostAsync(reply);
            context.Done("");
        }

        private async Task ReturnDinner(IDialogContext context, IAwaitable<string> argument)
        {
            var choice = await argument;
            var reply = context.MakeMessage();
            reply.Text = $"If today is {choice} you should ";

            if (choice == "Monday") {reply.Text += "get a Thai takeout!";}
            else if (choice == "Tuesday") { reply.Text += "have a cheeseburger at the Cheesecake Factory!"; }
            else if (choice == "Wednesday") { reply.Text += "play Halo with your friends!"; }
            else if (choice == "Thursday") { reply.Text += "have a nice slice of pizza!"; }
            else if (choice == "Friday") { reply.Text += "get a chinese takeaway!"; }
            else if (choice == "Saturday") { reply.Text += "do some of your laundry!"; }
            else if (choice == "Sunday") { reply.Text += "relax at home and do some physics!"; }
            else { reply.Text = "That isn´t a day of the week!!"; }

            await context.PostAsync(reply);
            context.Done("");

        }


    }
}