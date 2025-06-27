using Aspire.Hosting;

using Azure.Provisioning.PostgreSql;

var builder = DistributedApplication.CreateBuilder(args);

var db = builder.AddPostgres("postgres")
    .WithVolume(
        name: "postgres-data",
        target: "/var/lib/postgresql/data",
        isReadOnly: false
    )
    .WithImage("postgis/postgis")
    .WithPgAdmin()
    .AddDatabase("application-db");

var databaseMigrations = 
    builder.AddProject<Projects.Application_Services_DatabaseMigrations>("database-migrations")
        .WithReference(db)
        .WaitFor(db);

var server =
    builder.AddProject<Projects.Application_Services_API>("api-server")
        .WithReference(db)
        .WaitForCompletion(databaseMigrations);

var client =
    builder.AddNpmApp("web-client", @"..\Application.Clients.Web", "dev")
        .WithReference(server)
        .WaitFor(server)
        .WithExternalHttpEndpoints()
        .WithEnvironment("VITE_AUTH0_DOMAIN", builder.AddParameter("auth0-domain"))
        .WithEnvironment("VITE_AUTH0_CLIENT_ID", builder.AddParameter("auth0-client-id", secret: true))
        .WithEnvironment("VITE_AUTH0_AUDIENCE", builder.AddParameter("auth0-audience"))
        .WithEnvironment("VITE_API_BASE_URL", server.GetEndpoint("https"))
        .PublishAsDockerFile();

builder.Build().Run();
