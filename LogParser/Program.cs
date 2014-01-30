using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            //Typical format 
            //22/01/2014 15:21:39 [2032] CMSEvent::NullActionHandler complete
            //1389362450 [6592] NetIQccm: pingms=30000  pollmc=5000  tracing=1  level=17
            string pattern = @"(?<datetime>\d{2}[\./]\d{2}[\./]\d{4}\s\d{2}:\d{2}:\d{2})\s(?<threadId>\[\d+\])";//\\s(?<threadId>[.*])";//\\s(?<message>.*)";
            if(!(args.Length>0))
                return;
            var lines = File.ReadLines(args[0]);
            string header= ReadHeader(lines.ToArray<string>());
            Regex regExpr = new Regex(pattern);
            var threads = GetThreads(regExpr, lines.ToArray<string>());
            var filterbyThread = GetLogsForThread("[4492]", lines.ToArray<string>());
        }
        static string ReadHeader(string[] lines)
        {
            string header="";
            //string pattern = "(?<date>\\d{2}/\\d{2}/\\d{4})\\s(?<time>\\d{2}:\\d{2}:\\d{2})(?<threadId>[\\d{4}])\\s(?<message>.*)";
            //string pattern = "(?<datetime>\\d{2}/\\d{2}/\\d{4}\\s\\d{2}:\\d{2}:\\d{2})\\s(?<threadId>\\[\\d{5}\\])";//\\s(?<threadId>[.*])";//\\s(?<message>.*)";
            //string pattern = @"(?<datetime>\d{2}[\./]\d{2}[\./]\d{4}\s\d{2}:\d{2}:\d{2})\s(?<threadId>\[\d{5}\])";//\\s(?<threadId>[.*])";//\\s(?<message>.*)";
            string pattern = @"(?<datetime>\d{2}[\./]\d{2}[\./]\d{4}\s\d{2}:\d{2}:\d{2})\s(?<threadId>\[\d+\])";//\\s(?<threadId>[.*])";//\\s(?<message>.*)";
            //string curLine = "28/01/2014 08:29:50 [45304] SocketWorkerThread: message send to 149.184.165.134, size 474, actual bytes wrote 474";
            //string curLine = "22.01.2014 16:14:30 [2188] ClaimRPCTableEntryAccess: RPC Table Lock claimed SQL-EU-IRMS-2";
            Regex regExpr = new Regex(pattern);
            //var value = regExpr.IsMatch(curLine);
            foreach(var line in lines)
            {
                if(!regExpr.IsMatch(line))
                {
                    header+=line+"\\r\\n";
                }
                else
                    break;

            }
            return header;

        }

        static string[] GetThreads(Regex regexpr,string[] lines)
        {
            List<string> threadList = new List<string>();
            foreach(string line in lines)
            {
                MatchCollection matches = regexpr.Matches(line);
                if (matches.Count > 0)
                {
                    GroupCollection groups = matches[0].Groups;
                    string value = groups["threadId"].Value;
                    if (!threadList.Contains(value))
                        threadList.Add(value);
                }

            }
            return threadList.ToArray<string>();
        }

        static string[] GetLogsForThread(string thread, string[] lines)
        {
            var filterData = lines.Where(line => line.Contains(thread)).Select(x=>x);
            return filterData.ToArray<string>();
        }

        
    }
}
