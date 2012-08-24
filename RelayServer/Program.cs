using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using Microsoft.ServiceBus;
using System.Configuration;

namespace RelayServer
{
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost sh = new ServiceHost(typeof(StringReverser));

            sh.AddServiceEndpoint(
               typeof(IStringReverser), new NetTcpBinding(),
               "net.tcp://localhost:9358/reverse");

            sh.AddServiceEndpoint(
               typeof(IStringReverser), new NetTcpRelayBinding(),
               ServiceBusEnvironment.CreateServiceUri("sb", "ckdemo", "solver"))
                .Behaviors.Add(new TransportClientEndpointBehavior
                {
                    TokenProvider = TokenProvider.CreateSharedSecretTokenProvider(
                    ConfigurationManager.AppSettings["issuerName"],
                    ConfigurationManager.AppSettings["issuerSecret"])
                });

            sh.Open();

            Console.WriteLine("Press ENTER to close");
            Console.ReadLine();

            sh.Close();
        }
    }

    [ServiceContract(Namespace="urn:chriskoenig")]
    interface IStringReverser
    {
        [OperationContract]
        string ReverseString(string input);
    }

    interface IStringReverserChannel : IStringReverser, IClientChannel { }

    public class StringReverser : IStringReverser
    {
        public string ReverseString(string input)
        {
            var a1 = input.ToCharArray();
            Array.Reverse(a1);
            var output = String.Join("", a1);
            Console.WriteLine("{0} => {1}", input, output);
            return output;
        }
    }
}

