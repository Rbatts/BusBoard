using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RestSharp;
using System.Threading.Tasks;
using RestSharp.Authenticators;
using System.Net;

namespace BusBoard.ConsoleApp
{
  class Program
  {
    static void Main(string[] args)
    {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            var client = new RestClient();
            client.BaseUrl = new Uri("https://api.tfl.gov.uk/");
            client.Authenticator = new HttpBasicAuthenticator("2e2a0c23", "dc17c694c0041fabbe7d31d366ddde94");

            Console.Write("Enter your postcode.");
            string uResponse = Console.ReadLine();

                var request = new RestRequest();
                request.Resource = $"StopPoint/{uResponse}/Arrivals"; //string interperation ${} places variables within strings.

                var response = client.Execute(request);
                var JSONFromApi = response.Content;
            try
            {
                var busses = JsonConvert.DeserializeObject<List<Bus>>(JSONFromApi);
                List<Bus> bussesSorted = busses.OrderBy(b => Convert.ToInt32(b.TimeToStation)).ToList();
                List<Bus> bussesFirstFive = bussesSorted.GetRange(0, 5);
                foreach (var Bus in bussesFirstFive)
                {
                    int minutes = Bus.TimeToStation / 60;
                    Console.WriteLine(Bus.Towards + " " + minutes + " " + Bus.DestinationName);

                }
            }
            catch
            {
                Console.WriteLine("You have not entered an invalid bus stop code.");
                Console.ReadLine();
            }
            Console.ReadLine();   
    }
  }
    class Bus
    {
        public int TimeToStation { get; set; }
       
        public string Towards { get; set; }
      
        public string DestinationName { get; set; }

    }

}



