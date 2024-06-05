using Microsoft.EntityFrameworkCore;

using MiniProfilerEfCore.Data;

using StackExchange.Profiling.Storage;

using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpContextAccessor();

var dbConnectionString = "Data Source=applicaiton.db";
bool profilerUseDb = false;

builder.Services.AddDbContext<ApplicationDbContext>(options => {
    options.UseSqlite(dbConnectionString);
});

builder.Services.AddMiniProfiler(options => {
    options.RouteBasePath = "/profiler";

    options.UserIdProvider = (request) => 
    {
        var id = request.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? request.HttpContext.User.FindFirst("sub")?.Value;
        return id;
    };

    if (profilerUseDb)
    {
        var storage = new SqliteStorage(dbConnectionString, "MiniProfilers", "MiniProfilerTimings", "MiniProfilerClientTimings");

        try
        {
            storage.CreateSchema(dbConnectionString, []);
        }
        catch (Exception)
        {
        }
        options.Storage = storage;
    }
    else
    {
        ((MemoryCacheStorage)options.Storage).CacheDuration = TimeSpan.FromMinutes(60);
    }

    options.SqlFormatter = new StackExchange.Profiling.SqlFormatters.InlineFormatter();
    //跟踪连接打开关闭
    options.TrackConnectionOpenClose = true;
    //界面主题颜色方案;默认浅色
    options.ColorScheme = StackExchange.Profiling.ColorScheme.Dark;
    //.net core 3.0以上：对MVC过滤器进行分析
    options.EnableMvcFilterProfiling = true;
    //对视图进行分析
    options.EnableMvcViewProfiling = true;

    //控制访问页面授权，默认所有人都能访问
    //options.ResultsAuthorize;
    //要控制分析哪些请求，默认说有请求都分析
    //options.ShouldProfile;

    //内部异常处理
    //options.OnInternalError = e => MyExceptionLogger(e);

})
    .AddEntityFramework();

var app = builder.Build();


app.UseDefaultFiles();

app.UseStaticFiles();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(config => {
        config.IndexStream = () => typeof(Program).Assembly.GetManifestResourceStream("MiniProfilerEfCore.wwwroot.index.html");
        //config.RoutePrefix = string.Empty;
        config.SwaggerEndpoint("/swagger/v1/swagger.json", "MiniProfilerEfCore");
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseMiniProfiler();

app.MapControllers();

app.Run();

partial class Program
{
}
