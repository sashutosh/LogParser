using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace TraceAnalyzer
{
    public class LogReader
    {
        string pattern = @"(?<datetime>\d{2}[\./]\d{2}[\./]\d{4}\s\d{2}:\d{2}:\d{2})\s(?<threadId>\[\d+\])";//\\s(?<threadId>[.*])";//\\s(?<message>.*)";
        Regex regExpr;
        string logFileName = "";
        IEnumerable<string> fileLines;

        public LogReader(string fileName)
        {
            this.logFileName = fileName;
            regExpr = new Regex(pattern);
            ReadLog();

        }
        bool ReadLog()
        {
            if (!File.Exists(this.logFileName))
                throw new FileNotFoundException("Not able to access file %s", logFileName);
            else
            {
                try
                {
                    fileLines = File.ReadLines(logFileName);
                }
                catch (Exception ex)
                {
                    Trace.TraceError(ex.Message);
                    return false;
                }
                return true;
            }
        }

        public string ReadHeader()
        {
            string header = "";
            foreach (var line in fileLines)
            {
                if (!regExpr.IsMatch(line))
                {
                    header += line + "\\r\\n";
                }
                else
                    break;
            }
            return header;
        }
        string FindGroup(string group,string line)
        {
            string matchValue="";
            MatchCollection matches = regExpr.Matches(line);
            if (matches.Count > 0)
            {
                GroupCollection groups = matches[0].Groups;
                matchValue = groups[group].Value;

            }
            return matchValue;
        }
        public string[] GetThreads()
        {
            List<string> threadList = new List<string>();
            
            foreach (string line in fileLines)
            {
                string value = FindGroup("threadId",line);
                if (value.Length > 0)
                {
                    if (!threadList.Contains(value))
                        threadList.Add(value);
                }
            }
            return threadList.ToArray();
        }

        public List<string> GetLogsForThread(string thread)
        {
            var filterData = from line in fileLines where line.Contains(thread) select line;
            return filterData.ToList();
            //return null;
        }
        public string GetStartTime()
        {
            var header = ReadHeader();
            int headerLines = header.Split('\n').Length;

            var result = regExpr.Split(fileLines.ToArray<string>()[headerLines + 1]);
            return result[0];

        }
        public string GetEndTime()
        {
            return null;
        }
    }
}
