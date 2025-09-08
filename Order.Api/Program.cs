<<<<<<< HEAD:Loyalty.Api/Program.cs
﻿using BuildingBlocks.Common.Extensions;
using Loyalty.Api;
using Loyalty.Infrastructure;
using Loyalty.Infrastructure.Persistence;
using Loyalty.Application;
=======
﻿using Order.Api;
using Order.Application;
using Order.Infrastructure;
using Order.Infrastructure.Persistence;
using BuildingBlocks.Common.Extensions;
>>>>>>> parent of 126ebf4 (api loyalty):Order.Api/Program.cs

var builder = WebApplication.CreateBuilder(args);

//builder.Host.UseSerilog()
//    .ConfigureAppConfiguration(AppConfiguration.Configure);

// Add services to the container.
builder.Services.AddControllers();

builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddAPIServices(builder.Configuration);

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseServiceDefaults(builder);

await app.InitialiseDatabaseAsync();

app.Run();

// Make the implicit Program class public so test projects can access it
public partial class Program { }

// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}

//app.UseHttpsRedirection();

//app.UseAuthorization();

//app.MapControllers();

//app.Run();

