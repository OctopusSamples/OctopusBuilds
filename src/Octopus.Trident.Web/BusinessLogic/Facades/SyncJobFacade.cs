using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Octopus.Trident.Web.BusinessLogic.Factories;
using Octopus.Trident.Web.Core.Constants;
using Octopus.Trident.Web.Core.Models;
using Octopus.Trident.Web.Core.Models.CompositeModels;
using Octopus.Trident.Web.DataAccess;

namespace Octopus.Trident.Web.BusinessLogic.Facades
{
    public interface ISyncJobFacade
    {
        Task ProcessSyncJob(SyncJobCompositeModel syncJobCompositeModel);
    }

    public class SyncJobFacade : ISyncJobFacade
    {
        private readonly ILogger<SyncJobFacade> _logger;
        private readonly ISyncLogRepository _syncLogRepository;
        private readonly IOctopusRepository _octopusRepository;
        private readonly ISpaceRepository _spaceRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IEnvironmentRepository _environmentRepository;
        private readonly ITenantRepository _tenantRepository;
        private readonly IReleaseRepository _releaseRepository;
        private readonly IDeploymentRepository _deploymentRepository;
        private readonly ISyncRepository _syncRepository;
        private readonly ISyncLogModelFactory _syncLogModelFactory;

        public SyncJobFacade(
            ILogger<SyncJobFacade> logger,
            ISyncLogRepository syncLogRepository,
            IOctopusRepository octopusRepository,
            ISpaceRepository spaceRepository,
            IProjectRepository projectRepository,
            IEnvironmentRepository environmentRepository,
            ITenantRepository tenantRepository,
            IReleaseRepository releaseRepository,
            IDeploymentRepository deploymentRepository,
            ISyncRepository syncRepository,
            ISyncLogModelFactory syncLogModelFactory)
        {
            _logger = logger;
            _syncLogRepository = syncLogRepository;
            _octopusRepository = octopusRepository;
            _spaceRepository = spaceRepository;
            _projectRepository = projectRepository;
            _environmentRepository = environmentRepository;
            _tenantRepository = tenantRepository;
            _releaseRepository = releaseRepository;
            _deploymentRepository = deploymentRepository;
            _syncRepository = syncRepository;
            _syncLogModelFactory = syncLogModelFactory;
        }

        public async Task ProcessSyncJob(SyncJobCompositeModel syncJobCompositeModel)
        {
            try
            {
                await LogInformation("Setting the starting date to UTC Now", syncJobCompositeModel);
                syncJobCompositeModel.SyncModel.Started = DateTime.UtcNow;
                await _syncRepository.UpdateAsync(syncJobCompositeModel.SyncModel);

                var isSuccessful = await SyncAllRecords(syncJobCompositeModel);

                await ProcessStatus(syncJobCompositeModel, isSuccessful);
            }
            catch (Exception ex)
            {
                await LogException($"Exception when processing Sync Job {syncJobCompositeModel.SyncModel.Id} {ex.Message}", syncJobCompositeModel);
            }
        }

        private async Task ProcessStatus(SyncJobCompositeModel syncJobCompositeModel, bool isSuccessful)
        {
            if (isSuccessful)
            {
                await LogInformation("The sync was successful, setting the completed date to UTC Now and the state to completed", syncJobCompositeModel);
                syncJobCompositeModel.SyncModel.Completed = DateTime.UtcNow;
                syncJobCompositeModel.SyncModel.State = SyncState.Completed;
            }
            else
            {
                await LogInformation("The sync failed, incrementing retry counter", syncJobCompositeModel);
                syncJobCompositeModel.SyncModel.RetryAttempts = syncJobCompositeModel.SyncModel.RetryAttempts.GetValueOrDefault() + 1;

                if (syncJobCompositeModel.SyncModel.RetryAttempts > 5)
                {
                    await LogInformation("The retry counter has gone over 5, failing this sync", syncJobCompositeModel);
                    syncJobCompositeModel.SyncModel.State = SyncState.Failed;
                }
                else
                {
                    await LogInformation("Retry attempts are still possible, setting the start date to null", syncJobCompositeModel);
                    syncJobCompositeModel.SyncModel.Started = null;
                }
            }

            await LogInformation("Updating the sync job", syncJobCompositeModel);
            await _syncRepository.UpdateAsync(syncJobCompositeModel.SyncModel);
        }

