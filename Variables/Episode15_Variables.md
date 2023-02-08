Tenant Common Variables Variable Set (New Variable Set)
- Tenant Abbreviation Common Variable
    - Name: Tenant.Common.Abbreviation
    - Label: Tenant Abbreviation
    - Help Text: The Abbr of the tenant's name
    - Type: Single-line Textbox

Trident Project Variables
- Runbook API Key Variable
    - Name: Project.Runbook.Api.Key
    - Value: The API Key you creted
    - Type: Sensitive
- Connection String For Database Deployments
    - Name: Project.Connection.String
    - Value: Server=#{Project.Database.Server.Name};Integrated Security=true;Database=#{Project.Database.Name}
    - Type: Text
- Report Path Variable (UPDATE!)
    - Name: Project.Database.Report.Path
    - Value: C:\DatabaseReports\#{Tenant.Common.Abbreviation}\#{Octopus.Environment.Name}
    - Type: Text
- Connection String Variable For Configuration Transform
    - Name: ConnectionStrings:Database
    - Value: #{Project.Connection.String}
    - Type: Text
- Database Name Variable (UPDATE!)
    - Name: Project.Database.Name     
    - Value: Trident_#{Tenant.Common.Abbreviation} (Production Scoping)
    - Value: Trident_#{Tenant.Common.Abbreviation}_#{Octopus.Environment.Name}
    - Type: Text
- Database Server Variable
    - Name: Project.Database.Server.Name    
    - Value: (localdb)\MSSQLLocalDB
    - Type: Text

Notification Library Variable Set (NO CHANGES NEED TO BE MADE)
- Notification Body Variable 
    - Name: Notification.Body.Text
    - Value: You can view the the deployment here: #{Octopus.Web.ServerUri}/app#/#{Octopus.Space.Id}/tasks/#{Octopus.Task.Id}
    - Type: Text
- Notification Subject 
    - Name: Notification.Subject.Text
    - Value: #{Octopus.Project.Name} #{Octopus.Release.Number} to #{Octopus.Environment.Name} has #{if Octopus.Deployment.Error}failed#{else}completed successfully#{/if}
    - Type: Text
- Slack Webhook URL (new!)
    - Name: Notification.Slack.Webhook.Url
    - Value: Use the value provided by your slack administrator
    - Type: Text

Library Variable Set SQL Verification (NO CHANGES NEED TO BE MADE)
- SQL Command List Indicating Change Variable
    - Name: SQL.Verification.Change.List
    - Value: #{SQL.Verification.Command.List},With,Join,Alter Procedure
    - Type: Text
- SQL Command List To Monitor Variable
    - Name: SQL.Verification.Command.List
    - Value: Create Table,Alter Table,Drop Table,Drop View,Create View,Create Function,Drop Function,sp_addrolemember,sp_droprolemember,alter role,Merge,Create Schema,Alter View,Alter Table
    - Type: Text