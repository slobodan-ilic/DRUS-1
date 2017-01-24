﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace PubSubClient
{
    public class Client
    {
        public delegate void MyEventCallbackHandler(string Id, int Value);
        public static event MyEventCallbackHandler MyEventCallbackEvent;
        private InstanceContext context = null;
        private PubSubServiceReference.PubSubServiceClient client = null;

        [CallbackBehaviorAttribute(UseSynchronizationContext = false)]
        public class ServiceCallback : PubSubServiceReference.IPubSubServiceCallback
        {
            public void ValueChange(string Id, int Value)
            {
                Client.MyEventCallbackEvent(Id, Value);
            }
        }

        public Client()
        {
            context = new InstanceContext(new ServiceCallback());
            client = new PubSubServiceReference.PubSubServiceClient(context);
            string[] publishers = client.ListAllPublishers();

            MyEventCallbackHandler callbackHandler = new MyEventCallbackHandler(Print);
            MyEventCallbackEvent += callbackHandler;

            string reply = "y";

            while (reply.Contains('y'))
            {
                Console.WriteLine("List of stations by ID:");

                for (int i = 0; i < publishers.Length; i++ )
                {
                    Console.WriteLine("{0}. {1}", i, publishers[i]);
                }

                Console.WriteLine("Enter station ID: ");
                string ID = Console.ReadLine();
                string msg = client.Subscribe(ID);
                Console.WriteLine(msg);
                Console.WriteLine("Do you want to add another station?[y/n]");
                reply = Console.ReadLine();
            }

            client.ClientInit();
            Console.WriteLine("Client ready, starting transmision...");
        }

        public void Print(string Id, int Value)
        {
            Console.WriteLine("Publisher: {0} Value: {1}", Id, Value);
        }

        public void Close()
        {
            client.UnsubscribeAll();
        }
    }
}
