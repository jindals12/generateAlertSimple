using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace ConsoleApp1
{
    class Program
    {
       public struct log
        {
            public String errorCode;
            public String machineName;
            public int level;
        }
        public struct printList
        {
            public string machineName;
            public string errorCode;
            public int count;
        }

        public static void populateQueue()
        {
            string textFile = @"C:\Users\sachinji.FAREAST\source\repos\ConsoleApp1\ConsoleApp1\testData.txt";
            using (StreamReader file = new StreamReader(textFile))
            {
                string ln;
                while ((ln = file.ReadLine()) != null)
                {
                    string[] tempString = ln.Split(' ');
                    log templog;
                    templog.machineName = tempString[0];
                    templog.errorCode = tempString[1];
                    templog.level = int.Parse(tempString[2]);
                    logQueue.Enqueue(templog);
                }
                file.Close();
            }
        }

        public static Queue<log> logQueue = new Queue<log>();
        public static List<printList> printCountPerMachine = new List<printList>();

        static void Main(string[] args)
        {   
            populateQueue();
            const int Threshold = 30;
            int cumCount = 0;
            //Machine name, error code/count
            Dictionary<string, Dictionary<string, int>> machineErrorCount= new Dictionary<string, Dictionary<string, int>>();
            while (logQueue.Count > 0)
            {
                log tempLog = logQueue.Dequeue();
                if (tempLog.level == 0 || tempLog.level == 1)
                {
                    cumCount++;
                    if (machineErrorCount.ContainsKey(tempLog.machineName))
                    {
                        Dictionary<string, int> tempErrorCount = new Dictionary<string, int>();
                        machineErrorCount.TryGetValue(tempLog.machineName, out tempErrorCount);
                        if (tempErrorCount.ContainsKey(tempLog.errorCode))
                        {
                            int count = 0;
                            tempErrorCount.TryGetValue(tempLog.errorCode, out count);
                            machineErrorCount[tempLog.machineName][tempLog.errorCode] = ++count;
                        }
                        else
                        {
                            tempErrorCount.Add(tempLog.errorCode, 1);
                            machineErrorCount[tempLog.machineName] = tempErrorCount;
                        }
                    }
                    else
                    {
                        Dictionary<string, int> tempErrorCount = new Dictionary<string, int>();
                        tempErrorCount.Add(tempLog.errorCode, 1);
                        machineErrorCount.Add(tempLog.machineName, tempErrorCount);
                    }

                    if (cumCount >= Threshold)
                    {
                        foreach(KeyValuePair<string, Dictionary<string, int>> perMachineErrorCount in machineErrorCount)
                        {
                            string machinename = perMachineErrorCount.Key;
                            foreach(KeyValuePair<string, int> errorCount in perMachineErrorCount.Value)
                            {
                                printList tempitem;
                                tempitem.machineName = machinename;
                                tempitem.errorCode = errorCount.Key;
                                tempitem.count = errorCount.Value;
                                printCountPerMachine.Add(tempitem);
                            }
                        }
                        break;
                    }
                }
            }
            foreach(printList item in printCountPerMachine)
            {
                Console.WriteLine(item.machineName + "  " + item.errorCode + "  " + item.count);
            }
            Console.ReadLine();
        }
    }
}