        private async Task<bool> SyncAllRecords(SyncJobCompositeModel syncJobCompositeModel)
        {
            try
            {
                await LogInformation($"Getting all the spaces for {syncJobCompositeModel.InstanceModel.Name}", syncJobCompositeModel);
                var octopusSpaces = await _octopusRepository.GetAllSpacesAsync(syncJobCompositeModel.InstanceModel);
                await LogInformation($"{octopusSpaces.Count} space(s) found", syncJobCompositeModel);

                foreach (var item in octopusSpaces)
                {
                    await LogInformation($"Checking to see if space {item.OctopusId}:{item.Name} already exists", syncJobCompositeModel);
                    var spaceModel = await _spaceRepository.GetByOctopusIdAsync(item.OctopusId, syncJobCompositeModel.InstanceModel.Id);
                    await LogInformation($"{(spaceModel != null ? "Space already exists, updating" : "Unable to find space, creating")}", syncJobCompositeModel);
                    item.Id = spaceModel?.Id ?? 0;
                    item.InstanceId = syncJobCompositeModel.InstanceModel.Id;

                    await LogInformation($"Saving space {item.OctopusId}:{item.Name} to the database", syncJobCompositeModel);
                    var spaceToTrack = item.Id > 0 ? await _spaceRepository.UpdateAsync(item) : await _spaceRepository.InsertAsync(item);

                    syncJobCompositeModel.SpaceDictionary.Add(item.OctopusId, spaceToTrack);

                    await ProcessEnvironments(syncJobCompositeModel, spaceToTrack);
                    await ProcessTenants(syncJobCompositeModel, spaceToTrack);
                    await ProcessProjects(syncJobCompositeModel, spaceToTrack);
                }

                if (syncJobCompositeModel.SyncModel.SearchStartDate.HasValue)
                {
                    await ProcessDeploymentsSinceLastSync(syncJobCompositeModel);
                }

                return true;
            }
            catch (Exception ex)
            {
                await LogException($"Exception when processing Sync Job {syncJobCompositeModel.SyncModel.Id} {ex.Message}", syncJobCompositeModel);

                return false;
            }
        }

        private async Task ProcessProjects(SyncJobCompositeModel syncJobCompositeModel, SpaceModel space)
        {
            await LogInformation($"Getting all the projects for {syncJobCompositeModel.InstanceModel.Name}:{space.Name}", syncJobCompositeModel);
            var octopusList = await _octopusRepository.GetAllProjectsForSpaceAsync(syncJobCompositeModel.InstanceModel, space);
            await LogInformation($"{octopusList.Count} projects(s) found in {syncJobCompositeModel.InstanceModel.Name}:{space.Name}", syncJobCompositeModel);

            foreach (var item in octopusList)
            {
                await LogInformation($"Checking to see if project {item.OctopusId}:{item.Name} already exists", syncJobCompositeModel);
                var itemModel = await _projectRepository.GetByOctopusIdAsync(item.OctopusId, space.Id);
                await LogInformation($"{(itemModel != null ? "Project already exists, updating" : "Unable to find project, creating")}", syncJobCompositeModel);
                item.Id = itemModel?.Id ?? 0;

                await LogInformation($"Saving project {item.OctopusId}:{item.Name} to the database", syncJobCompositeModel);
                var modelToTrack = item.Id > 0 ? await _projectRepository.UpdateAsync(item) : await _projectRepository.InsertAsync(item);

                syncJobCompositeModel.ProjectDictionary.Add(item.OctopusId, modelToTrack);

                if (syncJobCompositeModel.SyncModel.SearchStartDate.HasValue == false)
                {
                    await ProcessReleasesForProject(syncJobCompositeModel, space, item);
                }
            }
        }

        private async Task ProcessEnvironments(SyncJobCompositeModel syncJobCompositeModel, SpaceModel space)
        {
            await LogInformation($"Getting all the environments for {syncJobCompositeModel.InstanceModel.Name}:{space.Name}", syncJobCompositeModel);
            var octopusList = await _octopusRepository.GetAllEnvironmentsForSpaceAsync(syncJobCompositeModel.InstanceModel, space);
            await LogInformation($"{octopusList.Count} environments(s) found in {syncJobCompositeModel.InstanceModel.Name}:{space.Name}", syncJobCompositeModel);

            foreach (var item in octopusList)
            {
                await LogInformation($"Checking to see if environment {item.OctopusId}:{item.Name} already exists", syncJobCompositeModel);
                var itemModel = await _environmentRepository.GetByOctopusIdAsync(item.OctopusId, space.Id);
                await LogInformation($"{(itemModel != null ? "Environment already exists, updating" : "Unable to find environment, creating")}", syncJobCompositeModel);
                item.Id = itemModel?.Id ?? 0;

                await LogInformation($"Saving environment {item.OctopusId}:{item.Name} to the database", syncJobCompositeModel);
                var modelToTrack = item.Id > 0 ? await _environmentRepository.UpdateAsync(item) : await _environmentRepository.InsertAsync(item);

                await LogInformation($"Adding environment {item.OctopusId}:{item.Name} to our sync dictionary for faster lookup", syncJobCompositeModel);
                syncJobCompositeModel.EnvironmentDictionary.Add(item.OctopusId, modelToTrack);
            }
        }

