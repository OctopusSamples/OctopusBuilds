- Notification.Body.Text: You can view the the deployment here: #{Octopus.Web.ServerUri}/app#/#{Octopus.Space.Id}/tasks/#{Octopus.Task.Id}
- Notification.Subject.Text: #{Octopus.Project.Name} #{Octopus.Release.Number} to #{Octopus.Environment.Name} has #{if Octopus.Deployment.Error}failed#{else}completed successfully#{/if}
- Notification.Slack.Webhook.Url: This is provided by your slack administrator
