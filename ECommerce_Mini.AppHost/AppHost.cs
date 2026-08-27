var builder = DistributedApplication.CreateBuilder(args);

//var cache = builder.AddRedis("cache");
var dbPassword = builder.AddParameter("postgres-password");

// Truyền biến dbPassword vào cấu hình PostgreSQL
var postgres = builder.AddPostgres("postgres", password: dbPassword, port: 5432)
                      .WithDataVolume().WithPgAdmin();

var authDb = postgres.AddDatabase("authdb");

var dbgate = builder.AddDbGate("dbgate")
                    .WithReference(postgres)
                    .WithEnvironment("CONNECTIONS", "con1")
                    .WithEnvironment("LABEL_con1", "Postgres DB")
                    .WithEnvironment("SERVER_con1", "postgres")
                    .WithEnvironment("PORT_con1", "5432") 
                    .WithEnvironment("USER_con1", "postgres")
                    .WithEnvironment("PASSWORD_con1", dbPassword.Resource.Value)
                    .WithEnvironment("ENGINE_con1", "postgres@dbgate-plugin-postgres");

builder.AddProject<Projects.AuthService>("authservice")
       .WithReference(authDb);


//var apiService = builder.AddProject<Projects.ECommerce_Mini_ApiService>("apiservice")
//    .WithHttpHealthCheck("/health");


builder.Build().Run();
