using System;
using System.Net.Http.Headers;
using System.Text;
using System.Net.Http;
using System.Web;
using Newtonsoft.Json;
using System.Data;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web.Script;



namespace System.Web.Script
{ 
}
namespace CSHttpClientSample
{
    public class ResultSet
    {
        public int count { get; set; }
    }
    public class MarketingAirlane
    {
        public string companyShortName { get; set; }
    }
    public class DepartureAirport
    {
        public string locationCode { get; set; }
    }
    public class ArrivalAirport
    {
        public string locationCode { get; set; }
    }
    public class PricingInfoSum
    {
        public double totalPriceAllPassenger { get; set; }
        public double totalPriceOnePassenger { get; set; }
        public double baseFare { get; set; }
        public double taxSurcharge { get; set; }
        public string currencyCode { get; set; }
        public string productClass { get; set; }
    }
    public class Deeplink
    {
        public string href { get; set; }
    }
    public class OutBoundFlight
    {
        public string id { get; set; }
        public string departureDateTime { get; set; }
        public string arrivalDateTime { get; set; }
        public MarketingAirlane marketingAirlane { get; set; }
        public int flightNumber { get; set; }
        public DepartureAirport departureAirport { get; set; }
        public ArrivalAirport arrivalAirport { get; set; }
    }
    public class FlightOffer
    {
        public OutBoundFlight outboundflight {get; set;}
        public PricingInfoSum pricingInfoSum { get; set; }
        public Deeplink deeplink { get; set; }
    }
    public class RootObject
    {
        public ResultSet resultset { get; set; }
        public FlightOffer[] flightOffer { get; set; }
    }
    class FlightInfo
    {
        public string id { get; set; }
        public string count { get; set; }
        public string airportDepartureCode { get; set; }
        public string airportArrivalCode { get; set; }
        public string departureName { get; set; }
        public string arrivalName { get; set; }
        public string departureDate { get; set; }
        public string arrivalDate { get; set; }
        public double priceOnePassanger { get; set; }
        public string currency { get; set; }
        public int flightNumber { get; set; }
        public string href { get; set; }

        public double childPrice { get; set; }
        public double teenPrice { get; set; }
        public double adultPrice { get; set; }
        public double childAfterDiscount { get; set; }
        public double teenAfterDiscount { get; set; }
        public double adultAfterDiscount { get; set; }
        public string flightDuration { get; set; }
        public string marketingAirlane { get; set; }
        
    }

    static class Program
    {
        static void Main()
        {
            //flight_offers_from_to("EIN","ACE", "202006");
            //available_routes_from("EIN");
            all_from_airport_date("EIN", "20200613");
            Console.ReadKey();
        }

        public static List<String> splitIATA(string iata)
        {
            int counter = 0;
            string one_iata_code = "";
            List<String> iata_list = new List<String>();
            for(int i=0; i<iata.Length;i++)
            {
                if (counter % 3 == 0 && i != 0)
                {
                    iata_list.Add(one_iata_code);
                    counter = 0;
                    one_iata_code = "";
                }
                one_iata_code += iata[i];
                counter++;
            }
            return iata_list;
        }

        static async Task<string> available_routes_from(string originIATA)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Add("X-Deeplink-CultureCode", "en-EU");
            client.DefaultRequestHeaders.Add("apikey", "5bf3eb90c9864303b1761af0c08ab259");

            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["Origin"] = originIATA;
            var uri = "https://api.transavia.com/v3/routes/?" + queryString;
            var response_txt = client.GetAsync(uri).Result;
            HttpContent stream = response_txt.Content;
            var data = await stream.ReadAsStringAsync();

            string dataS = data;
            dynamic routeJson = JsonConvert.DeserializeObject(dataS);
            dynamic selectedAirport = null;
            List<string> available_routes = new List<string>();
            string available_routes_string = "";
            foreach (var route in routeJson)
            {
                selectedAirport = route;
                available_routes.Add((selectedAirport.destination.id).ToString());
                available_routes_string += (selectedAirport.destination.id).ToString();
            }
            //Console.WriteLine("Available routes from " + originIATA + ": ");
            return available_routes_string; 
        }