        private async Task ProcessTenants(SyncJobCompositeModel syncJobCompositeModel, SpaceModel space)
        {
            await LogInformation($"Getting all the tenants for {syncJobCompositeModel.InstanceModel.Name}:{space.Name}", syncJobCompositeModel);
            var octopusList = await _octopusRepository.GetAllTenantsForSpaceAsync(syncJobCompositeModel.InstanceModel, space);
            await LogInformation($"{octopusList.Count} tenants(s) found in {syncJobCompositeModel.InstanceModel.Name}:{space.Name}", syncJobCompositeModel);

            foreach (var item in octopusList)
            {
                await LogInformation($"Checking to see if tenant {item.OctopusId}:{item.Name} already exists", syncJobCompositeModel);
                var itemModel = await _tenantRepository.GetByOctopusIdAsync(item.OctopusId, space.Id);
                await LogInformation($"{(itemModel != null ? "Tenant already exists, updating" : "Unable to find tenant, creating")}", syncJobCompositeModel);
                item.Id = itemModel?.Id ?? 0;

                await LogInformation($"Saving tenant {item.OctopusId}:{item.Name} to the database", syncJobCompositeModel);
                var modelToTrack = item.Id > 0 ? await _tenantRepository.UpdateAsync(item) : await _tenantRepository.InsertAsync(item);

                await LogInformation($"Adding tenant {item.OctopusId}:{item.Name} to our sync dictionary for faster lookup", syncJobCompositeModel);
                syncJobCompositeModel.TenantDictionary.Add(item.OctopusId, modelToTrack);
            }
        }

        private async Task ProcessReleasesForProject(SyncJobCompositeModel syncJobCompositeModel, SpaceModel space, ProjectModel project)
        {
            await LogInformation($"Getting all the releases for {syncJobCompositeModel.InstanceModel.Name}:{space.Name}:{project.Name}", syncJobCompositeModel);
            var octopusList = await _octopusRepository.GetAllReleasesForProjectAsync(syncJobCompositeModel.InstanceModel, space, project);
            await LogInformation($"{octopusList.Count} releases(s) found in {syncJobCompositeModel.InstanceModel.Name}:{space.Name}:{project.Name}", syncJobCompositeModel);

            foreach (var item in octopusList)
            {
                await LogInformation($"Checking to see if release {syncJobCompositeModel.InstanceModel.Name}:{space.Name}:{project.Name}:{item.OctopusId}:{item.Version} already exists", syncJobCompositeModel);
                var itemModel = await _releaseRepository.GetByOctopusIdAsync(item.OctopusId, project.Id);
                await LogInformation($"{(itemModel != null ? "Release already exists, updating" : "Unable to find release, creating")}", syncJobCompositeModel);
                item.Id = itemModel?.Id ?? 0;

                await LogInformation($"Saving release {syncJobCompositeModel.InstanceModel.Name}:{space.Name}:{project.Name}:{item.OctopusId}:{item.Version} to the database", syncJobCompositeModel);
                var modelToTrack = item.Id > 0 ? await _releaseRepository.UpdateAsync(item) : await _releaseRepository.InsertAsync(item);

                await ProcessDeploymentsForProjectsRelease(syncJobCompositeModel, space, project, modelToTrack);
            }
        }

        private async Task ProcessDeploymentsForProjectsRelease(SyncJobCompositeModel syncJobCompositeModel, SpaceModel space, ProjectModel project, ReleaseModel releaseModel)
        {
            await LogInformation($"Getting all the deployments for {syncJobCompositeModel.InstanceModel.Name}:{space.Name}:{project.Name}:{releaseModel.Version}", syncJobCompositeModel);
            var octopusList = await _octopusRepository.GetAllDeploymentsForReleaseAsync(syncJobCompositeModel.InstanceModel, space, project, releaseModel, syncJobCompositeModel.EnvironmentDictionary, syncJobCompositeModel.TenantDictionary);
            await LogInformation($"{octopusList.Count} deployments(s) found in {syncJobCompositeModel.InstanceModel.Name}:{space.Name}:{project.Name}:{releaseModel.Version}", syncJobCompositeModel);

            foreach (var item in octopusList)
            {
                await LogInformation($"Checking to see if deployment {item.OctopusId}:{item.Name} already exists", syncJobCompositeModel);
                var itemModel = await _deploymentRepository.GetByOctopusIdAsync(item.OctopusId, releaseModel.Id);
                await LogInformation($"{(itemModel != null ? "Deployment already exists, updating" : "Unable to find deployment, creating")}", syncJobCompositeModel);
                item.Id = itemModel?.Id ?? 0;

                await LogInformation($"Saving deployment {item.OctopusId}:{item.Name} to the database", syncJobCompositeModel);
                var modelToTrack = item.Id > 0 ? await _deploymentRepository.UpdateAsync(item) : await _deploymentRepository.InsertAsync(item);
            }
        }

