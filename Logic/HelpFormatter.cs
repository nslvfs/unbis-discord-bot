using System.Collections.Generic;
using System.Linq;
using System.Text;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Converters;
using DSharpPlus.CommandsNext.Entities;

namespace unbis_discord_bot.Logic
{
    public class HelpFormatter : BaseHelpFormatter
    {
        private StringBuilder MessageBuilder { get; }

        public HelpFormatter(CommandContext ctx) : base(ctx)
        {
            this.MessageBuilder = new StringBuilder();
        }

        public override BaseHelpFormatter WithCommand(Command command)
        {
            this.MessageBuilder.Append("Befehl: ")
                .AppendLine(Formatter.Bold(command.Name))
                .AppendLine();


            this.MessageBuilder.Append("Beschreibung: ")
                .AppendLine(command.Description)
                .AppendLine();

            if (command is CommandGroup)
                this.MessageBuilder.AppendLine("Diese Gruppe hat nur einen Befehl.").AppendLine();

            this.MessageBuilder.Append("Aliases: ")
                .AppendLine(string.Join(", ", command.Aliases))
                .AppendLine();


            foreach (var overload in command.Overloads)
            {
                if (overload.Arguments.Count == 0)
                {
                    continue;
                }

                this.MessageBuilder.Append($"[Overload {overload.Priority}] Argumente: ")
                .AppendLine(string.Join(", ", overload.Arguments.Select(xarg => $"{xarg.Name} ({xarg.Type.Name})")))
                .AppendLine();
            }

            return this;
        }

        public override BaseHelpFormatter WithSubcommands(IEnumerable<Command> subcommands)
        {
            this.MessageBuilder.Append("Befehle in dieser Gruppe:\n")
                .AppendLine(string.Join("\n", subcommands.Select(xc => xc.Name)))
                .AppendLine();

            return this;
        }

        public override CommandHelpMessage Build()
        {
            return new CommandHelpMessage(this.MessageBuilder.ToString().Replace("\r\n", "\n"));
        }

    }
}
