using Aspire.Hosting;

using Azure.Provisioning.PostgreSql;

using Microsoft.Extensions.Hosting;

var builder = DistributedApplication.CreateBuilder(args);

var postgresUserName = builder.AddParameter("postgres-username", secret: true);
var postgresPassword = builder.AddParameter("postgres-password", secret: true);
var azureMapsKey = builder.AddParameter("azure-maps-key", secret: true);
var auth0ClientId = builder.AddParameter("auth0-client-id", secret: true);
var auth0Domain = builder.AddParameter("auth0-domain");
var auth0Audience = builder.AddParameter("auth0-audience");

var postgres = builder.AddAzurePostgresFlexibleServer("postgres")  
                      .RunAsContainer(container =>
                      {
                          if (builder.Environment.IsDevelopment())
                          {
                              container.WithImage("postgis/postgis");
                              container.WithDataBindMount(
                                  source: @"..\..\data\postgresql",
                                  isReadOnly: false
                                )
                              .WithPgWeb();
                          }
                          else
                          {
                                container.WithDataVolume(
                                    name: "postgres-data",
                                    isReadOnly: false
                                    );
                          }
                      })
                      .WithPasswordAuthentication(postgresUserName, postgresPassword);

var db = postgres.AddDatabase("application-db");

var databaseMigrations = 
    builder.AddProject<Projects.Application_Services_DatabaseMigrations>("database-migrations")
        .WithReference(db)
        .WaitFor(db);

var server =
    builder.AddProject<Projects.Application_Services_API>("api-server")
        .WithReference(db)
        .WithExternalHttpEndpoints()
        .WithEnvironment("Application__AzureMapsKey", azureMapsKey)
        .WithEnvironment("Auth0__Domain", auth0Domain)
        .WithEnvironment("Auth0__Audience", auth0Audience)
        .WithEnvironment("Authentication__Schemes__Bearer__Authority", $"https://{auth0Domain}")
        .WithEnvironment("Authentication__Schemes__Bearer__ValidAudiences__0", auth0Audience)
        .WithEnvironment("Authentication__Schemes__Bearer__ValidIssuer", auth0Domain)
        .WaitForCompletion(databaseMigrations);

var client =
    builder.AddNpmApp("web-client", @"..\Application.Clients.Web", builder.Environment.IsDevelopment() ? "dev" : "build")
        .WithReference(server)
        .WithHttpEndpoint(env: "PORT")
        .WithExternalHttpEndpoints()
        .WithEnvironment("VITE_AUTH0_DOMAIN", auth0Domain)
        .WithEnvironment("VITE_AUTH0_CLIENT_ID", auth0ClientId)
        .WithEnvironment("VITE_AUTH0_AUDIENCE", auth0Audience)
        .WithEnvironment("VITE_API_BASE_URL", server.GetEndpoint("https"))
        .PublishAsDockerFile();

builder.Build().Run();
