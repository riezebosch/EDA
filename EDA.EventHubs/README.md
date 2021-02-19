# Azure EventHubs

Using **Azure EventHubs** as an **adapter** for the **Event Driven Architecture**.

* All messages are published on a single hub.
* The `type` of event is annotated in a custom `event` property.
* Routing is implemented on the receiver side by inspecting the custom property before passing it to the service.

Helper methods are available on the `BlobServiceClient` to create a checkpoint store when initializing a `EventProcessorClient`.