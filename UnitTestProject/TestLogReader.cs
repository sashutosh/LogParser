using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TraceAnalyzer;

namespace UnitTestProject
{
    [TestClass]
    public class TestLogReader
    {
        LogReader reader;
        //string logFile = "c:\\temp.log";
       string logFile = @"D:\AppManager\CI\ENG332863\TSC-SVRV-NIQMS1_01.29.14_16.28.29_diag\ms.log.bkup";
       string testLog = @"29/01/2014 07:58:55
+++++  MS process ID :  1768
       MS host name  :  TSC-SVRV-NIQMS1
       MS Version    :  8.0.30003.611

29/01/2014 07:58:55 [2256] Enter MSCompareAndSync
29/01/2014 07:58:56 [3004] MSDataLog_V6ServiceCall: processing service call from machine 10.1.24.10";

        [TestInitialize]
        public void SetUpTest()
        {
            
            List<string> logList = new List<string>();
            logList.Add(testLog);
            File.WriteAllLines(logFile, logList.ToArray());
            reader = new LogReader(logFile);
        }

        [TestMethod]
        public void TestHeaderContainsHeader()
        {
            string header = reader.ReadHeader();
            Assert.AreEqual(true, header.Contains("MS Version    :  8.0.30003.611"));
        }

        [TestMethod]
        public void TestHeaderDoesNotContainLog()
        {
            string header = reader.ReadHeader();
            Assert.AreEqual(false, header.Contains("29/01/2014 07:58:55 [2256] Enter MSCompareAndSync"));
        }
        [TestMethod]
        public void TestThreadCountInLogs()
        {
            var threads = reader.GetThreads();
            Assert.AreEqual(2, threads.Length);
        }
        
        [TestMethod]
        public void TestLogsforThread()
        {
            var logs = reader.GetLogsForThread("[2256]");
            Assert.AreEqual("29/01/2014 07:58:55 [2256] Enter MSCompareAndSync", logs[0]);
        }
        [TestMethod]
        public void TestStartTime()
        {
            var datetime = reader.GetStartTime();
            Assert.AreEqual("29/01/2014 07:58:55", datetime);
        }


        [TestCleanup]
        public void TearDown()
        {
            File.Delete(logFile);
        }

    }
}
