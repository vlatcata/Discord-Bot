using System;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.EventArgs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using DiscordBot.Commands;
using DSharpPlus.Interactivity;
using DSharpPlus.Interactivity.Extensions;

namespace DiscordBot
{
    class Bot
    {
        // The "Client" is the Bot
        public DiscordClient Client { get; private set; }

        public InteractivityExtension Interactivity { get; private set; }

        public CommandsNextExtension Commands { get; private set; }

        public async Task RunBotAsync()
        {
            var json = string.Empty;

            using (var fs = File.OpenRead(@"C:\Programming\Projects\Discord Bot\Discord Bot\Discord Bot\config.json"))
            using (var sr = new StreamReader(fs, new UTF8Encoding(false)))
                json = await sr.ReadToEndAsync().ConfigureAwait(false);

            var configJson = JsonConvert.DeserializeObject<ConfigJson>(json);

            var config = new DiscordConfiguration()
            {
                Token = configJson.Token,
                TokenType = TokenType.Bot,
                AutoReconnect = true,
                MinimumLogLevel = LogLevel.Debug
            };

            Client = new DiscordClient(config);

            Client.Ready += OnClientReady;

            Client.UseInteractivity(new InteractivityConfiguration
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] {configJson.Prefix},
                EnableMentionPrefix = true,
                EnableDms = false,
                DmHelp = true
            };

            Commands = Client.UseCommandsNext(commandsConfig);

            // Register every new class for commands
            Commands.RegisterCommands<FunCommands>();

            Commands.RegisterCommands<TeamCommands>();

            await Client.ConnectAsync();

            await Task.Delay(-1);
        }

        private Task OnClientReady(DiscordClient c, ReadyEventArgs e)
        {
            return Task.CompletedTask;
        }
    }
}
