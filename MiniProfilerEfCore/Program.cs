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
    //�������Ӵ򿪹ر�
    options.TrackConnectionOpenClose = true;
    //����������ɫ����;Ĭ��ǳɫ
    options.ColorScheme = StackExchange.Profiling.ColorScheme.Dark;
    //.net core 3.0���ϣ���MVC���������з���
    options.EnableMvcFilterProfiling = true;
    //����ͼ���з���
    options.EnableMvcViewProfiling = true;

    //���Ʒ���ҳ����Ȩ��Ĭ�������˶��ܷ���
    //options.ResultsAuthorize;
    //Ҫ���Ʒ�����Щ����Ĭ��˵�����󶼷���
    //options.ShouldProfile;

    //�ڲ��쳣����
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
