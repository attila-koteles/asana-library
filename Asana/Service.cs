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
            client.DefaultRequestHeaders.Add("Asana-Disable", "string_ids");
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

        public async Task<long> CreateTaskAsync(long workspaceId, long assignee, string name, string notes)
        {
            var req = new
            {
                data = new
                {
                    workspace = workspaceId,
                    assignee = assignee,
                    name = name,
                    notes = notes
                }
            };

            var res = _client.PostAsync("tasks", GetJsonContent(req)).Result;
            res.EnsureSuccessStatusCode();

            return GetIdFromResponse(await res.Content.ReadAsStringAsync());
        }

        public async Task AddTagToTaskAsync(long taskId, long tagId)
        {
            await PostObjectToUrl(
                $"tasks/{taskId}/addTag",
                new
                {
                    data = new
                    {
                        tag = tagId
                    }
                });
        }

        public async Task<long> AddCommentToTaskAsync(long taskId, string text)
        {
            return GetIdFromResponse(await PostObjectToUrl(
                $"tasks/{taskId}/stories",
                new
                {
                    data = new
                    {
                        text = text
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

        private long GetIdFromResponse(string responseJson)
        {
            var jObj = JObject.Parse(responseJson);

            var id = jObj["data"]["id"].ToString();
            if (id == null)
                throw new NullReferenceException("Can't parse service result - no ID returned!");

            return long.Parse(id);
        }
    }
}