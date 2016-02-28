using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleServiceBusClient
{
    [DataContract(Namespace = "TKDemo")]
    public class Param
    {
        [DataMember]
        public int A { get; set; }
        [DataMember]
        public int B { get; set; }
    }
    class Program
    {
        static SubscriptionClient m_clientLog = SubscriptionClient.Create("2016obliczenia", "log");
        static SubscriptionClient m_clientAdd = SubscriptionClient.Create("2016obliczenia", "dodaj");
        static SubscriptionClient m_clientSub = SubscriptionClient.Create("2016obliczenia", "odejmij");
        static QueueClient m_queue = QueueClient.Create("2016wynik");
        static void Main(string[] args)
        {
            Task.Run(() => processLog());
            Task.Run(() => processAdd());
            Task.Run(() => processSub());
            Console.WriteLine("Enter = End");
            Console.ReadLine();
        }

        static async void processLog()
        {
            while(true)
            {
                var msg = await m_clientLog.ReceiveAsync();
                if (msg!=null)
                {
                    Param p = msg.GetBody<Param>();
                    Console.WriteLine($"log {DateTime.Now.Ticks}: {p.A}, {p.B}");
                    await m_clientLog.CompleteAsync(msg.LockToken);
                }
            }
        }

        static async void processAdd()
        {
            while (true)
            {
                var msg = await m_clientAdd.ReceiveAsync();
                if (msg != null)
                {
                    Param p = msg.GetBody<Param>();
                    Console.WriteLine($"add {DateTime.Now.Ticks}: {p.A}, {p.B}");
                    BrokeredMessage msgResp = new BrokeredMessage(p.A + p.B);
                    msgResp.SessionId = msg.ReplyToSessionId;
                    await m_queue.SendAsync(msgResp);
                    await m_clientAdd.CompleteAsync(msg.LockToken);
                }
            }

        }
        static async void processSub()
        {
            while (true)
            {
                var msg = await m_clientSub.ReceiveAsync();
                if (msg != null)
                {
                    Param p = msg.GetBody<Param>();
                    Console.WriteLine($"sub {DateTime.Now.Ticks}: {p.A}, {p.B}");
                    BrokeredMessage msgResp = new BrokeredMessage(p.A - p.B);
                    msgResp.SessionId = msg.ReplyToSessionId;
                    await m_queue.SendAsync(msgResp);
                    await m_clientSub.CompleteAsync(msg.LockToken);
                }
            }

        }
    }
}
