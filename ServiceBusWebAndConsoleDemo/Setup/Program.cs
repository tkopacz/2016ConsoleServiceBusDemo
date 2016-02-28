using Microsoft.ServiceBus;
using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Setup
{
    class Program
    {
        static void Main(string[] args)
        {
            NamespaceManager manager = NamespaceManager.Create(); // Automatycznie bierze informacje z App.config
            Console.WriteLine(manager.Address.ToString());
            //Wolę na początku - wygodniej "zaczynamy" zawsze od zera
            manager.DeleteTopic("2016obliczenia"); //Kasuje temat i subskrypcje
            manager.DeleteQueue("2016wynik");

            //Tworzenie Topics - tematu
            TopicDescription td = new TopicDescription("2016obliczenia");

            //Nie przewidujemy dużego ruchu nie wymagamy partycjonowania
            td.EnablePartitioning = false;
            //Wymagamy wykrywania duplikatów - by klient 2 razy nie wysłał tego samego polecenia
            td.RequiresDuplicateDetection = true;
            //Nie pozwalamy na tematy tylko w pamięci; chcemy żeby klient był pewien że wysłał wiadomość = wiadomość zostanie przetworzona
            td.EnableExpress = false;
            manager.CreateTopic(td); //Tworzenie tematu

            //Suma i średnia będzie wyliczana gdy opowiednia własciwość zostanie zdefiniowana
            manager.CreateSubscription("2016obliczenia", "log", new SqlFilter("1=1"));
            manager.CreateSubscription("2016obliczenia", "dodaj", new SqlFilter("operation=1"));
            manager.CreateSubscription("2016obliczenia", "odejmij", new SqlFilter("operation=2"));

            QueueDescription qd = new QueueDescription("2016wynik");
            qd.RequiresSession = true;
            manager.CreateQueue(qd);
            Console.WriteLine("Done, Enter");
            Console.ReadLine();
        }
    }
}
