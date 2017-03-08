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
            var choices = new[] {"My Friends", "Tonight´s Dinner" };
            PromptDialog.Choice(context, ChoiceSelected, choices,
            "What do you want me to talk to you about?","Please select a valid option");
        }

        private async Task ChoiceSelected(IDialogContext context, IAwaitable<string> argument)
        {
            var choice = await argument;
            switch (choice)
            {
                case "My Friends":
                    showFriends(context);
                    break;
                case "Tonight´s Dinner":
                    await context.PostAsync($"What day of the week is it today?");
                    context.Done("");
                    break;
                default:
                    OptionsMenuAsync(context);
                    break;
            }
        }

        public void createHeroCard(IMessageActivity reply, string name)
        {
            reply.Attachments = new List<Microsoft.Bot.Connector.Attachment>();
            HeroCard hc = new HeroCard();
            List<CardImage> images = new List<CardImage>();
            CardImage ci = new CardImage();

            if (name == "Leonard")
            {
                hc.Title = "Leonard Hofstadter";
                hc.Subtitle = "Experimental Physicist";
                ci.Url = "Images/Leonard.jpg";
            }
            else if (name == "Penny")
            {
                hc.Title = "Penny";
                hc.Subtitle = "Aspiring Actress";
                ci.Url = "../Images/Penny.jpg";
            }

            else if (name == "Raj")
            {
                hc.Title = "Rajesh Koothrappali";
                hc.Subtitle = "Particle Astrophysicist";
                ci.Url = "../Images/Raj.jpg";
            }

            else if (name == "Howard")
            {
                hc.Title = "Howard Wolowitz";
                hc.Subtitle = "Aerospace Engineer";
                ci.Url = "../Images/howard.jpg";
            }

            images.Add(ci);
            hc.Images = images;
            reply.Attachments.Add(hc.ToAttachment());

        }
        
        public void showFriends(IDialogContext context) {
            var choices = new[] { "Leonard", "Penny", "Howard", "Raj"};
            PromptDialog.Choice(context, FriendSelected, choices, "Who do you want to know about?", "Please select a valid option");
        }

        private async Task FriendSelected(IDialogContext context, IAwaitable<string> argument)
        {
            var choice = await argument;
            var reply = context.MakeMessage();
            createHeroCard(reply, choice);
            await context.PostAsync(reply);
            context.Done("");
        }

    }
}