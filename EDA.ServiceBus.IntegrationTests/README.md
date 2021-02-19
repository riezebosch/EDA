# Prepare ServiceBus tests

Lookup the connection string from your service bus and execute the following command in the folder of the test project:

```c#
dotnet user-secrets set "Azure:ServiceBus:ConnectionString" "Endpoint=sb://***.servicebus.windows.net/;SharedAccessKeyName=***;SharedAccessKey=***"
```

The `topic` and `subscribers` are created within the test using some extension methods on the `ServiceBusAdministrationClient`.