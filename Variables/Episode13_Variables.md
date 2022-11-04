Add the following variables
- Project.Connection.String: Server=(localdb)\MSSQLLocalDB;Integrated Security=true;Database=#{Project.Database.Name}
- Project.Runbook.Api.Key
    - Sensitive Variable
    - API Key of Service Account

Change the following variables
- ConnectionStrings:Database
    - New Value: #{Project.Connection.String}
- Project.Database.Name
    - New Value: Trident (scoped to Production)
    - New Value: Trident_#{Octopus.Environment.Name} (Scoped to Dev, QA, Staging)

