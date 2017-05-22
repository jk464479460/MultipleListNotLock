using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;

namespace TestRand
{
    class Program
    {
        static void Main(string[] args)
        {
            for(var i = 0; i<10; i++)
            {
                var timeConsume = TimeConsume();
                using (var w=new StreamWriter("consume_normal.txt", true, Encoding.UTF8))
                {
                    w.WriteLine(timeConsume);
                }
            }
            Console.WriteLine("process end");
            Console.ReadLine();
        }

        static long TimeConsume()
        {
            var list = new List<string>();
            for (var i = 0; i < 1000000; i++)
                list.Add(Guid.NewGuid().ToString());
            var start = new Stopwatch();
            var startTime = DateTime.Now;
            start.Start();
            foreach (var i in list)
                Console.WriteLine("Do once");
            start.Stop();

            return start.ElapsedMilliseconds;
        }

        static void Main2(string[] args)
        { RNGCryptoServiceProvider csp = new RNGCryptoServiceProvider();

            for (var i = 0; i < 10; i++)
            {

                byte[] byteCsp = new byte[10];
                csp.GetBytes(byteCsp);
                var ran = new Random(BitConverter.ToInt32(byteCsp, 0));
                Console.Write(ran.Next(0, 10) + " ");
                Thread.Sleep(1);
            }
            Console.WriteLine();
        }
    }
}
