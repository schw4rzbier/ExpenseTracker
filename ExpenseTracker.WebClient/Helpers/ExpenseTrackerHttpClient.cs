using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace ExpenseTracker.WebClient.Helpers
{
    public static class ExpenseTrackerHttpClient
    {
        public static HttpClient GetClient(string requestedVersion = null)
        {
            var client = new HttpClient();

            client.BaseAddress = new Uri(ExpenseTrackerConstants.ExpenseTrackerAPI);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));

            if (requestedVersion != null)
            {
                // through a custom request header
                //client.DefaultRequestHeaders.Add("api-version", requestedVersion);

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/vnd.expensetrackerapi.v" + requestedVersion + "+json"));
            }

            return client;
        }
    }
}
