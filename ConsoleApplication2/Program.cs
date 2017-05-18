using System;
using System.IO;
using System.Messaging;
using System.Text;
using System.Threading;

namespace ConsoleApplication4
{
    class Program
    {
        static string path = "LearingMQ";
        static void Main(string[] args)
        {
            while (true)
            {
                if (MessageQueue.Exists(@".\" + path))
                {
                    using (var mq = new MessageQueue(@".\" + path))
                    {
                        mq.Formatter = new XmlMessageFormatter(new string[] { "System.String" });

                        var firstMsg = mq.Receive();
                        using (var w = new StreamWriter(@"t1.txt", true, Encoding.UTF8))
                        {
                            w.WriteLine(firstMsg.Body);
                        }
                    }
                }
                Thread.Sleep(20);
            }

        }
    }
}
