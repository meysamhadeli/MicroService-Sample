using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using App.Metrics;
using App.Metrics.Gauge;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Pacco.Services.Availability.Infrastructure.Metrics
{
    public class MetricsJob : BackgroundService
    {
        private readonly IMetricsRoot _metricsRoot;
        private readonly ILogger<MetricsJob> _logger;

        private readonly GaugeOptions _threads = new GaugeOptions()
        {
            Name = "threads"
        };
        private readonly GaugeOptions _workingSet = new GaugeOptions()
        {
            Name = "working_set"
        };

        public MetricsJob(IMetricsRoot metricsRoot, ILogger<MetricsJob> logger)
        {
            _metricsRoot = metricsRoot;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("processing the matrics...");
                var process = Process.GetCurrentProcess();
                _metricsRoot.Measure.Gauge.SetValue(_threads,process.Threads.Count);
                _metricsRoot.Measure.Gauge.SetValue(_workingSet,process.WorkingSet64);
                await Task.Delay(5000, stoppingToken);
            }
        }
    }
}