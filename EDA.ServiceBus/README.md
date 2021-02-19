# Azure Service Bus

Using **Azure Service Bus** as an **adapter** for the **Event Driven Architecture**.

* A `topic` is used to fan-out events to `subscribers`.
* The `type` of event is annotated in the `subject` or `label` property.
* Routing is implemented using `Rules` on the `subscribers`.