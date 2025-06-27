using Aspire.Hosting;

using Azure.Provisioning.PostgreSql;

using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var postgres = builder.AddAzurePostgresFlexibleServer("postgres")
                      .RunAsContainer(container =>
                      {
                          container.WithImage("postgis/postgis");
                          if (builder.Environment.IsDevelopment())
                          {
                              container.WithDataBindMount(
                                    source: @"..\..\data\postgresql",
                                    isReadOnly: false
                                );
                              container.WithPgWeb();
                          }
                          else
                          {
                              container.WithDataVolume(
                                    name: "postgres-data",
                                    isReadOnly: false
                                );
                          }
                      });

var db = postgres.AddDatabase("application-db");

var databaseMigrations = 
    builder.AddProject<Projects.Application_Services_DatabaseMigrations>("database-migrations")
        .WithReference(db)
        .WaitFor(db);

var server =
    builder.AddProject<Projects.Application_Services_API>("api-server")
        .WithReference(db)
        .WithEnvironment("Application__AzureMapsKey", builder.AddParameter("azure-maps-key", secret: true))
        .WaitForCompletion(databaseMigrations);

var client =
    builder.AddNpmApp("web-client", @"..\Application.Clients.Web", "dev")
        .WithReference(server)
        .WithExternalHttpEndpoints()
        .WithEnvironment("VITE_AUTH0_DOMAIN", builder.AddParameter("auth0-domain"))
        .WithEnvironment("VITE_AUTH0_CLIENT_ID", builder.AddParameter("auth0-client-id", secret: true))
        .WithEnvironment("VITE_AUTH0_AUDIENCE", builder.AddParameter("auth0-audience"))
        .WithEnvironment("VITE_API_BASE_URL", server.GetEndpoint("https"))
        .WaitFor(server)
        .PublishAsDockerFile();

builder.Build().Run();
