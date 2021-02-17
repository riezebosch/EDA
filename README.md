# Event Driven Architecture Exploration

https://hackernoon.com/events-as-first-class-citizens-8633e8479493

## Hexagonal Architecture

The services only make use of 2 ports: 
* pub
* sub

For publishing events and subscribing to events.

My goal is to implement as many adapters as possible proving different ways of building a EDA on Azure.

## Adapters

In this experiment I crafted 2 adapters for doing EDA on Azure:

* Azure ServiceBus
* Azure EventHubs

## Test Adapters

Asserting for events to arrive is hard because of eventual consistency. How long do you have to wait
before a message to arrive? What if you receive older messages first? To help with that I crafted
the `TestAdapters.Subscriber` that asserts received messages for a specific timeout with a sliding window.

# Setup the integration tests

See the `README.md` in the respective integration test projects.