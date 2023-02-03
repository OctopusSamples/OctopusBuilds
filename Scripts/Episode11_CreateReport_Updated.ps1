$packagePath = $OctopusParameters["Octopus.Action.Package[Trident.Database].ExtractedPath"]
$connectionString = $OctopusParameters["Project.Connection.String"]
$commandsToLookFor = $OctopusParameters["SQL.Verification.Command.List"]
$environmentName = $OctopusParameters["Octopus.Environment.Name"]
$reportPath = $OctopusParameters["Project.Database.Report.Path"]

cd $packagePath
$appToRun = ".\Octopus.Trident.Database.DbUp"
$generatedReport = "$reportPath\UpgradeReport.html"

& $appToRun --ConnectionString="$connectionString" --PreviewReportPath="$reportPath"

New-OctopusArtifact -Path "$generatedReport" -Name "$environmentName.UpgradeReport.html"

Write-Host "Looping through all commands"
$commandListToCheck = $CommandsToLookFor -split ","
$ApprovalRequired = $false

$fileContent = Get-Content -path $generatedReport
foreach ($command in $commandListToCheck)
{
    Write-Host "Checking $($sqlFile.FileName) for command $command"
    $foundCommand = $fileContent -match "$command"

    if ($foundCommand)
    {
        Write-Highlight "$($sqlFile.FileName) has the command '$command'"
        $ApprovalRequired = $true
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

Set-OctopusVariable -name "ApprovalRequired" -value $ApprovalRequired