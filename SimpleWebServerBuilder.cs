namespace zhusmelb.NetLib.Http
{
    using System;

    public class SimpleWebServerBuilder 
    {
        private Action<SimpleWebServerOption> _optionsConfig;

        public SimpleWebServerBuilder ConfigWebServer(Action<SimpleWebServerOption> config)
        {
            _optionsConfig += config;
            return this;
        }
        public SimpleWebServer Build()
        {
            var options = new SimpleWebServerOption();
            _optionsConfig(options);
            return new SimpleWebServer(options);
        }
    }
}