        static List<FlightInfo> json_to_listclass(string data)
        {
            List<FlightInfo> flghtInfoList = new List<FlightInfo>();
            var flightsJson = JsonConvert.DeserializeObject<RootObject>(data);
            if(flightsJson!=null)
            {
                for (int i = 0; i < flightsJson.resultset.count; i++)
                {
                    FlightInfo f = new FlightInfo();
                    f.airportDepartureCode = flightsJson.flightOffer[i].outboundflight.departureAirport.locationCode;
                    f.airportArrivalCode = flightsJson.flightOffer[i].outboundflight.arrivalAirport.locationCode;
                    f.departureDate = flightsJson.flightOffer[i].outboundflight.departureDateTime;
                    f.arrivalDate = flightsJson.flightOffer[i].outboundflight.arrivalDateTime;
                    f.flightNumber = flightsJson.flightOffer[i].outboundflight.flightNumber;
                    f.id = flightsJson.flightOffer[i].outboundflight.id;
                    f.priceOnePassanger = flightsJson.flightOffer[i].pricingInfoSum.totalPriceOnePassenger;
                    f.href = flightsJson.flightOffer[i].deeplink.href;
                    f.currency = flightsJson.flightOffer[i].pricingInfoSum.currencyCode;
                    //f.marketingAirlane = flightsJson.flightOffer[i].outboundflight.marketingAirlane.companyShortName;
                    flghtInfoList.Add(f);
                }
            }
            
            return flghtInfoList;
        }

        static void print_flights(List<FlightInfo> flghtInfoList)
        {
            foreach (FlightInfo f in flghtInfoList)
            {
                Console.WriteLine("-----------------flight-------------");
                Console.WriteLine(f.id);
                Console.WriteLine(f.airportArrivalCode + " - " + f.airportDepartureCode);
                Console.WriteLine(f.departureDate + " - " + f.arrivalDate);
                Console.WriteLine(f.flightNumber);
                Console.WriteLine(f.marketingAirlane);
                Console.WriteLine(f.priceOnePassanger + " " + f.currency);
                Console.WriteLine(f.href);
            }
        }
       
        static async void flight_offers_from_to(string originIATA, string destinationIATA, string originData)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);

            // Request headers
            client.DefaultRequestHeaders.Add("X-Deeplink-CultureCode", "en-EU");
            client.DefaultRequestHeaders.Add("apikey", "5bf3eb90c9864303b1761af0c08ab259");

            // Request parameters
            queryString["Origin"] = originIATA;
            queryString["Destination"] = destinationIATA;
            queryString["OriginDepartureDate"] = originData;

            var uri3 = "https://api.transavia.com/v1/flightoffers/?" + queryString;

            var response_txt = client.GetAsync(uri3).Result;
            HttpContent stream = response_txt.Content;
            var data = await stream.ReadAsStringAsync();

            List<FlightInfo> flghtInfoList = json_to_listclass(data);
            print_flights(flghtInfoList);
            
        }

        static async void all_from_airport_date(string originIATA, string originData)
        {
            var client = new HttpClient();
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["Origin"] = originIATA;
            queryString["OriginDepartureDate"] = originData;
            // Request headers
            client.DefaultRequestHeaders.Add("X-Deeplink-CultureCode", "en-EU");
            client.DefaultRequestHeaders.Add("apikey", "5bf3eb90c9864303b1761af0c08ab259");

            List<string> list_IATA = new List<string>();
            List<FlightInfo> flghtInfoList = new List<FlightInfo>();
            string airports = await(available_routes_from(originIATA));
            list_IATA = splitIATA(airports);

            foreach(string s in list_IATA)
            {                
                queryString["Destination"] = s;           
                var uri3 = "https://api.transavia.com/v1/flightoffers/?" + queryString;

                var response_txt = client.GetAsync(uri3).Result;
                HttpContent stream = response_txt.Content;
                var data = await stream.ReadAsStringAsync();

                if(data!=null)
                    flghtInfoList.AddRange(json_to_listclass(data));
            }
            print_flights(flghtInfoList);

        }


    }
}