        private async Task ProcessDeploymentsSinceLastSync(SyncJobCompositeModel syncJobCompositeModel)
        {
            var startIndex = 0;
            var canContinue = true;

            await LogInformation($"Finding all deployments since {syncJobCompositeModel.SyncModel.SearchStartDate}", syncJobCompositeModel);

            while (canContinue)
            {
                await LogInformation($"Getting the next results at {startIndex}", syncJobCompositeModel);
                var eventResults = await _octopusRepository.GetAllEvents(syncJobCompositeModel.InstanceModel, syncJobCompositeModel.SyncModel, startIndex);

                foreach (var octopusEvent in eventResults.Items)
                {
                    var spaceId = octopusEvent.SpaceId;
                    var space = syncJobCompositeModel.SpaceDictionary[spaceId];

                    var projectId = octopusEvent.RelatedDocumentIds.First(x => x.StartsWith("Projects"));
                    var project = syncJobCompositeModel.ProjectDictionary[projectId];

                    var releaseId = octopusEvent.RelatedDocumentIds.First(x => x.StartsWith("Release"));
                    var releaseModel = await _octopusRepository.GetSpecificRelease(syncJobCompositeModel.InstanceModel, space, project, releaseId);

                    if (releaseModel != null)
                    {
                        var existingReleaseModel = await _releaseRepository.GetByOctopusIdAsync(releaseModel.OctopusId, project.Id);
                        await LogInformation($"{(existingReleaseModel != null ? "Release already exists, updating" : "Unable to find release, creating")}", syncJobCompositeModel);
                        releaseModel.Id = existingReleaseModel?.Id ?? 0;

                        await LogInformation($"Saving release {syncJobCompositeModel.InstanceModel.Name}:{space.Name}:{project.Name}:{releaseModel.OctopusId}:{releaseModel.Version} to the database", syncJobCompositeModel);
                        var releaseModelToTrack = releaseModel.Id > 0 ? await _releaseRepository.UpdateAsync(releaseModel) : await _releaseRepository.InsertAsync(releaseModel);

                        var deploymentId = octopusEvent.RelatedDocumentIds.First(x => x.StartsWith("DeploymentId"));
                        var deploymentModel = await _octopusRepository.GetSpecificDeployment(syncJobCompositeModel.InstanceModel, space, releaseModelToTrack, deploymentId, syncJobCompositeModel.EnvironmentDictionary, syncJobCompositeModel.TenantDictionary);

                        if (deploymentModel != null)
                        {
                            var itemModel = await _deploymentRepository.GetByOctopusIdAsync(deploymentModel.OctopusId, releaseModel.Id);
                            await LogInformation($"{(itemModel != null ? "Deployment already exists, updating" : "Unable to find deployment, creating")}", syncJobCompositeModel);
                            deploymentModel.Id = itemModel?.Id ?? 0;

                            await LogInformation($"Saving deployment {deploymentModel.OctopusId}:{deploymentModel.Name} to the database", syncJobCompositeModel);
                            var modelToTrack = deploymentModel.Id > 0 ? await _deploymentRepository.UpdateAsync(deploymentModel) : await _deploymentRepository.InsertAsync(deploymentModel);
                        }
                    }
                }

                canContinue = eventResults.Items.Count > 0;
                startIndex += 10;
            }
            
        }

        private string GetMessagePrefix(SyncJobCompositeModel syncJobCompositeModel)
        {
            return $"Sync {syncJobCompositeModel.SyncModel.Id}: ";
        }

        private async Task LogInformation(string message, SyncJobCompositeModel syncJobCompositeModel)
        {
            var formattedMessage = $"{GetMessagePrefix(syncJobCompositeModel)}{message}";
            _logger.LogInformation(formattedMessage);
            await _syncLogRepository.InsertAsync(_syncLogModelFactory.MakeInformationLog(formattedMessage, syncJobCompositeModel.SyncModel.Id));
        }

        private async Task LogException(string message, SyncJobCompositeModel syncJobCompositeModel)
        {
            var formattedMessage = $"{GetMessagePrefix(syncJobCompositeModel)}{message}";
            _logger.LogError(formattedMessage);
            await _syncLogRepository.InsertAsync(_syncLogModelFactory.MakeErrorLog(formattedMessage, syncJobCompositeModel.SyncModel.Id));
        }
    }
}
