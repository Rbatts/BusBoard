using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using System.Threading.Tasks;
using RestSharp.Authenticators;

namespace BusBoard.ConsoleApp
{
  class Program
  {
    static void Main(string[] args)
    {
            var client = new RestClient();
            client.BaseUrl = new Uri("https://api.tfl.gov.uk/");
            //client.Authenticator = new HttpBasicAuthenticator("2e2a0c23", "dc17c694c0041fabbe7d31d366ddde94");

            var request = new RestRequest();
            request.Resource = "StopPoint/490008660N/Arrivals";
            
            var response = client.Execute(request);
            
            Console.WriteLine(client.BuildUri(request));
            Console.WriteLine(response.Content);
            Console.ReadLine();
    }
  }
    class Bus
    {
        public string stopCode;
        public string arrival;
        public string route;
        public string destination;
     
    }
}
