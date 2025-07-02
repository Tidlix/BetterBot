using BetterBot.Database;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;

namespace BetterBot.Commands {
    public class NoteChoiceProvider : IAutoCompleteProvider
    {
        public async ValueTask<IEnumerable<DiscordAutoCompleteChoice>> AutoCompleteAsync(AutoCompleteContext ctx)
        {
            #pragma warning disable CS8604
            var list = await DBEngine.getNoteChoices(ctx.User.Id, ctx.UserInput);

            return await new ValueTask<IEnumerable<DiscordAutoCompleteChoice>>(list);        
        }
    }
}