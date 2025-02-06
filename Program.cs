using Carter;
using Microsoft.OpenApi.Models;
using Neo4j.Configs;
using Neo4j.Driver;

var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
	.AddJsonFile("appsettings.json")
	.AddJsonFile($"appsettings.{environment}.json", optional: true)
	.AddEnvironmentVariables();

var neo4jConfig = builder.Configuration.GetRequiredSection(nameof(Neo4jConfigurations))
	.Get<Neo4jConfigurations>();

builder.Logging.ClearProviders().AddConsole();

builder.Services.AddSignalR();
builder.Services.AddControllers();

builder.Services.AddSingleton<IDriver>(sp =>
	GraphDatabase.Driver(
		neo4jConfig.Uri,
		AuthTokens.Basic(neo4jConfig.Username, neo4jConfig.Password)
	));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCarter();
builder.Services.AddSwaggerGen(c =>
{
	c.SwaggerDoc("v1", new OpenApiInfo { Title = "Messaging_System", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if(app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseRouting();
app.MapControllers();

app.MapCarter();

// add swagger for API documentation
app.UseSwagger();
app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Messaging_System v1"));

await app.RunAsync();