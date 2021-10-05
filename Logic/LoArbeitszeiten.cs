using System;
using System.IO;
using System.Text.Json;
using unbis_discord_bot.Model;

namespace unbis_discord_bot.Logic
{
    public class LoArbeitszeiten
    {
        public static bool ValidateInput(string begin, string end)
        {
            try
            {
                TimeSpan beginTs = TimeSpan.Parse(begin);
                TimeSpan endTs = TimeSpan.Parse(end);
                return true;
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static void SetSingleDay(ulong userId, string newDay, TimeSpan begin, TimeSpan end)
        {
            string fileName = userId.ToString() + ".json";

            if (!File.Exists(fileName))
            {
                UserWeek dataItem = new UserWeek(userId);
                WorkDay addDay = new WorkDay(newDay);
                addDay.sBegin = begin.ToString(("hh':'mm"));
                addDay.sEnd = end.ToString(("hh':'mm")); ;
                dataItem.Week.Add(addDay);
                WriteWeekFile(dataItem);
            }
            else
            {
                UserWeek dataItem = ReadWeekFile(userId);
                foreach (var day in dataItem.Week)
                {
                    if (day.DayName == newDay)
                    {
                        day.sBegin = begin.ToString(("hh':'mm")); ;
                        day.sEnd = end.ToString(("hh':'mm")); ;
                        WriteWeekFile(dataItem);
                        return;
                    }

                }
                WorkDay addDay = new WorkDay(newDay);
                addDay.sBegin = begin.ToString(("hh':'mm")); ;
                addDay.sEnd = end.ToString(("hh':'mm")); ;
                dataItem.Week.Add(addDay);
                WriteWeekFile(dataItem);
            }

        }

        public static string GetSingleDay(ulong id, string getDay)
        {
            UserWeek dataItem = ReadWeekFile(id);
            if(dataItem == null)
            {
                return "keine Daten Vorhanden";
            }
            foreach (var day in dataItem.Week)
            {
                if (day.DayName == getDay)
                {
                    var begin = day.sBegin;
                    var end = day.sEnd;
                    return "arbeitet am " + getDay + " von " + begin + " bis " + end;
                }
            }
            return "arbeitet anscheinend am " + getDay + " nicht";
        }

        public static void GetSingleDay(ulong id)
        {
            string[] validDays = { "So", "Mo", "Di", "Mi", "Do", "Fr", "Sa" };
            int d = (int)DateTime.Now.DayOfWeek;
            string day = validDays[d];
            GetSingleDay(id, day);
        }

        public static void DelSingleDay(ulong id, string getDay)
        {
            UserWeek dataItem = ReadWeekFile(id);
            WorkDay temp = new WorkDay();
            bool found = false;
            foreach (var day in dataItem.Week)
            {
                if (day.DayName == getDay)
                {
                    temp = day;
                    found = true;
                    break;
                }
            }
            if(found)
            {
                dataItem.Week.Remove(temp);
                WriteWeekFile(dataItem);
            }

        }

        private static UserWeek ReadWeekFile(ulong id)
        {
            string fileName = id.ToString() + ".json";
            if(File.Exists(fileName)) { 
                string jsonString = File.ReadAllText(fileName);
                UserWeek data = JsonSerializer.Deserialize<UserWeek>(jsonString);
                return data;
            }
            return null;
        }

        private static void WriteWeekFile(UserWeek data)
        {
            string fileName = data.UserId.ToString() + ".json";
            string json = JsonSerializer.Serialize(data);
            File.WriteAllText(fileName, json);
        }
    }
}
