namespace zhusmelb.NetLib.Http
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    public delegate void RequestReceivedHandler(NameObjectCollectionBase request);
    public class SimpleWebServer
    {
        private SimpleWebServerOption _options;
        public event RequestReceivedHandler RequestReceived;

        public SimpleWebServer(SimpleWebServerOption options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            if (_options.RequestReceived != null)
                RequestReceived += _options.RequestReceived;
        }
        public Task StartAsync(CancellationToken cancellation) 
        {
            return Task.Run(async () => {
                var listener = new HttpListener();
                if (_options.Prefixes==null || _options.Prefixes.Count==0)
                    throw new InvalidOperationException("No Prefix set");
                    
                foreach (var p in _options.Prefixes)
                    listener.Prefixes.Add(p);
                try
                {
                    listener.Start();
                    HttpListenerContext context = await listener.GetContextAsync();
                    HttpListenerRequest request = context.Request;

                    OnRequestReceived(request.QueryString);

                    listener.Stop();
                    listener.Close();
                }
                catch (HttpListenerException lnEx)
                {
                    var code = lnEx.ErrorCode;
                }
            }, cancellation);
        }
        protected virtual void OnRequestReceived(NameObjectCollectionBase request)
        {
            var args = new[] { (object)request };
            foreach (var d in RequestReceived.GetInvocationList())
            {
                var syncer = d.Target as ISynchronizeInvoke;
                if (syncer != null && syncer.InvokeRequired)
                    syncer.BeginInvoke(d, args);
                else
                    d.DynamicInvoke(args);
            }
        }

        
    }
}