using DataProtectionRedis;

using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

var builder = Host.CreateApplicationBuilder();

var redisConnStr = "127.0.0.1:6379,defaultDatabase=0";
var redis = ConnectionMultiplexer.Connect(redisConnStr);
builder.Services
    .AddDataProtection()
    .SetApplicationName("DataProtectionRedis")
    .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys");

builder.Services
    .AddStackExchangeRedisCache(options => {
        options.Configuration = redisConnStr;
        options.InstanceName = "DataProtectionRedis";
    });

builder.Services.AddSingleton<MyClass>();

var app = builder.Build();

app.Services.GetRequiredService<MyClass>().RunSample();
