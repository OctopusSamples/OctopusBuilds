using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Octopus.Trident.Web.BusinessLogic.Converters;
using Octopus.Trident.Web.Core.Constants;
using Octopus.Trident.Web.Core.Models;
using Octopus.Trident.Web.Core.Models.OctopusServerModels;
using RestSharp;

namespace Octopus.Trident.Web.DataAccess
{
    public interface IOctopusRepository
    {
        Task<List<SpaceModel>> GetAllSpacesAsync(InstanceModel instanceModel);
        Task<List<ProjectModel>> GetAllProjectsForSpaceAsync(InstanceModel instanceModel, SpaceModel space);
        Task<List<EnvironmentModel>> GetAllEnvironmentsForSpaceAsync(InstanceModel instanceModel, SpaceModel space);
        Task<List<TenantModel>> GetAllTenantsForSpaceAsync(InstanceModel instanceModel, SpaceModel space);
        Task<List<ReleaseModel>> GetAllReleasesForProjectAsync(InstanceModel instanceModel, SpaceModel space, ProjectModel project);
        Task<List<DeploymentModel>> GetAllDeploymentsForReleaseAsync(InstanceModel instanceModel, SpaceModel space, ProjectModel project, ReleaseModel releaseModel, Dictionary<string, EnvironmentModel> environmentDictionary, Dictionary<string, TenantModel> tenantDictionary);
        Task<PagedOctopusModel<EventOctopusModel>> GetAllEvents(InstanceModel instanceModel, SyncModel syncModel, int startIndex);
        Task<ReleaseModel> GetSpecificRelease(InstanceModel instanceModel, SpaceModel space, ProjectModel project, string releaseId);
        Task<DeploymentModel> GetSpecificDeployment(InstanceModel instanceModel, SpaceModel space, ReleaseModel release, string deploymentId, Dictionary<string, EnvironmentModel> environmentDictionary, Dictionary<string, TenantModel> tenantDictionary);
    }

    public class OctopusRepository : IOctopusRepository
    {
        private readonly IOctopusModelToInsightModelConverter _modelConverter;

        public OctopusRepository(IOctopusModelToInsightModelConverter modelConverter)
        {
            _modelConverter = modelConverter;
        }

        public Task<List<SpaceModel>> GetAllSpacesAsync(InstanceModel instanceModel)
        {
            return GetAllItemsFromOctopus<SpaceModel, NameOnlyOctopusModel>(instanceModel, "api/spaces", x => _modelConverter.ConvertFromOctopusToSpaceModel(x));
        }

        public Task<List<ProjectModel>> GetAllProjectsForSpaceAsync(InstanceModel instanceModel, SpaceModel space)
        {
            return GetAllItemsFromOctopus<ProjectModel, NameOnlyOctopusModel>(instanceModel, $"api/{space.OctopusId}/projects", x => _modelConverter.ConvertFromOctopusToProjectModel(x, space.Id));
        }

        public Task<List<ReleaseModel>> GetAllReleasesForProjectAsync(InstanceModel instanceModel, SpaceModel space, ProjectModel project)
        {
            return GetAllItemsFromOctopus<ReleaseModel, ReleaseOctopusModel>(instanceModel, $"api/{space.OctopusId}/projects/{project.OctopusId}/releases", x => _modelConverter.ConvertFromOctopusToReleaseModel(x, project.Id));
        }

        public Task<List<EnvironmentModel>> GetAllEnvironmentsForSpaceAsync(InstanceModel instanceModel, SpaceModel space)
        {
            return GetAllItemsFromOctopus<EnvironmentModel, NameOnlyOctopusModel>(instanceModel, $"api/{space.OctopusId}/environments", x => _modelConverter.ConvertFromOctopusToEnvironmentModel(x, space.Id));
        }

        public Task<List<TenantModel>> GetAllTenantsForSpaceAsync(InstanceModel instanceModel, SpaceModel space)
        {
            return GetAllItemsFromOctopus<TenantModel, NameOnlyOctopusModel>(instanceModel, $"api/{space.OctopusId}/tenants", x => _modelConverter.ConvertFromOctopusToTenantModel(x, space.Id));
        }

