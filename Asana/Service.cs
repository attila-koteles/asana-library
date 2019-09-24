using Asana.Mapping;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Asana
{
    public class Service
    {
        private HttpClient _client;
        private string _personalAccessToken;

        public Service(string personalAccessToken)
        {
            _personalAccessToken = personalAccessToken;
            _client = CreateClient();
        }

        private HttpClient CreateClient()
        {
            var client = new HttpClient()
            {
                BaseAddress = new Uri("https://app.asana.com/api/1.0/")
            };

            // specify to use TLS 1.2 as default connection
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;

            client.DefaultRequestHeaders.Add("Authorization", "Bearer " + _personalAccessToken);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            return client;
        }

        public async Task<List<User>> GetUsersAsync()
        {
            var response = await _client.GetAsync("users");
            response.EnsureSuccessStatusCode();

            var jObject = JObject.Parse(await response.Content.ReadAsStringAsync());
            var jToken = jObject.GetValue("data");
            List<User> users = (List<User>)jToken.ToObject(typeof(List<User>));
            return users;
        }

        public async Task<string> CreateTaskAsync(string workspaceId, string assignee, string name, string notes)
        {
            var req = new
            {
                data = new
                {
                    workspace = workspaceId,
                    assignee,
                    name,
                    notes
                }
            };

            var res = _client.PostAsync("tasks", GetJsonContent(req)).Result;
            res.EnsureSuccessStatusCode();

            return GetGidFromResponse(await res.Content.ReadAsStringAsync());
        }

        public async Task AddTagToTaskAsync(string taskGid, string tagId)
        {
            await PostObjectToUrl(
                $"tasks/{taskGid}/addTag",
                new
                {
                    data = new
                    {
                        tag = tagId
                    }
                });
        }

        public async Task<string> AddCommentToTaskAsync(string taskGid, string text)
        {
            return GetGidFromResponse(await PostObjectToUrl(
                $"tasks/{taskGid}/stories",
                new
                {
                    data = new
                    {
                        text
                    }
                }));
        }

        private StringContent GetJsonContent(object obj)
        {
            return new StringContent(JsonConvert.SerializeObject(obj), Encoding.UTF8, "application/json");
        }

        private async Task<string> PostObjectToUrl(string url, object obj)
        {
            var response = await _client.PostAsync(url, GetJsonContent(obj));
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        private string GetGidFromResponse(string responseJson)
        {
            var jObj = JObject.Parse(responseJson);

            var gid = jObj["data"]["gid"].ToString();
            if (gid == null)
                throw new NullReferenceException("Can't parse service result - no gid returned!");

            return gid;
        }
    }
}