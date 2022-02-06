# Seven Assignment Task

## This solution contains the following:

A console application - .Net Core 3.1 (SevenAssignmentApp)
A .Net Core 3.1 Library (SevenAssignmentLibrary) which is used a NUGET package.
A Test library, with interation and unit tests

## Description

A console application that outputs:
The users full name for id=41 (42 did not exist - so I used 41 for this task)
All the users first names (comma separated) who are 23
The number of genders per Age, displayed from youngest to oldest (I noticed some obsure genders, such as Y and T - wasnt sure what to
do with those, so treated them as Male or Female in any case)


## Nuget

The .Net Core 3.1 library has been packaged and published as a Nuget package to my person Nuget account. It is available to be downloaded and referenced.

Nuget: https://www.nuget.org/packages/SevenAssignmentLibrary/

Package Manager: Install-Package SevenAssignmentLibrary -Version 1.0.3

## Configuration

To use this Nuget Package please add an appsettings.json to the root of your app and test project, with the following default settings:

Note: this might be subject to change.

{
  "SevenAssignmentClientSettings": {
    "ServiceUrl": "https://f43qgubfhf.execute-api.ap-southeast-2.amazonaws.com",
    "EndPoint": "sampletest",
    "TimeOutInSeconds": 30
  },
  "SevenAssignmentSettings": {
    "CacheKey": "users",
    "CacheExpireMins":  30
  },
  "PollySettings": {
    "BreakDurationSeconds": 1,
    "RetryCount": 3,
    "RetrySeconds": 3,
    "NumOfExceptionsAllowed":  3
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Debug",
      "Override": {
        "Microsoft": "Error",
        "System": "Error"
      }
    }
  }
}


## Polly

Polly is a . NET resilience and transient-fault-handling library that allows developers to express policies such as Retry, Circuit Breaker,
in a fluent and thread-safe manner.

Notes/Reading: https://www.twilio.com/blog/using-polly-circuit-breakers-resilient-net-web-service-consumers

#### Retrying
I have set retries to 3, with 3 seconds wait and retry. These are configurable.


## Tests

Test are using XUnit and NSubstitute for the mocking. All tests should pass.

Note: My idea with the integration tests is to test against real objects, rather than the mocked (NSubstitute) versions.

## Caching

Caching is being used against the SevenClient responses, so we dont need to make repeated calls to that server. The effort here is
to help improve permformance and efficiency of the application. Caching expiration time is configurable.

## Logging

Serilog is being used to help generate a log file. For now, I just added afew log entries where we see exceptions. 