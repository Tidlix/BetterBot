using DSharpPlus.Commands;

namespace BetterBot.Commands
{
    [Command("Dev")]
    public class DevCommands
    {
        [Command("GetUser")]
        public async ValueTask getUserAsync(CommandContext ctx, ulong userid)
        {
            var user = await ctx.Client.GetUserAsync(userid, true);
            await ctx.RespondAsync(user.ToString());
        }
    }
}