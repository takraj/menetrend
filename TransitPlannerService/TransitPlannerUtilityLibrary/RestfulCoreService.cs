using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using TransitPlannerContracts;
using PortableUtilityLibrary;

namespace TransitPlannerUtilityLibrary
{
    public class RestfulCoreService
    {
        private string _baseUrl;

        public RestfulCoreService(string baseUrl)
        {
            _baseUrl = baseUrl;
        }

        private HttpClient _CreateHttpClient()
        {
            var client = new HttpClient();
            client.Timeout = new TimeSpan(0, 0, 30);
            client.BaseAddress = new Uri(_baseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        public List<TransitStop> GetAllStops()
        {
            using (var client = _CreateHttpClient())
            {
                HttpResponseMessage response = client.GetAsync("GetAllStops").Result;
                response.EnsureSuccessStatusCode();

                var result = response.Content.ReadAsAsync<List<TransitStop>>().Result;
                return result;
            }
        }

        public List<TransitStop> GetStops(string filter)
        {
            var url = String.Format("{0}?filter={1}", "GetStops", filter);

            using (var client = _CreateHttpClient())
            {
                HttpResponseMessage response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                var result = response.Content.ReadAsAsync<List<TransitStop>>().Result;
                return result;
            }
        }

        public TransitStopInfo GetStop(int id)
        {
            var url = String.Format("{0}?id={1}", "GetStop", id);

            using (var client = _CreateHttpClient())
            {
                HttpResponseMessage response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                var result = response.Content.ReadAsAsync<TransitStopInfo>().Result;
                return result;
            }
        }

        public List<TransitRoute> GetRoutes(string filter)
        {
            var url = String.Format("{0}?filter={1}", "GetRoutes", filter);

            using (var client = _CreateHttpClient())
            {
                HttpResponseMessage response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                var result = response.Content.ReadAsAsync<List<TransitRoute>>().Result;
                return result;
            }
        }

        public TransitRoute GetRoute(int id)
        {
            var url = String.Format("{0}?id={1}", "GetRoute", id);

            using (var client = _CreateHttpClient())
            {
                HttpResponseMessage response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                var result = response.Content.ReadAsAsync<TransitRoute>().Result;
                return result;
            }
        }

        public TransitMetadata GetMetadata()
        {
            using (var client = _CreateHttpClient())
            {
                HttpResponseMessage response = client.GetAsync("GetMetadata").Result;
                response.EnsureSuccessStatusCode();

                var result = response.Content.ReadAsAsync<TransitMetadata>().Result;
                return result;
            }
        }

        public List<TransitSequenceGroup> GetSchedule(int route_id, TransitDate when)
        {
            var url = String.Format(
                "{0}?route_id={1}&year={2}&month={3}&day={4}", "GetSchedule",
                route_id, when.year, when.month, when.day);

            using (var client = _CreateHttpClient())
            {
                HttpResponseMessage response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                var result = response.Content.ReadAsAsync<List<TransitSequenceGroup>>().Result;
                return result;
            }
        }

        public List<TransitSequenceInfo> GetSequences(int route_id, TransitDate when)
        {
            var url = String.Format(
                "{0}?route_id={1}&year={2}&month={3}&day={4}", "GetSequences",
                route_id, when.year, when.month, when.day);

            using (var client = _CreateHttpClient())
            {
                HttpResponseMessage response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                var result = response.Content.ReadAsAsync<List<TransitSequenceInfo>>().Result;
                return result;
            }
        }

        public List<TransitSequenceElement> GetSequence(int id)
        {
            var url = String.Format("{0}?id={1}", "GetSequence", id);

            using (var client = _CreateHttpClient())
            {
                HttpResponseMessage response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                var result = response.Content.ReadAsAsync<List<TransitSequenceElement>>().Result;
                return result;
            }
        }

        public TransitPlan GetPlan(TransitPlanRequestParameters parameters)
        {
            using (var client = _CreateHttpClient())
            {
                HttpResponseMessage response = client.PostAsJsonAsync("GetPlan", parameters).Result;
                response.EnsureSuccessStatusCode();

                var result = response.Content.ReadAsAsync<TransitPlan>().Result;
                return result;
            }
        }

        public TransitPlan GetSimplePlan(int from, int to, int year, int month, int day, int hour, int minute)
        {
            var url = String.Format(
                "{0}?from={1}&to={2}&year={3}&month={4}&day={5}&hour={6}&minute={7}", "GetSimplePlan",
                from, to, year, month, day, hour, minute);

            using (var client = _CreateHttpClient())
            {
                HttpResponseMessage response = client.GetAsync(url).Result;
                response.EnsureSuccessStatusCode();

                var result = response.Content.ReadAsAsync<TransitPlan>().Result;
                return result;
            }
        }

        public TransitStop GetNearestStop(double latitude, double longitude)
        {
            var all_stops = this.GetAllStops();
            var heap = new HeapDict<TransitStop, double>();
            foreach (var stop in all_stops)
            {
                var distance = Haversine.GetDistanceBetween(latitude, longitude, stop.latitude, stop.longitude);
                heap.SetItem(stop, distance);
            }
            var result = heap.PeekItem();
            return result.Key;
        }
    }
}
