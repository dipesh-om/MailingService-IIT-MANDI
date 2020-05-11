using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace MailingService
{
    class UtilityClass
    {
        public String GetCode(String s)
        {
            String os = "";
            if (Regex.IsMatch(s, "^B"))
            {
                os = Regex.Replace(s, "B", "100").Trim();
            }
            else if (Regex.IsMatch(s, "^A"))
            {
                os = Regex.Replace(s, "A", "200").Trim();
            }
            else if (Regex.IsMatch(s, "^V"))
            {
                os = Regex.Replace(s, "V", "300").Trim();
            }
            return os;
        }
        public Boolean ValidateEntryTime(String s, int dayNo)
        {
            Match match = Regex.Match(s, "....(..)(..)(....)(..)(..)([A-Za-z0-9]+)");
            DateTime d = new DateTime(int.Parse(match.Groups[3].Value), int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[4].Value), int.Parse(match.Groups[5].Value), 00);
            if (match.Success)
            {
                DateTime d1 = DateTime.Now.AddDays(dayNo);
                DateTime d2 = new DateTime(d1.Year, d1.Month, d1.Day, 20, 0, 0);
                DateTime d3 = new DateTime(d1.Year, d1.Month, d1.Day+1, 00, 55, 0);
                if (d < d3 && d > d2)
                {
                    return true;
                } else
                {
                    return false;
                }
                
            } else
            {
                return false;
            }
        }
        public DateTime Gettime(String s)
        {
            Match match = Regex.Match(s, "....(..)(..)(....)(..)(..)([A-Za-z0-9]+)");
            DateTime d = new DateTime(int.Parse(match.Groups[3].Value), int.Parse(match.Groups[1].Value), int.Parse(match.Groups[2].Value), int.Parse(match.Groups[4].Value), int.Parse(match.Groups[5].Value), 00);
            //Console.WriteLine(d.ToString());
            return d;
        }

        public List<KeyValuePair<String, String>> AddCCs(DataRow dr)
        {
            List<KeyValuePair<String, String>> cc = new List<KeyValuePair<String, String>>();
            KeyValuePair<String, String> cc1 = new KeyValuePair<String, String>(dr.Field<String>(4).ToString().Trim(), dr.Field<String>(6).ToString().Trim());
            KeyValuePair<String, String> cc2 = new KeyValuePair<String, String>(dr.Field<String>(5).ToString().Trim(), dr.Field<String>(7).ToString().Trim());
            KeyValuePair<String, String> cc3;
            cc.Add(cc1);
            cc.Add(cc2);
            if (dr.Field<String>(11) != null)
            {
                cc3 = new KeyValuePair<String, String>(dr.Field<String>(11).ToString().Trim(), dr.Field<String>(12).ToString().Trim());
                cc.Add(cc3);
            }
            return cc;
        }
    }
}
