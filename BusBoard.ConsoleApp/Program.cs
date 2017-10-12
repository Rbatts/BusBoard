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

            var tflApi = new TflApi();
            var postcodeApi = new PostCodeApi();

            Console.Write("Enter your postcode.");
            string postcode = Console.ReadLine();
            PostcodeLocation location = postcodeApi.GetLongLat(postcode);
            string stopcode = tflApi.GetStopCodeNearLocation(location.longitude, location.latitude);
            var nextBusses = tflApi.GetNextArrivalsForStop(stopcode);
            List<Bus> bussesSorted = nextBusses.OrderBy(b => Convert.ToInt32(b.TimeToStation)).ToList();
            List<Bus> bussesFirstFive = bussesSorted.GetRange(0, 5);
            foreach (var Bus in bussesFirstFive)
            {
                int minutes = Bus.TimeToStation / 60;
                Console.WriteLine(Bus.Towards + " " + minutes + " " + Bus.DestinationName);
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
    class TflApi
    {
        public RestClient restClient = new RestClient();
        public RestRequest restRequest = new RestRequest();
        public TflApi()
        {
            restClient.BaseUrl = new Uri("https://api.tfl.gov.uk/");
            restClient.Authenticator = new HttpBasicAuthenticator("2e2a0c23", "dc17c694c0041fabbe7d31d366ddde94");
        }
        public List<Bus> GetNextArrivalsForStop(string stopCode)
        {
            restRequest.Resource = $"StopPoint/{stopCode}/Arrivals"; //string interperation ${} places variables within strings.
            var response = restClient.Execute<List<Bus>>(restRequest);
            var arrivals = response.Data;
            return arrivals;
        }
        public string GetStopCodeNearLocation(float longitude, float latitude)
        {
            restRequest.Resource = $"StopPoint?stopTypes=NaptanPublicBusCoachTram&lat={latitude}&lon={longitude}";
            IRestResponse<StopPointDetails> response = restClient.Execute<StopPointDetails>(restRequest);
            StopPointDetails stopPointDetails = response.Data;
            StopPoint FirstStopPoint = stopPointDetails.stopPoints[0];
            string stopcode = FirstStopPoint.naptanId;
            return stopcode;
        }

    }
    class PostCodeApi
    {
        public RestClient restClient = new RestClient();
        public RestRequest restRequest = new RestRequest();

        public PostCodeApi()
        {
            restClient.BaseUrl = new Uri("https://api.postcodes.io/");
        }
        public PostcodeLocation GetLongLat(string postcode)
        {
            restRequest.Resource = $"postcodes/{postcode}";
            var response = restClient.Execute<PostcodeDetails>(restRequest);
            var location = response.Data.result;
            return location;
        }

    }

    class PostcodeDetails
    {
        public PostcodeLocation result { get; set; }
    }

    class PostcodeLocation
    {
        public float longitude { get; set; }
        public float latitude { get; set; }
    }
    class StopPointDetails
    {
        public List<StopPoint> stopPoints { get; set; }
    }

    class StopPoint
    {
        public string naptanId { get; set; }
    }
}



