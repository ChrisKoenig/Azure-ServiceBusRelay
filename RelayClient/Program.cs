using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Microsoft.ServiceBus;
using System.Configuration;

namespace RelayClient
{
    class Program
    {
        static void Main(string[] args)
        {
            var cf = new ChannelFactory<IStringReverserChannel>(
                new NetTcpRelayBinding(),
                new EndpointAddress(ServiceBusEnvironment.CreateServiceUri("sb", "ckdemo", "solver")));

            cf.Endpoint.Behaviors.Add(new TransportClientEndpointBehavior { 
                TokenProvider = TokenProvider.CreateSharedSecretTokenProvider(
                                    ConfigurationManager.AppSettings["issuerName"],
                                    ConfigurationManager.AppSettings["issuerSecret"]) 
            });

            using (var ch = cf.CreateChannel())
            {
                Console.WriteLine(ch.ReverseString("Chris Koenig"));
            }

        }
    }

    [ServiceContract(Namespace = "urn:chriskoenig")]
    interface IStringReverser
    {
        [OperationContract]
        string ReverseString(string input);
    }

    interface IStringReverserChannel : IStringReverser, IClientChannel { }

}
