Project updates to make:

- Octopus.ServiceNow.ChangeRequest.Number
    - Remove Production - Green and Production - Blue from the scoping
    - Add Production to scoping
- Project.Database.Name
    - Remove "Trident_Staging_#{Retail.Tenant.ShortName}" value
    - Add Staging scoping to the value for "Trident_#{Octopus.Environment.Name}_#{Retail.Tenant.ShortName}"
- Project.Database.Report.Path
    - Remove both the items for "C:\DatabaseReports\Staging\#{Retail.Tenant.ShortName}" and "C:\DatabaseReports\Production\#{Retail.Tenant.ShortName}"
- Project.Loadbalancer.HostToUpdate
    - Remove this variable completely
- Project.Loadbalancer.Percent
    - Remove this variable completely
- Project.Loadbalancer.WaitInMinutes
    - Remove this variable completely
- Project.LoadBalancer.UpdateTime
    - New variable
    - Value: #{Retail.LoadBalancer.SwapTimeInUtc}
        - Scoping: Environment is Production and Process is Deployment Process
        - Prompted Variable
    - Value: N/A
        - Scoping: Environment is Staging and Process is Deployment Process
        - Not a prompted variable

Library Variable Set "Retail Location" Updates.  

Make sure you have a tenant tag for retail location defined!

- Retail.LoadBalancer.SwapTimeInUtc
    - New Variable
    - Value: Tomorrow 04:00
        - Scoping: US-Central Tenant Tag
    - Value: Tomorrow 03:00
        - Scoping: US-Eastern Tenant Tag
    - Value: Tomorrow 06:00
        - Scoping: US-Pacific Tenant Tag
    - Value: Tomorrow 05:00
        - Scoping: US-Mountain Tenant Tag
