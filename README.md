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

## Deployment (systemd)

Debug builds are self-contained `linux-x64` — no .NET runtime needed on the server.

1. Copy `bin/Debug/net10.0/linux-x64/` to the server
2. Place your `config.json` in that folder
3. `chmod +x /home/vfs/discordbot/unbis-discord-bot`
4. Adjust user/paths in `deploy/unbis-discord-bot.service`, then:

```bash
sudo cp deploy/unbis-discord-bot.service /etc/systemd/system/
sudo systemctl daemon-reload
sudo systemctl enable --now unbis-discord-bot
```

Logs: `journalctl -u unbis-discord-bot -f`. The service shuts down gracefully (SIGTERM → Discord disconnect) and restarts on failure.
