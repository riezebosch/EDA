# Azure Service Bus with MassTransit

> Using **MassTransit** as an **adapter** for the **Event Driven Architecture**.

Make sure the `Shared Access Policy` has `manage` claim to auto-create the `topics`, `queues` and `subscriptions` from the test.

**Note:**
This is not the best of examples on how-to do an EDA using MassTransit. 
I introduced yet-another-layer as an abstraction over MassTransit for my
services to be MassTransit agnostic. If MassTransit is all you use then I'd
probably implement the interfaces directly on my services.