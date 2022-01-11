using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Interactivity.Extensions;
using System;
using System.Threading.Tasks;

namespace DiscordBot.Commands
{
    public class FunCommands : BaseCommandModule
    {
        [Command("ping")]
        [Description("Returns pong")]
        public async Task Ping(CommandContext context)
        {
            await context.Channel.SendMessageAsync("Pong").ConfigureAwait(false);
        }

        [Command("calc")]
        [Description("Does simple calculations with 2 numbers")]
        public async Task Calculator(CommandContext context, [Description("First Number")] double num1, [Description("Sign")] string sign, [Description("Second Number")] double num2)
        {
            double result = 0;

            if (sign == "+")
            {
                result = num1 + num2;
            }
            else if (sign == "-")
            {
                result = num1 - num2;
            }
            else if (sign == "*")
            {
                result = num1 * num2;
            }
            else if (sign == "/")
            {
                result = num1 / num2;
            }

            await context.Channel.SendMessageAsync(result.ToString()).ConfigureAwait(false);
        }

        [Command("shlong")]
        [Description("Returns the size of your pp")]
        public async Task Shlong(CommandContext context)
        {
            Random rnd = new Random();
            int num = rnd.Next(2, 30);

            await context.Channel.SendMessageAsync($"The size of {context.User.Username}'s shlong is {num}cm").ConfigureAwait(false);
        }

        [Command("respond")]
        [Description("Repeats back a message")]
        public async Task Respond(CommandContext context)
        {
            var interactivity = context.Client.GetInteractivity();

            var response = await interactivity.WaitForMessageAsync(x => x.Channel == context.Channel).ConfigureAwait(false);

            await context.Channel.SendMessageAsync(response.Result.Content);
        }
    }
}
