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
            client.BaseAddress = new Uri(_baseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        private string _CreateQueryString(object obj)
        {
            var properties = from p in obj.GetType().GetTypeInfo().DeclaredProperties
                             where p.GetValue(obj, null) != null
                             select p.Name + "=" + WebUtility.UrlEncode(p.GetValue(obj, null).ToString());

            return String.Join("&", properties.ToArray());
        }

        public async Task<List<TransitStop>> GetAllStops()
        {
            using (var client = _CreateHttpClient())
            {
                HttpResponseMessage response = await client.GetAsync("GetAllStops");
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsAsync<List<TransitStop>>();
                return result;
            }
        }

        public async Task<List<TransitStop>> GetStops(string filter)
        {
            var url = String.Format("{0}?filter={1}", "GetStops", filter);

            using (var client = _CreateHttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsAsync<List<TransitStop>>();
                return result;
            }
        }

        public async Task<TransitStopInfo> GetStop(int id)
        {
            var url = String.Format("{0}?id={1}", "GetStop", id);

            using (var client = _CreateHttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsAsync<TransitStopInfo>();
                return result;
            }
        }

        public async Task<List<TransitRoute>> GetRoutes(string filter)
        {
            var url = String.Format("{0}?filter={1}", "GetRoutes", filter);

            using (var client = _CreateHttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsAsync<List<TransitRoute>>();
                return result;
            }
        }

        public async Task<TransitRoute> GetRoute(int id)
        {
            var url = String.Format("{0}?id={1}", "GetRoute", id);

            using (var client = _CreateHttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsAsync<TransitRoute>();
                return result;
            }
        }

        public async Task<TransitMetadata> GetMetadata()
        {
            using (var client = _CreateHttpClient())
            {
                HttpResponseMessage response = await client.GetAsync("GetMetadata");
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsAsync<TransitMetadata>();
                return result;
            }
        }

        public async Task<List<TransitSequenceGroup>> GetSchedule(int route_id, TransitDate when)
        {
            var url = String.Format("{0}?route_id={1}&{2}", "GetSchedule", route_id, _CreateQueryString(when));

            using (var client = _CreateHttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsAsync<List<TransitSequenceGroup>>();
                return result;
            }
        }

        public async Task<List<TransitSequenceInfo>> GetSequences(int route_id, TransitDate when)
        {
            var url = String.Format("{0}?route_id={1}&{2}", "GetSequences", route_id, _CreateQueryString(when));

            using (var client = _CreateHttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsAsync<List<TransitSequenceInfo>>();
                return result;
            }
        }

        public async Task<List<TransitSequenceElement>> GetSequence(int id)
        {
            var url = String.Format("{0}?id={1}", "GetSequence", id);

            using (var client = _CreateHttpClient())
            {
                HttpResponseMessage response = await client.GetAsync(url);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsAsync<List<TransitSequenceElement>>();
                return result;
            }
        }

        public async Task<TransitPlan> GetPlan(TransitPlanRequestParameters parameters)
        {
            using (var client = _CreateHttpClient())
            {
                HttpResponseMessage response = await client.PostAsJsonAsync("GetPlan", parameters);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsAsync<TransitPlan>();
                return result;
            }
        }

        public async Task<TransitPlan> GetSimplePlan(int from, int to, int year, int month, int day, int hour, int minute)
        {
            var url = String.Format(
                "{0}?from={1}&to={2}&year={3}&month={4}&day={5}&hour={6}&minute={7}", "GetSimplePlan",
                from, to, year, month, day, hour, minute);

            using (var client = _CreateHttpClient())
            {
                HttpResponseMessage response = await client.GetAsync("GetSimplePlan");
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadAsAsync<TransitPlan>();
                return result;
            }
        }
    }
}
