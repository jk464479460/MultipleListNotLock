using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Messaging;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RandomTest
{
    class Program
    {
        static void Main(string[] args)
        {
            for (var i = 0; i < 10; i++)
            {try
                {
                    var timeConsume = TestConsume();
                    using (var w = new StreamWriter("consume_normal.txt", true, Encoding.UTF8))
                    {
                        w.WriteLine(i+"===========");
                        w.WriteLine(timeConsume);
                    }
                }catch(Exception ex)
                {

                }
              
            }
            Console.WriteLine("process end");
            Console.ReadLine();
        }
        static long TestConsume()
        {
            var recordNumber = 1000000;
            var random = new RandomTestDist();
            var share1 = new ShareList(random);

            var insertEndTime = DateTime.Now;
            var task=Task.Factory.StartNew(() => {
                while (recordNumber-- > 0)
                {
                    share1.In(Guid.NewGuid().ToString() + " " + DateTime.Now);
                }
                if (recordNumber <= 0)
                    insertEndTime = DateTime.Now;
            });
            //task.Wait();
          
            var start = new Stopwatch();
            var startTime = DateTime.Now;
            start.Start();

            var i = 0;
            while (true)
            {
                Task.Factory.StartNew(() => { share1.PrintByMulti(); });
                Task.Factory.StartNew(() => { share1.PrintByMulti(); });
                Task.Factory.StartNew(() => { share1.PrintByMulti(); });
                Task.Factory.StartNew(() => { share1.PrintByMulti(); });
                Task.Factory.StartNew(() => { share1.PrintByMulti(); });
                Task.Factory.StartNew(() => { share1.PrintByMulti(); });
                Task.Factory.StartNew(() => { share1.PrintByMulti(); });
                Task.Factory.StartNew(() => { share1.PrintByMulti(); });
                Task.Factory.StartNew(() => { share1.PrintByMulti(); });
                Task.Factory.StartNew(() => { share1.PrintByMulti(); });
                Task.Factory.StartNew(() => { share1.PrintByMulti(); });
                Task.Factory.StartNew(() => { share1.PrintByMulti(); });
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("Do once ==============================================================================");
                //if (share1.IsStop(startTime)) break;
                if (share1.CheckAllEmpty()) i++;
                if (i == 1000) break;
            }
            //start.Stop();
            return start.ElapsedMilliseconds;
        }
    }
    class ShareList
    {
        private IDictionary<int, IList<string>> _shareQueue = new Dictionary<int, IList<string>>();
        static string path = "LearingMQ";
        RandomTestDist _random = null;

        public ShareList(RandomTestDist random)
        {
            _random = random;
        }
        public void In(string value)
        {
            var number = _random.GetQueueNumber();
            if (!_shareQueue.ContainsKey(number))
                _shareQueue.Add(number, new List<string>());
            _shareQueue[number].Add(value);
        }

        public void PrintByNumber(int number)
        {
            foreach (var str in _shareQueue[number])
                Console.WriteLine(str);
        }
        public void CntItem()
        {
            var cnt = 0;
            foreach(KeyValuePair<int, IList<string>> kv in _shareQueue)
            {
                cnt = cnt + kv.Value.Count;
            }
            Console.WriteLine(cnt);
        }

        public void PrintAll()
        {
            var number = _random.GetQueueNumber();
            if (!_shareQueue.ContainsKey(number)) return;
            var list = _shareQueue[number];
            if (list.Count == 0) return;
            var value = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            Console.WriteLine(value);
        }

        public void PrintByMulti()
        {
            var number = _random.GetQueueNumber();
            try
            {
                if (_shareQueue.ContainsKey(number))
                {
                    var list = _shareQueue[number];
                    if (list.Count == 0) return;
                    var value = list[list.Count - 1];
                    list.RemoveAt(list.Count - 1);
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine(value);
                }

                //RecordInMQ(value);
            }
            catch(Exception ex)
            {
               
            }
            
        }

        public bool IsStop(DateTime now)
        {
            var curr = DateTime.Now;
            if (curr.Subtract(now).Hours >= 3)
                return true;
            else return false;
        }

        public bool CheckAllEmpty()
        {
            var isempty = true;
            foreach (KeyValuePair<int, IList<string>> item in _shareQueue)
            {
                var currentEmpty=_shareQueue[item.Key].Count == 0 ? true : false;
                if(currentEmpty == false)
                isempty = currentEmpty;
            }
            return isempty;
        }

        void RecordInMQ(string value)
        {
            if (MessageQueue.Exists(@".\" + path))
            {
                try
                {
                    using (var mq = new MessageQueue(@".\" + path))
                    {
                        mq.Label = path;
                        mq.Send(value, "Leaning Hard");
                    }
                }
                catch(MessageQueueException ex)
                {
                    Console.WriteLine(ex.Message);
                }
               
            }
            else
            {
                MessageQueue.Create(@".\" + path);
            }
        }
    }
    class RandomTestDist
    {
        private const int begin = 0;
        private const int end = 160;
        private IDictionary<int, int> _statics = new Dictionary<int, int>();
        RNGCryptoServiceProvider csp = new RNGCryptoServiceProvider();
        Random random = null;

        public int GetQueueNumber()
        {
            byte[] byteCsp = new byte[10];
            csp.GetBytes(byteCsp);
            var seed = BitConverter.ToInt32(byteCsp, 0);
            random = new Random(seed);
            
            return random.Next(begin, end);
        }

        public void CountItem()
        {
            var get = random.Next(begin, end);
            if (_statics.ContainsKey(get))
                _statics[get] = _statics[get] + 1;
            else
                _statics.Add(get, 0);
        }
    }
}
