using BotExample.Extensions;
using BotExample.Model;
using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;

namespace BotExample.Dialogs
{
    [Serializable]
    public class MyFirstDialog : IDialog<object>
    {
        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync("Welcome, I'm Sheldon Bot");
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            ShowOptions(context);
        }

        private void ShowOptions(IDialogContext context)
        {
            var choices = new[] { "My Friends", "Tonight's Plan" };
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
                case "Tonight's Plan":
                    PromptDialog.Text(context, ReturnPlanAsync, "What day of the week is today?", "Please enter the name of the day", 3);
                    break;
                default:
                    ShowOptions(context);
                    break;
            }
        }
        
        private async Task FriendSelectedAsync(IDialogContext context, IAwaitable<string> argument)
        {
            var choice = await argument;
            var character = new BigBangTheoryClient().GetCharacter(choice);
            if (character != null)
            {
                await context.PostAsync(character.ToMessage(context));
            }
            else
            {
                await context.PostAsync($"Sorry, {choice} isn't in my friends list");
            }

            ShowOptions(context);
        }

        private async Task ReturnPlanAsync(IDialogContext context, IAwaitable<string> argument)
        {
            var choice = await argument;
            var plan = new BigBangTheoryClient().GetPlan(choice);
            if (plan != null)
            {
                await context.PostAsync($"If today is {choice} you should {plan}");
            }
            else
            {
                await context.PostAsync($"Sorry, {choice} isn't a day of my week");
            }

            ShowOptions(context);
        }
    }
}