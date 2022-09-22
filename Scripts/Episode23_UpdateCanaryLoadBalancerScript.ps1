$EnvironmentName = $OctopusParameters["Octopus.Environment.Name"]
$TenantName = $OctopusParameters["Octopus.Deployment.Tenant.Name"]

Write-Host "Environment Name: $EnvironmentName"
Write-Host "Tenant to change: $TenantName"

Write-Host "Verifying the new version of code isn't throwing errors"
$randomNumber = Get-Random -Minimum 1 -Maximum 100

Write-Verbose "The random number is $randomNumber.  If it is above 95, this will fail the step."
if ($randomNumber -ge 95)
{
	Write-Verbose "An error occurred verifying the application."
    exit 1
}
else
{
	Write-Highlight "The application passed verification, can swap all traffic over to it."
}

Write-Highlight "Updating the load balancer to point $TenantName to the latest version of code for $EnvironmentName servers"