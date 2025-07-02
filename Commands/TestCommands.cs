using System.ComponentModel;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.ArgumentModifiers;
using DSharpPlus.Entities;

namespace BetterBot.Commands {
    public class TestCommands {
        [Command("Test"), Description("Just a Test Command")]
        public static async ValueTask TestCommand (SlashCommandContext ctx) {
            await ctx.RespondAsync("test complete");
        }
    }    
}