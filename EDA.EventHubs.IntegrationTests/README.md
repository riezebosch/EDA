# Prepare Event Hubs tests

Create an event hubs namespace and event hub named `orders`.  Lookup the connection string from your event hubs and execute the following command in the folder of the test project:

```c#
dotnet user-secrets set "Azure:EventHubs:ConnectionString" "Endpoint=sb://***.servicebus.windows.net/;SharedAccessKeyName=***;SharedAccessKey=***"
```

