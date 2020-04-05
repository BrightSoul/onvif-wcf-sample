using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Threading.Tasks;
using OnvifDemo.Proxies.Ptz;
using System.Text;
using OnvifDemo.Proxies.Device;
using OnvifDemo.Proxies.Media;
using System.Linq;
using System;

namespace OnvifDemo
{
    class Program
    {
        const string deviceEndpoint = "http://192.168.1.22:10000/onvif/device_service"; //Modifica questo

        static async Task Main(string[] args)
        {
            string mediaEndpoint = await GetEndpointForService(deviceEndpoint, "http://www.onvif.org/ver10/media/wsdl");
            Console.WriteLine($"Il media endpoint è: {mediaEndpoint}");

            string ptzEndpoint = await GetEndpointForService(deviceEndpoint, "http://www.onvif.org/ver20/ptz/wsdl");
            Console.WriteLine($"Il PTZ endpoint è: {ptzEndpoint}");

            string profileName = await GetFirstProfileName(mediaEndpoint);
            Console.WriteLine($"Il nome del primo profilo esposto è: {profileName}");

            //Spostiamo l'inquadratura
            float x = 0.5f;
            float y = 0.5f;
            float zoom = 0.5f;
            await AbsoluteMoveAsync(ptzEndpoint, profileName, x, y, zoom);
            Console.WriteLine($"L'inquadratura è stata spostata su x={x} y={y} zoom={zoom}");

            Console.WriteLine("Premere INVIO per terminare");
            Console.ReadLine();
        }

        private async static Task<string> GetEndpointForService(string deviceEndpoint, string serviceNamespace)
        {
            var client = CreateDeviceClient(deviceEndpoint);
            var services = await client.GetServicesAsync(IncludeCapability: false);
            var endpoint = services.Service.Single(service => service.Namespace == serviceNamespace);
            return endpoint.XAddr;
        }

        private async static Task<string> GetFirstProfileName(string mediaEndpoint)
        {
            var client = CreateMediaClient(mediaEndpoint);
            var profiles = await client.GetProfilesAsync();
            return profiles.Profiles.First().Name;
        }

        private static async Task AbsoluteMoveAsync(string ptzEndpoint, string profileName, float x, float y, float zoom)
        {
            //Creazione del client
            var client = CreatePtzClient(ptzEndpoint);

            //Definizione dei parametri
            var vector = new PTZVector {
                PanTilt = new Proxies.Ptz.Vector2D { x = x, y = y },
                Zoom = new Proxies.Ptz.Vector1D { x = zoom }
            };
            var speed = new Proxies.Ptz.PTZSpeed {
                PanTilt = new Proxies.Ptz.Vector2D { x = 1.0f, y = 1.0f },
                Zoom = new Proxies.Ptz.Vector1D { x = 1.0f }
            };

            //Invocazione dell'operazione esposta dal servizio
            await client.AbsoluteMoveAsync(profileName, vector, speed);
        }

        private static DeviceClient CreateDeviceClient(string deviceEndpoint)
        {
            //A
            var address = new EndpointAddress(deviceEndpoint);
            //B
            var binding = CreateBinding();
            //C
            var client = new DeviceClient(binding, address);

            return client;
        }

        private static MediaClient CreateMediaClient(string mediaEndpoint)
        {
            //A
            var address = new EndpointAddress(mediaEndpoint);
            //B
            var binding = CreateBinding();
            //C
            var client = new MediaClient(binding, address);

            return client;
        }

        private static PTZClient CreatePtzClient(string ptzEndpoint)
        {
            //A
            var address = new EndpointAddress(ptzEndpoint);
            //B
            var binding = CreateBinding();
            //C
            var client = new PTZClient(binding, address);

            return client;
        }

        private static Binding CreateBinding()
        {
            return new CustomBinding(new BindingElement[] { new TextMessageEncodingBindingElement(MessageVersion.Soap12WSAddressing10, Encoding.UTF8), new HttpTransportBindingElement() });
        }
    }
}
