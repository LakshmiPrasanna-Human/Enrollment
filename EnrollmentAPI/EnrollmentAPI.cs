using System;
using System.Collections.Generic;
using System.Fabric;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.ServiceFabric.Services.Communication.AspNetCore;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using System.Fabric.Health;
using System.Fabric.Description;

namespace EnrollmentUI
{
    /// <summary>
    /// The FabricRuntime creates an instance of this class for each service type instance. 
    /// </summary>
    internal sealed class EnrollmentAPI : StatelessService
    {
        public EnrollmentAPI(StatelessServiceContext context)
            : base(context)
        {

            _healthTimer = new Timer(ReportHealthAndLoad, null, Timeout.Infinite, Timeout.Infinite);
            context.CodePackageActivationContext.ConfigurationPackageModifiedEvent += CodePackageActivationContext_ConfigurationPackageModifiedEvent;
        }

        /// <summary>
        /// Optional override to create listeners (like tcp, http) for this service instance.
        /// </summary>
        /// <returns>The collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            return new ServiceInstanceListener[]
            {
                new ServiceInstanceListener(serviceContext =>
                    new WebListenerCommunicationListener(serviceContext, "ServiceEndpoint", (url, listener) =>
                    {
                        ServiceEventSource.Current.ServiceMessage(serviceContext, $"Starting WebListener on {url}");

                        return new WebHostBuilder()
                                    .UseWebListener()
                                    .ConfigureServices(
                                        services => services
                                            .AddSingleton<StatelessServiceContext>(serviceContext))
                                    .UseContentRoot(Directory.GetCurrentDirectory())
                                    .UseStartup<Startup>()
                                    .UseServiceFabricIntegration(listener, ServiceFabricIntegrationOptions.None)
                                    .UseUrls(url)
                                    .Build();
                    }))
            };
        }

        private TimeSpan _interval = TimeSpan.FromSeconds(30);
        private long _lastCount = 0L;
        private DateTime _lastReport = DateTime.UtcNow;
        private Timer _healthTimer = null;
        private FabricClient _client = null;

        protected override Task OnOpenAsync(CancellationToken cancellationToken)
        {
            LoadConfiguration();

            _client = new FabricClient();
            _healthTimer = new Timer(ReportHealthAndLoad, null, _interval, _interval);
            return base.OnOpenAsync(cancellationToken);


        }

        public void ReportHealthAndLoad(object notused)
        {
          //  Calculate the values and then remember current values for the next report.

           long total = Controllers.StudentController._requestCount;
            long diff = total - _lastCount;
            long duration = Math.Max((long)DateTime.UtcNow.Subtract(_lastReport).TotalSeconds, 1L);
            long rps = diff / duration;
            _lastCount = total;
            _lastReport = DateTime.UtcNow;

            // Create the health information for this instance of the service and send report to Service Fabric.
            HealthInformation hi = new HealthInformation("EnrollmentAPIServiceHealth", "Heartbeat", HealthState.Ok)
            {
                TimeToLive = _interval.Add(_interval),
                Description = $"{diff} requests since last report. RPS: {rps} Total requests: {total}.",
                RemoveWhenExpired = false,
                SequenceNumber = HealthInformation.AutoSequenceNumber
            };
            var sshr = new StatelessServiceInstanceHealthReport(Context.PartitionId, Context.InstanceId, hi);
            _client.HealthManager.ReportHealth(sshr);

            //Report the load
            Partition.ReportLoad(new[] { new LoadMetric("RPS", (int)rps) });
            ServiceEventSource.Current.HealthReport(hi.SourceId, hi.Property, Enum.GetName(typeof(HealthState), hi.HealthState), Context.PartitionId, Context.ReplicaOrInstanceId, hi.Description);

          //  Report failing health report to cause rollback.
           //var nodeList = _client.QueryManager.GetNodeListAsync(Context.NodeContext.NodeName).GetAwaiter().GetResult();
           // var node = nodeList[0];
           // if ("4" == node.UpgradeDomain || "3" == node.UpgradeDomain || "2" == node.UpgradeDomain || "1" == node.UpgradeDomain)
           // {
           //     hi = new HealthInformation("EnrollmentAPIServiceHealth", "Error_Heartbeat", HealthState.Error);
           //     hi.TimeToLive = _interval.Add(_interval);
           //     hi.Description = $"Bogus health error to force rollback.";
           //     hi.RemoveWhenExpired = true;
           //     hi.SequenceNumber = HealthInformation.AutoSequenceNumber;
           //     sshr = new StatelessServiceInstanceHealthReport(Context.PartitionId, Context.InstanceId, hi);
           //     _client.HealthManager.ReportHealth(sshr);
           // }



        }

        private void CodePackageActivationContext_ConfigurationPackageModifiedEvent(object sender, PackageModifiedEventArgs<ConfigurationPackage> e)
        {
            ServiceEventSource.Current.Message("CodePackageActivationContext_ConfigurationPackageModifiedEvent");
            LoadConfiguration();
        }

        private void LoadConfiguration()
        {
            ServiceEventSource.Current.Message("LoadConfiguration");

            // Get the Health Check Interval configuration value.
            ConfigurationPackage pkg = Context.CodePackageActivationContext.GetConfigurationPackageObject("Config");
            if (null != pkg)
            {
                if (true == pkg.Settings?.Sections?.Contains("Health"))
                {
                    ConfigurationSection settings = pkg.Settings.Sections["Health"];
                    if (true == settings?.Parameters.Contains("HealthCheckIntervalSeconds"))
                    {
                        int value = 0;
                        ConfigurationProperty prop = settings.Parameters["HealthCheckIntervalSeconds"];
                        if (int.TryParse(prop?.Value, out value))
                        {
                            _interval = TimeSpan.FromSeconds(Math.Max(30, value));
                            _healthTimer.Change(_interval, _interval);
                        }

                        ServiceEventSource.Current.HealthReportIntervalChanged("EnrollmentAPIServiceHealth", "IntervalChanged", Context.PartitionId, Context.ReplicaOrInstanceId, (int)_interval.TotalSeconds);
                    }
                }
            }
        }

    }
}
