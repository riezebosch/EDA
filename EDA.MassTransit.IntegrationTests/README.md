This integration test uses the same configuration keys used by the [EDA.ServiceBus.IntegrationTests](../EDA.ServiceBus.IntegrationTests)

Lookup the connection string from your service bus and execute the following command in the folder of the test project:

```c#
dotnet user-secrets set "Azure:ServiceBus:ConnectionString" "Endpoint=sb://***.servicebus.windows.net/;SharedAccessKeyName=***;SharedAccessKey=***"
```

The `topic`, `queueus` and `subscribers` are created within the test by `MassTransit` when the SAS-token posesses the `manage` claim.