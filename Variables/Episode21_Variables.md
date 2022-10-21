Update the following variables:

- Project.Database.Name
    - Value: Trident_#{Retail.Tenant.ShortName} Scoping: None
    - Value: Trident_#{Octopus.Environment.Name}_#{Retail.Tenant.ShortName} Scoping: Development and QA environments
    - Value: Trident_Staging_#{Retail.Tenant.ShortName} Scoping: Staging, Staging - Blue and Staging - Green
- Project.Database.Report.Path
    - Value: C:\DatabaseReports\\#{Octopus.Environment.Name}\#{Retail.Tenant.ShortName} Scoping: None
    - Value: C:\DatabaseReports\Production\\#{Retail.Tenant.ShortName} Scoping: Production - Blue and Production - Green
    - Value: C:\DatabaseReports\Staging\\#{Retail.Tenant.ShortName} Scoping: Staging - Blue and Staging - Green
