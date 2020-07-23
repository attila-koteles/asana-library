using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace Asana.Mapping
{
    public class TypeAheadResult
    {
        [JsonProperty("gid")]
        public string Gid { get; set; } = "";

        [JsonProperty("name")]
        public string Name { get; set; } = "";

        [JsonProperty("resource_type")]
        public string ResourceType { get; set; } = "";
    }
}
