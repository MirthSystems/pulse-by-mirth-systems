var builder = DistributedApplication.CreateBuilder(args);

var keycloak =
    builder.AddKeycloak("keycloak", 8080)
        .WithDataBindMount(
            source: @"..\..\data\keycloak"
        );

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
        .WithReference(keycloak)
        .WaitForCompletion(databaseMigrations)
        .WaitFor(cache)
        .WaitFor(keycloak);

builder.Build().Run();
