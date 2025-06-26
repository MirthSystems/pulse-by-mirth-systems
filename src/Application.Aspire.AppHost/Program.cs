var builder = DistributedApplication.CreateBuilder(args);

var db = 
    builder.AddPostgres("postgres")        
        .WithDataBindMount(
            source: @"..\..\data\postgresql",
            isReadOnly: false
        )
        .WithImage("postgis/postgis")
        .WithPgAdmin()
        .WithPgWeb()
        .AddDatabase("application-db");

var cache = 
    builder.AddRedis("cache")
        .WithDataBindMount(
            source: @"..\..\data\redis",
            isReadOnly: false
        )
        .WithRedisInsight()
        .WithRedisCommander(); 
    ;

var databaseMigrations = 
    builder.AddProject<Projects.Application_Services_DatabaseMigrations>("database-migrations")
        .WithReference(db)
        .WaitFor(db);

var server = 
    builder.AddProject<Projects.Application_Services_API>("api-server")
        .WithReference(db)
        .WithReference(cache)
        .WaitForCompletion(databaseMigrations)
        .WaitFor(cache);

var client =
    builder.AddNpmApp("web-client", @"..\Application.Clients.Web", "dev")
        .WithExternalHttpEndpoints()
        .WithReference(server)
        .WaitFor(server);

builder.Build().Run();
