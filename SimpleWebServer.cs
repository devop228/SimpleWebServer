namespace zhusmelb.NetLib.Http
{
    using System;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Net;
    using System.Threading;
    using System.Threading.Tasks;

    public delegate void RequestReceivedHandler(NameObjectCollectionBase request);
    public delegate void SimpleWebServerErrorHandhler(Exception ex);
    public class SimpleWebServer
    {
        private SimpleWebServerOption _options;
        public event RequestReceivedHandler RequestReceived;
        public event SimpleWebServerErrorHandhler ServerExceptionThrown;
        public SimpleWebServer(SimpleWebServerOption options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            if (_options.RequestReceived != null)
                RequestReceived += _options.RequestReceived;
        }
        public Task StartAsync(CancellationToken cancellation) 
        {
            return Task.Run(async () => {
                if (_options.Prefixes==null || _options.Prefixes.Count==0)
                    throw new InvalidOperationException("No Prefix set");

                var listener = new HttpListener();
                foreach (var p in _options.Prefixes)
                    listener.Prefixes.Add(p);
                try
                {
                    listener.Start();
                    HttpListenerContext context = await listener.GetContextAsync();
                    cancellation.ThrowIfCancellationRequested();
                    HttpListenerRequest request = context.Request;
                    OnRequestReceived(request.QueryString);
                }
                catch (OperationCanceledException ex) {
                    if (!cancellation.IsCancellationRequested)
                        OnServerExceptionThrown(ex);
                    throw;  // cancellation token, not an error
                }
                catch (Exception ex)
                {
                    OnServerExceptionThrown(ex);
                    throw;
                }
                finally {
                    if (listener.IsListening) 
                        listener.Stop();
                    listener.Close();
                }
            }, cancellation);
        }
        protected virtual void OnRequestReceived(NameObjectCollectionBase request)
            => invokeEventHandler(RequestReceived, request);
        protected virtual void OnServerExceptionThrown(Exception ex) 
            => invokeEventHandler(ServerExceptionThrown, ex);
        private void invokeEventHandler(MulticastDelegate e, params object[] args) 
        {
            foreach (var d in e.GetInvocationList())
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