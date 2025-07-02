using BetterBot.Commands;
using BetterBot.Configuration;
using BetterBot.Database;
using BetterBot.Events;
using DSharpPlus;
using DSharpPlus.Commands;
using DSharpPlus.Commands.Processors.SlashCommands;
using DSharpPlus.Commands.Processors.SlashCommands.InteractionNamingPolicies;
using DSharpPlus.Interactivity.Extensions;
using Microsoft.Extensions.Logging;

namespace BetterBot {
    public class Program {
        public static async Task Main (string[] args) {
            await Config.ReadAsnyc();
            await DBEngine.createDB();

            DiscordClientBuilder builder = DiscordClientBuilder.CreateDefault(Config.Token, DiscordIntents.All);
            builder.UseInteractivity();

            
            builder.ConfigureExtraFeatures(c => {
                c.LogUnknownEvents = false;
                c.LogUnknownAuditlogs = false;
            });
            builder.ConfigureLogging(c => {
                c.SetMinimumLevel(LogLevel.Critical);
            });
            
            var commands = builder.UseCommands((IServiceProvider serviceProvider, CommandsExtension extension) => {
                extension.AddCommands([
                    typeof(TestCommands),
                    typeof(NoteCommands),
                ]);

                SlashCommandProcessor slashCommandProcessor = new(new SlashCommandConfiguration()
                {
                    NamingPolicy = new LowercaseNamingPolicy()
                });
                extension.AddProcessor(slashCommandProcessor);

            }, new CommandsConfiguration() {
                UseDefaultCommandErrorHandler = false
            });

            var events = builder.ConfigureEventHandlers(e => e
                .AddEventHandlers<ModalEvents>()
                .AddEventHandlers<BotEvents>()
            );

            await builder.ConnectAsync();
            await Task.Delay(-1);
        }
    }
}