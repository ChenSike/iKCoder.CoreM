using System;
using iKCoderSDK;
using System.Collections.Generic;
using System.Xml;

namespace CoreM
{
    class Program
    {

        public static string key_db_ikcoder_store = "ikcoder_store";
        public static iKCoderSDK.Basic_Config coreMconfig = new Basic_Config();
        public static ConsoleMessage obj_message = new ConsoleMessage();

        static void Main(string[] args)
        {
            Console.WriteLine("CoreM 1.0 2018 copyright by iKCoder.LTD.ShenZhen.");
            Console.WriteLine("Start services...");
            Console.WriteLine("[CoreM will keep running until this process closed.]");
            obj_message.start_process();
            coreMconfig.DoOpen("coreMconfig.xml");
            Console.WriteLine("[Load module:M_CheckStore]");
                       
            
        }
    }
}
