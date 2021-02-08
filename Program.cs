using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace unbis_discord_bot
{
    class Program
    {
        static void Main(string[] args)
        {
            Bot bot = new Bot();
            bot.TaskAsync().GetAwaiter().GetResult();
        }
    }
}
