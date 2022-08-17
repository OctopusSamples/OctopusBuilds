$packagePath = $OctopusParameters["Octopus.Action.Package[Trident.Database].ExtractedPath"]
$connectionString = $OctopusParameters["Project.Connection.String"]

cd $packagePath
$appToRun = ".\Octopus.Trident.Database.DbUp"

& $appToRun --ConnectionString="$connectionString"