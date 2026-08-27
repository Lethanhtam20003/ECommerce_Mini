using AuthService.Application.Extensions;
using AuthService.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDomainServices(builder.Configuration);
builder.Services.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddJwtAuthentication(builder.Configuration);


builder.Services.AddOpenApi();
builder.AddServiceDefaults();
builder.Services.AddControllers();

builder.AddNpgsqlDbContext<AuthDbContext>("authdb");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "AuthService v1");
    }); // Giao diện truy cập tại /swagger

    // Auto-migration trong môi trường Development
    using var scope = app.Services.CreateScope();
    var dbContext = scope.ServiceProvider.GetRequiredService<AuthDbContext>();
    await dbContext.Database.MigrateAsync();
}
// Kích hoạt Exception Handler Middleware
app.UseExceptionHandler();
app.UseHttpsRedirection();
app.MapDefaultEndpoints();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();