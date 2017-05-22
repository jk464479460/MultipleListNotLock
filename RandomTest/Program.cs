using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace RandomTest
{
    class Program
    {

        static void Main(string[] args)
        {
            var random = new RandomTestDist();
            var share1 = new ShareList();
            for (var i = 0; i <= 10000; i++)
                share1.In(random, Guid.NewGuid().ToString());
            Task.Factory.StartNew(()=> {
                while(true) try
                    {
                        share1.In(random, Guid.NewGuid().ToString()+" "+DateTime.Now);
                        share1.CntItem();
                        Thread.Sleep(10);
                    }
                    catch { }
                    
                //share1.CntItem();
            });
          
            //Console.WriteLine("complete data prepared: ");
            var start = new Stopwatch();
            var startTime = DateTime.Now;
            start.Start();

            try
            {
                while (true)
                {
                    try
                    {
                        //Task.Factory.StartNew(()=>share1.PrintAll(random));
                        Task.Factory.StartNew(() => { share1.PrintByMulti(random); });
                        Task.Factory.StartNew(() => { share1.PrintByMulti(random); });
                        Task.Factory.StartNew(() => { share1.PrintByMulti(random); });
                        Thread.Sleep(2);
                        Console.WriteLine("Do once");
                    }
                    catch(Exception ex)
                    {

                    }
                    //if (share1.IsStop(startTime)) break;
                    //if (share1.CheckAllEmpty()) break;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            start.Stop();
            Console.WriteLine("Time: " + start.ElapsedMilliseconds);
            Console.ReadLine();
        }
    }
    class ShareList
    {
        private IDictionary<int, IList<string>> _shareQueue = new Dictionary<int, IList<string>>();
        static string path = "LearingMQ";

        public void In(RandomTestDist random, string value)
        {
            var number = random.GetQueueNumber(DateTime.Now.Millisecond+100);
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

        public void PrintAll(RandomTestDist random)
        {
            var number = random.GetQueueNumber(100+DateTime.Now.Second);
            var list = _shareQueue[number];
            if (list.Count == 0) return;
            var value = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            Console.WriteLine(value);
        }
        public void PrintByMulti(RandomTestDist random)
        {
            //while (true)
            try
            {
                var number = random.GetQueueNumber(100+ DateTime.Now.Second);
                if (_shareQueue.ContainsKey(number)){
                    var list = _shareQueue[number];
                    if (list.Count == 0) return;
                    var value = list[list.Count - 1];
                    list.RemoveAt(list.Count - 1);
                    Console.WriteLine(value);
                }

                //RecordInMQ(value);
            }
            catch
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
        private const int end = 10;
        private IDictionary<int, int> _statics = new Dictionary<int, int>();
        private Random random = new Random(DateTime.Now.Second);

        public int GetQueueNumber(int seed= 12)
        {
            try
            {
                random = new Random(seed);
                return random.Next(begin, end);
            }
            catch
            {
                return 0;
            }
           
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
