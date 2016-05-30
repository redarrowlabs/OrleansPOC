using Orleans.Runtime.Host;
using System;
using System.Net;

namespace Server
{
    public class OrleansService : IDisposable
    {
        private SiloHost _host;

        internal OrleansService()
        {
            _host = new SiloHost(Dns.GetHostName());
        }

        public bool Start()
        {
            _host.LoadOrleansConfig();
            _host.InitializeOrleansSilo();
            return _host.StartOrleansSilo();
        }

        public bool Stop()
        {
            _host.ShutdownOrleansSilo();
            return true;
        }

        public void Dispose()
        {
            _host.Dispose();
            _host = null;
        }
    }
}