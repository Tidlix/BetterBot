using System.Net.Http.Headers;
using BetterBot.LogMessages;
using DSharpPlus;
using DSharpPlus.Commands.EventArgs;
using DSharpPlus.EventArgs;

namespace BetterBot.Events {
    public class BotEvents :
        IEventHandler<CommandErroredEventArgs>,
        IEventHandler<ClientErrorEventArgs>,
        IEventHandler<ClientStartedEventArgs>
    {

        #region errors
        public async Task HandleEventAsync(DiscordClient s, CommandErroredEventArgs e)
        {
            try {await e.Context.RespondAsync("An error occured. Please try again later! \n*If the error still occures, notify the support!*");}
            finally {await e.Context.EditResponseAsync("An error occured. Please try again later! \n*If the error still occures, notify the support!*");}
            
            MessageSystem.sendMessage($"Command Error - {e.Exception.Message}", MessageType.Error());   
        }
        public Task HandleEventAsync(DiscordClient s, ClientErrorEventArgs e)
        {
            MessageSystem.sendMessage($"Client Error - {e.Exception.Message}", MessageType.Error());   
            return Task.CompletedTask;
        }
        #endregion

        public Task HandleEventAsync(DiscordClient s, ClientStartedEventArgs e)
        {
            MessageSystem.sendMessage("Client started!", MessageType.Message());   
            return Task.CompletedTask;
        }
    }
}