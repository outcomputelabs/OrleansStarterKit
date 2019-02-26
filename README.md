## Orleans Starter Kit

[![Build status](https://ci.appveyor.com/api/projects/status/tbnukulpu51v4obo?svg=true)](https://ci.appveyor.com/project/JorgeCandeias/orleansstarterkit)
[![Coverage Status](https://coveralls.io/repos/github/JorgeCandeias/OrleansStarterKit/badge.svg?branch=master)](https://coveralls.io/github/JorgeCandeias/OrleansStarterKit?branch=master)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=JorgeCandeias_OrleansStarterKit&metric=alert_status)](https://sonarcloud.io/dashboard?id=JorgeCandeias_OrleansStarterKit)

The objective of this template is to provide developers with a shortcut to setting up a full end-to-end _real-time_ microservice from the ground up
by providing a pre-configured and pre-tested combination of front-end and back-end software and providers, with Microsoft Orleans at its core.

Its dream target is to enable developers to slice through devops efforts like butter and start writing _and deploying_ user stories from day one.

**Attention: This project is in early prototyping stages. Things will break and move around. These docs will change a lot. Use at your own risk.**

## Minimum Requirements

* Microsoft Visual Studio Community 2017
* .NET Core 2.1

## Pre-Configured Templates

* Hosting:
  * Local Only.

* Clustering:
  * ADO.NET > SQL Server Database

* Reminders:
  * ADO.NET > SQL Server Database

* Grain Storage:
  * ADO.NET > SQL Server Database

* Streaming:
  * SMS

* Deployment:
  * AppVeyor

* Quality:
  * Testing: xUnit
  * Coverage: OpenCover
  * Quality: SonarQube

## Todos

Current items in the todo list for this project, not necessarily in the order written:

* Develop Sample Real-Time Chat REST API.
* Develop Sample Real-Time Chat Web Site.
* Develop Sample SignalR Hub.
* Setup Performance Testing
* Setup Orleans Dashboard
* Setup Azure Cloud Hosting Template.
* Setup Azure Service Fabric Hosting Template.
* Setup Azure Kubernetes Hosting Template.
* Setup Azure Table Storage Clustering Template.
* Setup AWS DynamoDB Clustering Template.
* Setup Azure Table Storage Reminders Template.
* Setup AWS DynamoDB Reminders Template.
* Setup Azure Table Storage Grain Storage Template.
* Setup Azure Blob Storage Grain Storage Template.
* Setup AWS DynamoDB Grain Storage Template.
* Setup Azure EventHub Streaming Template.
* Setup Azure Queues Streaming Template.
* Setup AWS SQS Streaming Template.
* Setup Microsoft Bond Serializer.
* Setup Performance Counters Telemetry Consumer.
* Setup Azure Application Insights Telemetry Consumer.
* Setup Azure Storage Transactions Template.
* Setup Elastic Search Logging.
* Develop a Step-by-Step Installation Wizard.

## How To Use

Clone the repository.

``` bash
git clone https://github.com/JorgeCandeias/OrleansStarterKit.git
```

*TODO*