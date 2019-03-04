## Orleans Starter Kit

[![Build status](https://ci.appveyor.com/api/projects/status/tbnukulpu51v4obo?svg=true)](https://ci.appveyor.com/project/JorgeCandeias/orleansstarterkit)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=JorgeCandeias_OrleansStarterKit&metric=coverage)](https://sonarcloud.io/dashboard?id=JorgeCandeias_OrleansStarterKit)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=JorgeCandeias_OrleansStarterKit&metric=alert_status)](https://sonarcloud.io/dashboard?id=JorgeCandeias_OrleansStarterKit)

The objective of this template is to provide developers with a shortcut to setting up a full end-to-end _real-time_ microservice from the ground up
by providing a pre-configured and pre-tested combination of front-end and back-end software and providers, with Microsoft Orleans at its core.

Its dream target is to enable developers to slice through devops efforts like butter and start writing _and deploying_ user stories from day one.

**Attention: This project is in early prototyping stages. Things will break and move around. These docs will change a lot. Use at your own risk.**

## Minimum Requirements

* Microsoft Visual Studio Community 2017
* .NET Core 2.1

## Baked-In

* Hosting:
  * On Premises.

* Clustering:
  * ADO.NET > SQL Server Database

* Reminders:
  * ADO.NET > SQL Server Database

* Grain Storage:
  * ADO.NET > SQL Server Database

* Streaming:
  * SMS

* Logging:
  * Serilog > Console
  * Serilog > SQL Server Database

* Deployment:
  * AppVeyor

* Quality:
  * Testing: xUnit
  * Coverage: Coverlet > SonarQube
  * Inspection: SonarQube

## Todos

The Orleans Starter Kit is in early prototyping phase.

Tasks that need to happen are in the [Prototyping](https://github.com/JorgeCandeias/OrleansStarterKit/projects/1) project.

## How To Use

Clone the repository.

``` bash
git clone https://github.com/JorgeCandeias/OrleansStarterKit.git
```

*TODO*