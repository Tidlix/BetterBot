using BetterBot.Database;
using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace BetterBot.Events {
    public class ModalEvents : IEventHandler<ModalSubmittedEventArgs>
    {
        public async Task HandleEventAsync(DiscordClient s, ModalSubmittedEventArgs e)
        {
            if (e.Id.Contains("noteModal_")) await handleNoteModal(e);
        }

        private async Task handleNoteModal(ModalSubmittedEventArgs e) {
            await e.Interaction.DeferAsync(true);
            string title = e.Id.Substring(e.Id.IndexOf('_')+1);
            string value = e.Values.Values.First();
            ulong userid = e.Interaction.User.Id;

            if (await DBEngine.noteExists(userid, title)) {
                if (value == "") {
                    await DBEngine.deleteNote(userid, title);
                    await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"The note **{title}** was deleted!"));
                } 
                else {
                    await DBEngine.modifyNote(userid, title, value);
                    await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"The note **{title}** was modified!"));
                }
            }
            else {
                if (value != "") {
                    await DBEngine.createNote(userid, title, value);
                    await e.Interaction.EditOriginalResponseAsync(new DiscordWebhookBuilder().WithContent($"The note **{title}** was created!"));
                }
            }
        }
    }
}