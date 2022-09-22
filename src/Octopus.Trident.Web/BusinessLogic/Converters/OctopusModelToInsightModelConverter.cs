using System.Collections.Generic;
using Octopus.Trident.Web.Core.Models;
using Octopus.Trident.Web.Core.Models.OctopusServerModels;

namespace Octopus.Trident.Web.BusinessLogic.Converters
{
    public interface IOctopusModelToInsightModelConverter
    {
        SpaceModel ConvertFromOctopusToSpaceModel(NameOnlyOctopusModel nameOnlyOctopusModel);
        ProjectModel ConvertFromOctopusToProjectModel(NameOnlyOctopusModel projectOctopusModel, int spaceId);
        EnvironmentModel ConvertFromOctopusToEnvironmentModel(NameOnlyOctopusModel projectOctopusModel, int spaceId);
        TenantModel ConvertFromOctopusToTenantModel(NameOnlyOctopusModel tenantOctopusModel, int spaceId);
        ReleaseModel ConvertFromOctopusToReleaseModel(ReleaseOctopusModel releaseOctopusModel, int projectId);
        DeploymentModel ConvertFromOctopusToDeploymentModel(
            DeploymentOctopusModel deploymentOctopusModel, 
            DeploymentOctopusTaskModel deploymentOctopusTaskModel,
            int releaseId, 
            Dictionary<string, EnvironmentModel> environmentDictionary, 
            Dictionary<string, TenantModel> tenantDictionary);
    }

    public class OctopusModelToInsightModelConverter : IOctopusModelToInsightModelConverter
    {
        public SpaceModel ConvertFromOctopusToSpaceModel(NameOnlyOctopusModel nameOnlyOctopusModel)
        {
            return new SpaceModel
            {
                OctopusId = nameOnlyOctopusModel.Id,
                Name = nameOnlyOctopusModel.Name
            };
        }

        public ProjectModel ConvertFromOctopusToProjectModel(NameOnlyOctopusModel projectOctopusModel, int spaceId)
        {
            return new ProjectModel
            {
                Name = projectOctopusModel.Name,
                OctopusId = projectOctopusModel.Id,
                SpaceId = spaceId
            };
        }

        public EnvironmentModel ConvertFromOctopusToEnvironmentModel(NameOnlyOctopusModel projectOctopusModel, int spaceId)
        {
            return new EnvironmentModel
            {
                Name = projectOctopusModel.Name,
                OctopusId = projectOctopusModel.Id,
                SpaceId = spaceId
            };
        }

        public TenantModel ConvertFromOctopusToTenantModel(NameOnlyOctopusModel tenantOctopusModel, int spaceId)
        {
            return new TenantModel
            {
                Name = tenantOctopusModel.Name,
                OctopusId = tenantOctopusModel.Id,
                SpaceId = spaceId
            };
        }

        public ReleaseModel ConvertFromOctopusToReleaseModel(ReleaseOctopusModel releaseOctopusModel, int projectId)
        {
            return new ReleaseModel
            {
                Version = releaseOctopusModel.Version,
                Created = releaseOctopusModel.Assembled,
                OctopusId = releaseOctopusModel.Id,
                ProjectId = projectId
            };
        }

        public DeploymentModel ConvertFromOctopusToDeploymentModel(
            DeploymentOctopusModel deploymentOctopusModel, 
            DeploymentOctopusTaskModel deploymentOctopusTaskModel,
            int releaseId, 
            Dictionary<string, EnvironmentModel> environmentDictionary, 
            Dictionary<string, TenantModel> tenantDictionary)
        {
            return new DeploymentModel
            {
                OctopusId = deploymentOctopusModel.Id,
                ReleaseId = releaseId,
                Name = deploymentOctopusModel.Name,
                EnvironmentId = environmentDictionary.ContainsKey(deploymentOctopusModel.EnvironmentId) ? environmentDictionary[deploymentOctopusModel.EnvironmentId].Id : 0,
                TenantId = string.IsNullOrWhiteSpace(deploymentOctopusModel.TenantId) ? null : tenantDictionary.ContainsKey(deploymentOctopusModel.TenantId) ? (int?)tenantDictionary[deploymentOctopusModel.TenantId].Id : null,
                QueueTime = deploymentOctopusTaskModel.QueueTime,
                StartTime = deploymentOctopusTaskModel.StartTime,
                CompletedTime = deploymentOctopusTaskModel.CompletedTime,
                DeploymentState = deploymentOctopusTaskModel.State
            };
        }
    }
}
