using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using System.Threading.Tasks;
using System.Timers;
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;

namespace DiscordBot.Commands
{
    public class TeamCommands : BaseCommandModule
    {
        [Command("join")]
        [Description("Determines if a person wants to join a role or not")]
        public async Task Join(CommandContext context)
        {
            var joinEmbed = new DiscordEmbedBuilder
            {
                Title = "Would you like to join?",
                Color = DiscordColor.SpringGreen,
                ImageUrl = context.Client.CurrentUser.AvatarUrl
            };

            var joinMessage = await context.Channel.SendMessageAsync(embed: joinEmbed).ConfigureAwait(false);

            var thumbsUpEmoji = DiscordEmoji.FromName(context.Client, ":+1:");
            var thumbsDownEmoji = DiscordEmoji.FromName(context.Client, ":-1:");
            var joyEmoji = DiscordEmoji.FromName(context.Client, ":joy:");

            await joinMessage.CreateReactionAsync(thumbsUpEmoji).ConfigureAwait(false);
            await joinMessage.CreateReactionAsync(thumbsDownEmoji).ConfigureAwait(false);
            await joinMessage.CreateReactionAsync(joyEmoji).ConfigureAwait(false);

            var interactivity = context.Client.GetInteractivity();

            var reactionResult = await interactivity.WaitForReactionAsync(x =>
                x.Message == joinMessage && x.User == context.User &&
                (x.Emoji == thumbsUpEmoji || x.Emoji == thumbsDownEmoji || x.Emoji == joyEmoji)).ConfigureAwait(false);

            if (reactionResult.Result.Emoji == thumbsUpEmoji)
            {
                var role = context.Guild.GetRole(929499760853909505);
                await context.Member.GrantRoleAsync(role).ConfigureAwait(false);
            }
            else if (reactionResult.Result.Emoji == thumbsDownEmoji)
            {
                var role = context.Guild.GetRole(929499760853909505);
                await context.Member.RevokeRoleAsync(role).ConfigureAwait(false);
            }
            else if (reactionResult.Result.Emoji == joyEmoji)
            {
                await context.Channel.SendMessageAsync("It's a \"yes\" or \"no\" question you dumb fuck").ConfigureAwait(false);
            }
            
            await joinMessage.DeleteAsync().ConfigureAwait(false);
        }

        [Command("poll")]
        [Description("Creates a poll")]
        public async Task Poll(CommandContext context, TimeSpan duration, params DiscordEmoji[] emojiOptions)
        {
            await context.Channel.SendMessageAsync($"You have {duration} to vote!").ConfigureAwait(false);

            var interactivity = context.Client.GetInteractivity();

            var options = emojiOptions.Select(x => x.ToString());

            var pollEmbed = new DiscordEmbedBuilder()
            {
                Title = "Poll",
                Description = string.Join(" ", options)
            };

            var pollMessage = await context.Channel.SendMessageAsync(embed: pollEmbed).ConfigureAwait(false);

            foreach (var option in emojiOptions)
            {
                await pollMessage.CreateReactionAsync(option).ConfigureAwait(false);
            }

            var result = await interactivity.CollectReactionsAsync(pollMessage, duration).ConfigureAwait(false);
            //var distinctResult = result.Distinct();

            foreach (var emoji in result)
            {
                await context.Channel.SendMessageAsync($"{emoji.Emoji}: {emoji.Total}");
            }

            //var results = distinctResult.Select(x => $"{x.Emoji}: {x.Total}");

            //await context.Channel.SendMessageAsync("Results:").ConfigureAwait(false);
            //await context.Channel.SendMessageAsync(string.Join("\n", results)).ConfigureAwait(false);

            //await context.Channel.DeleteMessageAsync(pollMessage).ConfigureAwait(false);
        }
    }
}
