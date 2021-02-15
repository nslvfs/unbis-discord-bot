using System.Net;
using System.Text.RegularExpressions;
using System.IO;

namespace unbis_discord_bot.Logic
{
    public class Bash
    {
		public static string GetBash()
        {
			HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create("http://bash.org/?random");
			httpReq.Method = "GET";
			WebResponse httpRes = httpReq.GetResponse();
			StreamReader stream = new StreamReader(httpRes.GetResponseStream());
			string responseString = stream.ReadToEnd();
			stream.Close();
			httpRes.Close();
			int start = Regex.Match(responseString, "class=\"qt\"").Index + 11;
			int end = Regex.Match(responseString, "</p>\n<p class=\"quote\">").Index;
			string cutstring = responseString.Substring(start, end - start);
			cutstring = cutstring.Replace("&lt;", "<");
			cutstring = cutstring.Replace("&gt;", ">");
			cutstring = cutstring.Replace("&quot;", "\"");
			cutstring = cutstring.Replace("<br />", "\r\n");
			cutstring = cutstring.Replace("&nbsp;", " ");
			cutstring = cutstring.Replace("\r", "");
			cutstring = cutstring.Replace("\n\n", "\n");
			return cutstring;
		}

		public static string GetGermanBash()
		{
			HttpWebRequest httpReq = (HttpWebRequest)WebRequest.Create("http://german-bash.org/action/random/n/1");
			httpReq.Method = "GET";
			WebResponse httpRes = httpReq.GetResponse();
			StreamReader stream = new StreamReader(httpRes.GetResponseStream());
			string responseString = stream.ReadToEnd();
			stream.Close();
			httpRes.Close();
			Regex r = new Regex("\\<div class=\"zitat\"\\>(?<text>.*?)\\</div\\>", RegexOptions.Singleline);
			string cutstring = r.Match(responseString).Groups["text"].ToString();
			cutstring = cutstring.Replace("&lt;", "<");
			cutstring = cutstring.Replace("&gt;", ">");
			cutstring = cutstring.Replace("&quot;", "\"");
			cutstring = cutstring.Replace("<br />", "\r\n");
			cutstring = cutstring.Replace("&nbsp;", " ");
			cutstring = cutstring.Replace("&uuml;", "ü");
			cutstring = cutstring.Replace("&auml;", "ä");
			cutstring = cutstring.Replace("&ouml;", "ö");
			cutstring = cutstring.Replace("\r", "");
			cutstring = cutstring.Replace("\t", "");
			cutstring = cutstring.Replace("</span>", "");
			cutstring = cutstring.Replace("<span class=\"quote_zeile\">", "");
			cutstring = cutstring.Replace("\n                                    \n", "\n");
			cutstring = cutstring.Trim();
			return cutstring;
		}
	}
}
