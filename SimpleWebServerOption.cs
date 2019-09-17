using System.Collections.Generic;

namespace zhusmelb.NetLib.Http
{
    public class SimpleWebServerOption
    {
        public ICollection<string> Prefixes { get; } = new List<string>();
        public RequestReceivedHandler RequestReceived;
    }
}