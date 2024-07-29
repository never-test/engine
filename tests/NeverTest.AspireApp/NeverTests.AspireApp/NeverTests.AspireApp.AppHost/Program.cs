var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.NeverTests_AspireApp_ApiService>("apiservice");

builder
    .AddProject<Projects.NeverTests_AspireApp_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
