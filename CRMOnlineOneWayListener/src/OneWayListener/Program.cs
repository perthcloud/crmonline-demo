using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;
using System.Text;
using Microsoft.ServiceBus;
using Microsoft.Xrm.Sdk;


/**
 * This sample is based on the Dynamics CRM 2011 Developer Training Kit Azure Hands-on lab, http://www.microsoft.com/download/en/details.aspx?id=23416
 */
namespace OneWayListener
{

    class CRMOnlineOneWayListenerService : IServiceEndpointPlugin
    {
        public void Execute(RemoteExecutionContext executionContext)
        {
            Console.WriteLine("Event received");
        }

        void PrintExecutionContext(RemoteExecutionContext context)
        {
            
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Listen();
        }

        static void Listen()
        {
            string serviceNamespace;
            string serviceIdentityName;
            string serviceIdentityKey;


            Console.WriteLine("Service Namespace:");
            serviceNamespace = Console.ReadLine();
            
            Console.WriteLine("Service Identity:");
            serviceIdentityName = Console.ReadLine();            
            Console.WriteLine("Service Identity Key:");
            serviceIdentityKey = Console.ReadLine();
            

            TransportClientEndpointBehavior behavior = new TransportClientEndpointBehavior();
            behavior.TokenProvider = SharedSecretTokenProvider.CreateSharedSecretTokenProvider(serviceIdentityName, serviceIdentityKey);


            var address = ServiceBusEnvironment.CreateServiceUri(Uri.UriSchemeHttps, serviceNamespace, "Demo/OneWayListener");
            

            // binding
            var binding = new WS2007HttpRelayBinding();
            binding.Security.Mode = EndToEndSecurityMode.Transport;

            var host = new ServiceHost(typeof (CRMOnlineOneWayListenerService));

            var endPoint = host.AddServiceEndpoint(typeof (IServiceEndpointPlugin), binding, address);

            var serviceRegistrySettings = new ServiceRegistrySettings(DiscoveryType.Private);

            endPoint.Behaviors.Add(serviceRegistrySettings);
            endPoint.Behaviors.Add(behavior);


            try
            {
                // Open the service.
                host.Open();
                Console.WriteLine("Host Open. Listening on endpoint Address: " + address);
            }
            catch (TimeoutException timeout)
            {
                Console.WriteLine("Opening the service timed out, details below:");
                Console.WriteLine(timeout.Message);
            }

            Console.WriteLine("Press [Enter] to exit");
            Console.ReadLine();

            // Close the service.
            Console.Write("Closing the service host...");
            host.Close();
            Console.WriteLine(" done.");





        }
    }
}
