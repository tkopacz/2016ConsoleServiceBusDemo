using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace pltkw32016ServiceBusClient.Controllers
{
    [DataContract(Namespace = "TKDemo")]
    public class Param
    {
        [DataMember]
        public int A { get; set; }
        [DataMember]
        public int B { get; set; }
    }
    public class CalcController : Controller
    {
        // GET: CalcSum
        public ActionResult Index()
        {
            return View();
        }
        TopicClient m_tc = TopicClient.Create("2016obliczenia");
        QueueClient m_qc = QueueClient.Create("2016wynik");

        public async Task<ActionResult> Add(int a = 5, int b = 12)
        {
            int result=0;
            Param p = new Param { A = a, B = b };
            BrokeredMessage msg = new BrokeredMessage(p);
            msg.Properties.Add("operation", 1);
            msg.ReplyToSessionId = Guid.NewGuid().ToString("N");
            await m_tc.SendAsync(msg);
            MessageSession session = m_qc.AcceptMessageSession(msg.ReplyToSessionId, TimeSpan.FromSeconds(60));
            msg = await session.ReceiveAsync();
            if (msg!=null) result = msg.GetBody<int>();
            return View(result);
        }
        public async Task<ActionResult> Sub(int a = 5, int b = 12)
        {
            int result=0;
            Param p = new Param { A = a, B = b };
            BrokeredMessage msg = new BrokeredMessage(p);
            msg.Properties.Add("operation", 2);
            msg.ReplyToSessionId = Guid.NewGuid().ToString("N");
            await m_tc.SendAsync(msg);
            MessageSession session = m_qc.AcceptMessageSession(msg.ReplyToSessionId, TimeSpan.FromSeconds(60));
            msg = await session.ReceiveAsync();
            if (msg != null) result = msg.GetBody<int>();
            return View(result);
        }
    }
}