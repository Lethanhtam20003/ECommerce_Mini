var builder = DistributedApplication.CreateBuilder(args);

var cache = builder.AddRedis("cache");

var postgres = builder.AddPostgres("postgres");
var authDb = postgres.AddDatabase("authdb");

builder.AddProject<Projects.AuthService>("authservice")
       .WithReference(authDb);


//var apiService = builder.AddProject<Projects.ECommerce_Mini_ApiService>("apiservice")
//    .WithHttpHealthCheck("/health");



builder.Build().Run();
