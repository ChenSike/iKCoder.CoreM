using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace CoreM
{
    public class ConsoleMessageItem
    {
        public string Message
        {
            set;
            get;
        }

        public string FromCommand
        {
            set;
            get;
        }

        public bool Result
        {
            set;
            get;
        }

    }

    public class ConsoleMessage
    {
        private Queue<ConsoleMessageItem> pool_messages = new Queue<ConsoleMessageItem>();
        private Thread threadMessageProcess;

        public void set_newMessage(ConsoleMessageItem newItem)
        {
            pool_messages.Enqueue(newItem);
        }

        public void start_process()
        {
            Console.WriteLine("Console Module Running...");
            threadMessageProcess = new Thread(new ThreadStart(process));
        }

        private void process()
        {
            while (true)
            {
                if (pool_messages.Count > 0)
                {
                    ConsoleMessageItem activeItem = pool_messages.Dequeue();
                    Console.WriteLine("[Message:" + activeItem.Message + " ]");
                    if (pool_messages.Count > 0)
                        continue;
                }
                Thread.Sleep(200);
            }
        }

    }
}
