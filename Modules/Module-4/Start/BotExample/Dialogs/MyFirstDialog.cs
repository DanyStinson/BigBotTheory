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
        private int count;

        public async Task StartAsync(IDialogContext context)
        {
            await context.PostAsync($"You're new!");
            context.Wait(MessageReceivedAsync);
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> argument)
        {
            var message = await argument;
            await context.PostAsync($"You have talked to me {count++} times now, {message.From.Name}");
            context.Wait(MessageReceivedAsync);
        }
    }
}