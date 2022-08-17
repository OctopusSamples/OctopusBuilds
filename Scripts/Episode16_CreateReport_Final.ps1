$packagePath = $OctopusParameters["Octopus.Action.Package[Trident.Database].ExtractedPath"]
$connectionString = $OctopusParameters["Project.Connection.String"]
$environmentName = $OctopusParameters["Octopus.Environment.Name"]
$reportPath = $OctopusParameters["Project.Database.Report.Path"]

cd $packagePath
$appToRun = ".\Octopus.Trident.Database.DbUp"
$generatedReport = "$reportPath\UpgradeReport.html"

& $appToRun --ConnectionString="$connectionString" --PreviewReportPath="$reportPath"

New-OctopusArtifact -Path "$generatedReport" -Name "$environmentName.UpgradeReport.html"