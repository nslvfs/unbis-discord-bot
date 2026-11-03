# unbis-discord-bot

little .NET 10 C# project for my own discord server

it utilizes DSharpPlus 4.5 (MIT License)

## Setup

Copy `config.json` next to the executable and fill in your values (the file is gitignored):

- `token` – Discord bot token
- `prefix` – command prefix, e.g. `!`
- `quotePath`, `xSichterPath`, `badwords`, `wettenDBFile` – data file locations
- `guildIdUnbi`, `guildIdRatte`, `channelIdRotz`, `userIdvfs`, `botIdSelf`, `botIdSantos`, `roleIdMuted` – Discord IDs for your server

Run with `dotnet run` or deploy the published binary to your server.