        public async Task<List<DeploymentModel>> GetAllDeploymentsForReleaseAsync(InstanceModel instanceModel, SpaceModel space, ProjectModel project, ReleaseModel release, Dictionary<string, EnvironmentModel> environmentDictionary, Dictionary<string, TenantModel> tenantDictionary)
        {
            var startIndex = -10;
            var returnList = new List<DeploymentModel>();
            var continueQuery = true;
            var client = new RestClient(instanceModel.Url);

            while (continueQuery)
            {
                startIndex += 10;
                var request = new RestRequest($"api/{space.OctopusId}/releases/{release.OctopusId}/deployments");
                request.AddQueryParameter("skip", startIndex.ToString());
                request.AddQueryParameter("task", "10");
                request.AddHeader("X-Octopus-ApiKey", instanceModel.ApiKey);

                var response = await client.ExecuteGetAsync(request);
                var pagedModel = JsonConvert.DeserializeObject<PagedOctopusModel<DeploymentOctopusModel>>(response.Content);

                foreach (var item in pagedModel.Items)
                {
                    var deploymentRequest = new RestRequest(item.Links.Task);
                    deploymentRequest.AddHeader("X-Octopus-ApiKey", instanceModel.ApiKey);

                    var deploymentResponse = await client.ExecuteGetAsync(deploymentRequest);
                    var deploymentTaskModel = JsonConvert.DeserializeObject<DeploymentOctopusTaskModel>(deploymentResponse.Content);

                    returnList.Add(_modelConverter.ConvertFromOctopusToDeploymentModel(item, deploymentTaskModel, release.Id, environmentDictionary, tenantDictionary));
                }

                continueQuery = returnList.Count < pagedModel.TotalResults && pagedModel.Items.Count > 0;
            }

            return returnList;
        }

        public async Task<ReleaseModel> GetSpecificRelease(InstanceModel instanceModel, SpaceModel space, ProjectModel project, string releaseId)
        {
            var client = new RestClient(instanceModel.Url);
            var request = new RestRequest($"api/{space.OctopusId}/release/{releaseId}");
            request.AddHeader("X-Octopus-ApiKey", instanceModel.ApiKey);

            var response = await client.ExecuteGetAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var octopusRelease = JsonConvert.DeserializeObject<ReleaseOctopusModel>(response.Content);
                return _modelConverter.ConvertFromOctopusToReleaseModel(octopusRelease, project.Id);
            }

            return null;
        }

        public async Task<DeploymentModel> GetSpecificDeployment(InstanceModel instanceModel, SpaceModel space, ReleaseModel release, string deploymentId, Dictionary<string, EnvironmentModel> environmentDictionary, Dictionary<string, TenantModel> tenantDictionary)
        {
            var client = new RestClient(instanceModel.Url);
            var request = new RestRequest($"api/{space.OctopusId}/deployments/{deploymentId}");
            request.AddHeader("X-Octopus-ApiKey", instanceModel.ApiKey);

            var response = await client.ExecuteGetAsync(request);

            if (response.StatusCode == HttpStatusCode.OK)
            {
                var octopusModel = JsonConvert.DeserializeObject<DeploymentOctopusModel>(response.Content);

                var deploymentRequest = new RestRequest(octopusModel.Links.Task);
                deploymentRequest.AddHeader("X-Octopus-ApiKey", instanceModel.ApiKey);

                var deploymentResponse = await client.ExecuteGetAsync(deploymentRequest);
                var deploymentTaskModel = JsonConvert.DeserializeObject<DeploymentOctopusTaskModel>(deploymentResponse.Content);

                return _modelConverter.ConvertFromOctopusToDeploymentModel(octopusModel, deploymentTaskModel, release.Id, environmentDictionary, tenantDictionary);
            }

            return null;
        }

        public async Task<PagedOctopusModel<EventOctopusModel>> GetAllEvents(InstanceModel instanceModel, SyncModel syncModel, int startIndex)
        {
            var client = new RestClient(instanceModel.Url);
            var request = new RestRequest($"api/events");
            request.AddQueryParameter("from", syncModel.SearchStartDate.GetValueOrDefault().ToString("O"));
            request.AddQueryParameter("to", syncModel.Started.GetValueOrDefault().ToString("O"));
            request.AddQueryParameter("eventCategories", OctopusEventCategories.DeploymentStarted);
            request.AddQueryParameter("skip", startIndex.ToString());
            request.AddQueryParameter("take", "10");
            request.AddHeader("X-Octopus-ApiKey", instanceModel.ApiKey);

            var response = await client.ExecuteGetAsync(request);
            return JsonConvert.DeserializeObject<PagedOctopusModel<EventOctopusModel>>(response.Content);
        }

        private async Task<List<T>> GetAllItemsFromOctopus<T, OctoT>(
            InstanceModel instanceModel,
            string endPoint,
            Func<OctoT, T> converterFunction) where T : BaseModel where OctoT : BaseOctopusServerModel
        {
            var startIndex = -10;
            var returnList = new List<T>();
            var continueQuery = true;
            var client = new RestClient(instanceModel.Url);

            while (continueQuery)
            {
                startIndex += 10;
                var request = new RestRequest($"{endPoint}");
                request.AddQueryParameter("skip", startIndex.ToString());
                request.AddQueryParameter("task", "10");
                request.AddHeader("X-Octopus-ApiKey", instanceModel.ApiKey);

                var response = await client.ExecuteGetAsync(request);
                var pagedModel = JsonConvert.DeserializeObject<PagedOctopusModel<OctoT>>(response.Content);

                returnList.AddRange(pagedModel.Items.Select(converterFunction));

                continueQuery = returnList.Count < pagedModel.TotalResults && pagedModel.Items.Count > 0;
            }

            return returnList;
        }
    }
}
