$CommandsRequireApproval = $OctopusParameters["Databases.AutoApprove.CommandsRequireApproval"]
$CommandsIndicatingChange = $OctopusParameters["Databases.AutoApprove.CommandsIndicatingChange"]
$ReportPath = $OctopusParameters["Databases.AutoApproval.ReportPath"]
$stepName = $OctopusParameters["Octopus.Action.StepName"]

Write-Host "Commands requiring approval: $CommandsRequireApproval"
Write-Host "Octopus indicating changes: $CommandsIndicatingChange"
Write-Host "Report Path: $ReportPath"

$ApprovalRequired = $false
$HasDatabaseChanges = $false

$fileContent = Get-Content -path $ReportPath

$commandListToCheck = $CommandsRequireApproval -split ","
$commandListForChanges = $CommandsIndicatingChange -split ","

    Write-Host "Looping through all commands requiring approval for $ReportPath"
	foreach ($command in $commandListToCheck)
    {
    	Write-Host "Checking $ReportPath for command $command"
    	$foundCommand = $fileContent -match "$command"
    
    	if ($foundCommand)
        {
        	Write-Highlight "$ReportPath has the command '$command'"
            $ApprovalRequired = $true
            $HasDatabaseChanges = $true
        }
    }

    if ($ApprovalRequired -eq $true)
    {
        Write-Host "Approval is required, so there has to be changes, skipping over the changes check"
    }
    else
    {
      Write-Host "Looping through all commands indicating changes for $reportPath"
      foreach ($command in $commandListForChanges)
      {
          Write-Host "Checking $ReportPath for command $command"
          $foundCommand = $fileContent -match "$command"

          if ($foundCommand)
          {
              Write-Highlight "$ReportPath has the command '$command'"
              $HasDatabaseChanges = $true
          }
      }
    }


if ($approvalRequired -eq $false)
{
	Write-Highlight "All scripts look good"
}
else
{
	Write-Highlight "One of the specific commands we look for has been found"
}

if ($HasDatabaseChanges -eq $false)
{
    Write-Highlight "No database changes were detected"
}
else
{
    Write-Highlight "Database changes have been detected"
}

Set-OctopusVariable -name "ApprovalRequired" -value $ApprovalRequired
Write-Host "Variable run conditions: ##{unless Octopus.Deployment.Error}#{Octopus.Action[$stepName].Output.ApprovalRequired}##{/unless}"
Set-OctopusVariable -name "HasDatabaseChanges" -value $HasDatabaseChanges
Write-Host "Variable run conditions: ##{unless Octopus.Deployment.Error}#{Octopus.Action[$stepName].Output.HasDatabaseChanges}##{/unless}"