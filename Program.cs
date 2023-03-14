using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.SqlServer;
using PlatformService.SyncDataServices.Http;
using System;
using PlatformService.AsyncDataServices;
using PlatformService.SyncDataServices.Grpc;

var builder = WebApplication.CreateBuilder(args);


//Console.WriteLine("--> Using InMem Db");
//builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));

builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
builder.Services.AddGrpc();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

//if (env.IsProduction())
//{
Console.WriteLine("--> Using SqlServer Db");
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn")));
//}
//else
//{

//}
var app = builder.Build();
IWebHostEnvironment env = app.Environment;


Console.WriteLine("preparing for start");
Console.WriteLine($"CommandService Endpoint {builder.Configuration["CommandService"]}");
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

}

//app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapControllers();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    PrepDb.PrepPopulation(app, env.IsProduction());
}

    app.MapControllers();
    app.MapGrpcService<GrpcPlatformService>();
    app.MapGet("/protos/platforms.proto", async context =>
    {
        Console.WriteLine("---> Reading PROTOS");
        await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
    });



app.Run();

//PrepDb.PrepPopulation(app, env.IsProduction());


