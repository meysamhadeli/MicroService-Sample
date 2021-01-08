using System.Collections.Generic;

namespace MicroPack.Consul.Models
{
    public class Proxy
    {
        public List<Upstream> Upstreams { get; set; }
    }
}