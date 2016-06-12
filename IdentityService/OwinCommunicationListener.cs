using Microsoft.Owin.Hosting;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Owin;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;

namespace IdentityService
{
    internal class OwinCommunicationListener : ICommunicationListener
    {
        private readonly ServiceEventSource _eventSource;
        private readonly Action<IAppBuilder> _startup;
        private readonly ServiceContext _serviceContext;
        private readonly string _endpointName;
        private readonly string _appRoot;

        private IDisposable _webApp;
        private string _publishAddress;
        private string _listeningAddress;

        public OwinCommunicationListener(
            Action<IAppBuilder> startup,
            ServiceContext serviceContext,
            ServiceEventSource eventSource,
            string endpointName)
            : this(startup, serviceContext, eventSource, endpointName, null)
        {
        }

        public OwinCommunicationListener(Action<IAppBuilder> startup, ServiceContext serviceContext, ServiceEventSource eventSource, string endpointName, string appRoot)
        {
            if (startup == null)
            {
                throw new ArgumentNullException(nameof(startup));
            }

            if (serviceContext == null)
            {
                throw new ArgumentNullException(nameof(serviceContext));
            }

            if (endpointName == null)
            {
                throw new ArgumentNullException(nameof(endpointName));
            }

            if (eventSource == null)
            {
                throw new ArgumentNullException(nameof(eventSource));
            }

            _startup = startup;
            _serviceContext = serviceContext;
            _endpointName = endpointName;
            _eventSource = eventSource;
            _appRoot = appRoot;
        }

        public bool ListenOnSecondary { get; set; }

        public Task<string> OpenAsync(CancellationToken cancellationToken)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.WithProperty("Service", _serviceContext.ServiceName)
                .WriteTo.Trace(LogEventLevel.Information)
                .CreateLogger();

            var serviceEndpoint = _serviceContext.CodePackageActivationContext.GetEndpoint(_endpointName);
            int port = serviceEndpoint.Port;

            if (_serviceContext is StatefulServiceContext)
            {
                var statefulServiceContext = _serviceContext as StatefulServiceContext;

                _listeningAddress = String.Format(
                    CultureInfo.InvariantCulture,
                    "http://+:{0}/{1}{2}/{3}/{4}",
                    port,
                    String.IsNullOrWhiteSpace(_appRoot)
                        ? String.Empty
                        : _appRoot.TrimEnd('/') + '/',
                    statefulServiceContext.PartitionId,
                    statefulServiceContext.ReplicaId,
                    Guid.NewGuid()
                );
            }
            else if (_serviceContext is StatelessServiceContext)
            {
                _listeningAddress = String.Format(
                    CultureInfo.InvariantCulture,
                    "http://+:{0}/{1}",
                    port,
                    String.IsNullOrWhiteSpace(_appRoot)
                        ? String.Empty
                        : _appRoot.TrimEnd('/') + '/'
                );
            }
            else
            {
                throw new InvalidOperationException();
            }

            _publishAddress = _listeningAddress.Replace("+", FabricRuntime.GetNodeContext().IPAddressOrFQDN);
            try
            {
                _eventSource.ServiceMessage(_serviceContext, "Starting web server on " + _listeningAddress);
                _webApp = WebApp.Start(
                    _listeningAddress,
                    appBuilder =>
                    {
                        PopulateAppProperties(appBuilder.Properties);
                        _startup.Invoke(appBuilder);
                    }
                );

                _eventSource.ServiceMessage(_serviceContext, "Listening on " + _publishAddress);

                return Task.FromResult(_publishAddress);
            }
            catch (Exception ex)
            {
                _eventSource.ServiceMessage(_serviceContext, "Web server failed to open. " + ex.ToString());
                StopWebServer();

                throw;
            }
        }

        public Task CloseAsync(CancellationToken cancellationToken)
        {
            _eventSource.ServiceMessage(_serviceContext, "Closing web server");

            StopWebServer();

            return Task.FromResult(true);
        }

        public void Abort()
        {
            _eventSource.ServiceMessage(_serviceContext, "Aborting web server");

            StopWebServer();
        }

        private void StopWebServer()
        {
            if (_webApp != null)
            {
                try
                {
                    _webApp.Dispose();
                }
                catch (ObjectDisposedException) { }
            }
        }

        private void PopulateAppProperties(IDictionary<string, object> properties)
        {
            var config = _serviceContext.CodePackageActivationContext.GetConfigurationPackageObject("Config");

            properties.Add("AuthAuthority", config.Settings.Sections["Auth"].Parameters["Authority"].Value);
        }
    }
}