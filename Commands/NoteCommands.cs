using System.ComponentModel;
using BetterBot.Database;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;

namespace BetterBot.Commands {
    [Command("mynotes")]
    public class NoteCommands {
        [Command("show")]
        public async ValueTask showNote (SlashCommandContext ctx,  [SlashAutoCompleteProvider<NoteChoiceProvider>] string noteTitle) {
            await ctx.DeferResponseAsync();
            
            if (!await DBEngine.noteExists(ctx.User.Id, noteTitle)) {
                await ctx.EditResponseAsync($"The note {noteTitle} couldn't be shown! - This note doesn't exist!");
                return;
            }

            string value = await DBEngine.getNoteValue(ctx.User.Id, noteTitle);

            await ctx.EditResponseAsync(value);
        }

        [Command("open")]
        public async ValueTask openNote (SlashCommandContext ctx,  [SlashAutoCompleteProvider<NoteChoiceProvider>] string noteTitle) {
            var modal = new DiscordInteractionResponseBuilder();
            modal
                .WithTitle(noteTitle)
                .WithCustomId($"noteModal_{noteTitle}")
                .AddTextInputComponent(new DiscordTextInputComponent(
                    style: DiscordTextInputStyle.Paragraph,
                    label: "Your Note: ",
                    value: await DBEngine.getNoteValue(ctx.User.Id, noteTitle),
                    customId: $"noteInput_{noteTitle}",
                    required: false,
                    max_length: 2000
            ));
                                            
            await ctx.RespondWithModalAsync(modal);
        }
    }
}