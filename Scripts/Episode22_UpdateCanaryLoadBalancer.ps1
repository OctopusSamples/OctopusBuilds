$EnvironmentName = $OctopusParameters["Octopus.Environment.Name"]
$PercentOfTraffic = $OctopusParameters["Project.Loadbalancer.Percent"]
$DelayInMinutes = $OctopusParameters["Project.Loadbalancer.WaitInMinutes"]
$HostToUpdate = $OctopusParameters["Project.Loadbalancer.HostToUpdate"]
$StepName = $OctopusParameters["Octopus.Step.Name"]

$delayInMinutesAsNumber = [int]$DelayInMinutes
Write-Host "Environment Name: $EnvironmentName"
Write-Host "Percent of Traffic: $PercentOfTraffic"
Write-Host "Delay in Minutes: $delayInMinutesAsNumber"
Write-Host "Step Name: $StepName"
Write-Host "Tenant to chnage: $HostToUpdate"

Write-Highlight "Updating the load balancer to point $PercentOfTraffic% of traffic for $HostToUpdate to $EnvironmentName servers"

Write-Host "Verifying the new version of code isn't throwing errors"
$randomNumber = Get-Random -Minimum 1 -Maximum 100

Write-Host "The random number is $randomNumber.  If it is above 95, this will fail the step."
if ($randomNumber -ge 95)
{
	Write-Host "An error occurred verifying the application."
    exit 1
}

$percentOfTrafficAsNumber = [int]$PercentOfTraffic
$newPercentOfTraffic = $percentOfTrafficAsNumber * 2

$continue = $true
if ($percentOfTrafficAsNumber -ge 100)
{
	Write-Highlight "We have converted over all traffic for $HostToUpdate to $EnvironmentName"
    $continue = $false
}
elseif ($newPercentOfTraffic -gt 100)
{
	$newPercentOfTraffic = 100
	Write-Highlight "Triggering a runbook to update the load balancer to $newPercentOfTraffic% for $HostToUpdate to the $EnvironmentName in $DelayInMinutes minutes."    
}
else
{
	Write-Highlight "Triggering a runbook to update the load balancer to $newPercentOfTraffic% for $HostToUpdate to the $EnvironmentName in $DelayInMinutes minutes."
}

Set-OctopusVariable -name "ContinueWithCanary" -value $continue
Write-Host "Variable run condition: ##{unless Octopus.Deployment.Error}#{Octopus.Action[$stepName].Output.ContinueWithCanary}##{/unless}"
Set-OctopusVariable -name "PercentOfTraffic" -value $newPercentOfTraffic
Write-Host "Variable run condition: ##{unless Octopus.Deployment.Error}#{Octopus.Action[$stepName].Output.PercentOfTraffic}##{/unless}"

$newRunTime = (Get-Date).AddMinutes($delayInMinutesAsNumber)
$newRunTime = $newRunTime.ToString("yyyy-MM-dd HH:mm:ss")
Set-OctopusVariable -name "RunbookRunTime" -value $newRunTime
Write-Host "Variable run condition: ##{unless Octopus.Deployment.Error}#{Octopus.Action[$stepName].Output.RunbookRunTime}##{/unless}"