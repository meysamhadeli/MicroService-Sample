using System.Text.Json.Serialization;

namespace MicroPack.Consul.Models
{
    public class Connect
    {
        [JsonPropertyName("sidecar_service")]
        public SidecarService SidecarService { get; set; }
    }

}