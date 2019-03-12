## Orleans Starter Kit

[![Build status](https://ci.appveyor.com/api/projects/status/tbnukulpu51v4obo?svg=true)](https://ci.appveyor.com/project/JorgeCandeias/orleansstarterkit)
[![Coverage](https://sonarcloud.io/api/project_badges/measure?project=JorgeCandeias_OrleansStarterKit&metric=coverage)](https://sonarcloud.io/dashboard?id=JorgeCandeias_OrleansStarterKit)
[![Quality Gate Status](https://sonarcloud.io/api/project_badges/measure?project=JorgeCandeias_OrleansStarterKit&metric=alert_status)](https://sonarcloud.io/dashboard?id=JorgeCandeias_OrleansStarterKit)

The Orleans Starter Kit helps developers start adding business value from day one, using a scalable-by-default technology stack with [Microsoft Orleans](http://dotnet.github.io/orleans/) at its core.

## Progress

This project is now at MVP stage. Things will break and move around. These docs will change a lot. MVP progress [tracked here](https://github.com/JorgeCandeias/OrleansStarterKit/projects/1). The project will exit MVP stage when both on-premise and Azure providers are added and stabilized and the configuration instructions on this page are clear.

## Minimum Requirements

* Microsoft Visual Studio Community 2017
* .NET Core 2.1

## Baked-In Providers

* Hosting:
  * On Premises.

* Frontend APIs:
  * *None yet*.

* Cluster Membership:
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

## How To Use

Clone the repository.

``` bash
git clone https://github.com/JorgeCandeias/OrleansStarterKit.git
```

*TODO: Add Visual Studio instructions here.*

## Configuration

*TODO: Add configuration instructions here.*