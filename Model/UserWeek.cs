using System;
using System.Collections.Generic;
using System.Text;

namespace unbis_discord_bot.Model
{
    [Serializable]
    public class UserWeek
    {
        public ulong UserId { get; set; }
        public List<WorkDay> Week { get; set; }

        public UserWeek(ulong id)
        {
            UserId = id;
            Week = new List<WorkDay>();
        }
        public UserWeek() { }
    }

    [Serializable]
    public class WorkDay
    {
        public string DayName { get; set; }

        public string sBegin { get; set; }

        public string sEnd { get; set; }

        public WorkDay(string name)
        {
            DayName = name;
        }
        public WorkDay() { }
    }
}
