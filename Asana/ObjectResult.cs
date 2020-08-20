using Newtonsoft.Json;

namespace Asana
{
    public class ObjectResult
    {
        [JsonProperty("gid")]
        public string Gid { get; set; } = "";

        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("resource_type")]
        public string ResourceType { get; set; } = "";
    }
}
