using System;
using System.Collections.Generic;
using System.Linq;
using System.Messaging;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleApplication1
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
                        mq.Label = path;
                        mq.Send("MSMA Message "+ Guid.NewGuid(), "Leaning Hard");
                    }
                }
                else
                {
                    MessageQueue.Create(@".\" + path);
                }
                Thread.Sleep(10);
            }
        }
    }
}
