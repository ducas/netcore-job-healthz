# netcore-job-healthz

A simple library for adding a health check endpoint to your jobs

## Why...?

Background jobs either run continuously or on a schedule. Either way, they can sometimes end up in a state where they are not actually doing anything. For example, a job that runs on a schedule to query a database for work and then acts on that work may get stuck somewhere in a loop. When this happens you want that job to be killed and restarted rather than just continuing on for hours doing nothing.

Orchestrators (like Kubernetes) allow you to perform periodical health checks on your applications using probes. If the health check fails, the application can be easily killed and rescheduled. The */heathz* endpoint is a common practice for such health checks.

## Getting started

1 - Install JobHealthz using NuGet - [https://www.nuget.org/packages/JobHealthz/](https://www.nuget.org/packages/JobHealthz/)

2 - In your application's startup, intialize *Healthz* and call Start with a callback for evaluating the health of the running process - e.g.

    new Healthz()
        .Start(() => return new HealthzResult {
            Status = HealthzStatus.Ok,
            Message = "Nothing to see here..."
        });


## How does it work?

This library uses Microsoft.AspNetCore.Server.Kestrel to expose a single HTTP endpoint. When you start it, it will configure the web server using the default ASP.NET Core URLs and map the */healthz* endpoint to a health check. This is what you'll see on your console:

    Health checks listening on http://localhost:5000,https://localhost:5001 at /healthz.

You can change the base URL by setting the *ASPNETCORE_URLS* environment variable - e.g. `ASPNETCORE_URLS=http://localhost dotnet run`.

When the /healthz endpoint is called, the callback provided will be invoked and the result translated to a HTTP response - e.g.

    $ curl localhost:5000/healthz -v
    *   Trying ::1...
    * TCP_NODELAY set
    * Connected to localhost (::1) port 5000 (#0)
    > GET /healthz HTTP/1.1
    > Host: localhost:5000
    > User-Agent: curl/7.54.0
    > Accept: */*
    >
    < HTTP/1.1 200 OK
    < Date: Thu, 16 Aug 2018 11:08:50 GMT
    < Server: Kestrel
    < Transfer-Encoding: chunked
    <
    * Connection #0 to host localhost left intact
    Nothing to see here...

## Logging health checks

To get more visibility into the health checks, you can use Microsoft's libraries for DI and Logging to initialise Healthz with logging enabled.

    var serviceProvider = new ServiceCollection()
        .AddLogging(l => l.AddConsole())
        .AddSingleton<Healthz>()
        .BuildServiceProvider();

    serviceProvider.GetService<Healthz>()
        .Start(() => return new HealthzResult {
                Status = HealthzStatus.Ok,
                Message = "Nothing to see here..."
            });;

## Using IHealthCheck

Another way of specifying health checks is to implement the *IHealthCheck* interface and call Healthz.Start with it. There is also an implementation of this in the library called *LastCheckpointHealthCheck* which keeps track of the last time you call *Checkpoint()* and will return an *Ok* response while it is within a threshold. E.g.

    var check = new LastCheckpointHealthCheck(TimeSpan.FromMinutes(1));
    new Healthz.Start(check);
    while (true) {
        // do work...
        check.Checkpoint();
    }

If it takes longer than 1 minute between calls to *Checkpoint()* in the above snippet, the endpoint will return a *Fail* response.
