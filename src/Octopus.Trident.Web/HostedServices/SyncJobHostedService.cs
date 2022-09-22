using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Octopus.Trident.Web.BusinessLogic.Facades;
using Octopus.Trident.Web.BusinessLogic.Factories;
using Octopus.Trident.Web.DataAccess;

namespace Octopus.Trident.Web.HostedServices
{
    public class SyncJobHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<SyncJobHostedService> _logger;
        private readonly ISyncRepository _syncRepository;
        private readonly ISyncJobCompositeModelFactory _syncJobCompositeModelFactory;
        private readonly ISyncJobFacade _syncJobFacade;
        private Timer _timer;
        private Task _executingTask;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();

        public SyncJobHostedService(ILogger<SyncJobHostedService> logger,
            ISyncRepository syncRepository,
            ISyncJobCompositeModelFactory syncJobCompositeModelFactory,
            ISyncJobFacade syncJobFacade)
        {
            _logger = logger;
            _syncRepository = syncRepository;
            _syncJobCompositeModelFactory = syncJobCompositeModelFactory;
            _syncJobFacade = syncJobFacade;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is starting.");

            _timer = new Timer(ExecuteTask, null, TimeSpan.FromSeconds(30), TimeSpan.FromMilliseconds(-1));

            return Task.CompletedTask;
        }

        private void ExecuteTask(object state)
        {
            _timer?.Change(Timeout.Infinite, 0);
            _executingTask = ExecuteTaskAsync(_stoppingCts.Token);
        }

        private async Task ExecuteTaskAsync(CancellationToken stoppingToken)
        {
            await RunJobAsync(stoppingToken);
            _timer.Change(TimeSpan.FromSeconds(30), TimeSpan.FromMilliseconds(-1));
        }

        private async Task RunJobAsync(CancellationToken stoppingToken)
        {
            var recordsToProcess = await _syncRepository.GetNextRecordsToProcessAsync();

            _logger.LogInformation($"Found {recordsToProcess.Items.Count} record(s) to process.");

            foreach (var syncJob in recordsToProcess.Items)
            {
                var syncJobCompositeModel = await _syncJobCompositeModelFactory.MakeSyncJobCompositeModelAsync(syncJob);

                await _syncJobFacade.ProcessSyncJob(syncJobCompositeModel);
            }
        }

        public virtual async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Timed Background Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);

            if (_executingTask == null)
            {
                return;
            }

            try
            {
                _stoppingCts.Cancel();
            }
            finally
            {
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite, cancellationToken));
            }

        }

        public void Dispose()
        {
            _stoppingCts.Cancel();
            _timer?.Dispose();
        }
    }
}
