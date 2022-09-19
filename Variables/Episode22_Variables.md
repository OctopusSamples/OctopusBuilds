Create the following project variables:

- Project.Loadbalancer.Percent    
    - Value: 10 
    - Prompted: True
    - Scoping: 
        - Environment: Staging - Blue, Staging - Green, Production - Blue and Production - Green
        - Processes: Deployment Process and Update Load Balancer Runbook    
- Project.Loadbalancer.WaitInMinutes
    - Value: 10
    - Prompted: True
    - Scoping: 
        - Environment: Staging - Blue, Staging - Green, Production - Blue and Production - Green
        - Processes: Deployment Process and Update Load Balancer Runbook
- Project.Loadbalancer.HostToUpdate
    - Value: #{Project.Tenant.Loadbalancer.HostName}
    - Prompted: True
    - Scoping: 
        - Environment: Staging - Blue, Staging - Green, Production - Blue and Production - Green
        - Processes: Update Load Balancer Runbook

Create the following project templates variables:

- Project.Tenant.Loadbalancer.HostName
    - Label: Trident Project Host Name
    - Control Type: Single-line Text Box
    - Default Value: #{Octopus.Environment.Name}-#{Retail.Tenant.ShortName}
