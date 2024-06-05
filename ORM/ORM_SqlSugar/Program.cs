using ORM_SqlSugar.Data;
using ORM_SqlSugar.Repositories;
using SqlSugar;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddLogging();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<ISqlSugarClient>(sp =>
{
    var sqlSugar = new SqlSugarClient(
        new ConnectionConfig
        {
            DbType = DbType.Sqlite,
            ConnectionString = "DataSource=application.db",
            IsAutoCloseConnection = true
        },
        db =>
        {
            //获取IOC对象要求在一个上下文
            var appServive = sp.GetRequiredService<IHttpContextAccessor>();
            var log = appServive.HttpContext?.RequestServices.GetService<ILogger>();

            db.Aop.OnLogExecuting = (sql, pars) =>
            {
                log?.LogInformation(
                    message: $"{sql}\r\n{db.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value))}"
                );
            };
        }
    );

    return sqlSugar;
});

var uow = new SugarUnitOfWork<ApplicationDbContext>(
    new SqlSugarClient(
        new ConnectionConfig
        {
            DbType = DbType.Sqlite,
            ConnectionString = "DataSource=application.db",
            IsAutoCloseConnection = true
        }
    )
);

builder.Services.AddSingleton<ISugarUnitOfWork<ApplicationDbContext>>(uow);
builder.Services.AddScoped(typeof(Repository<>));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Services.CreateScope()
    .ServiceProvider.GetRequiredService<ISqlSugarClient>()
    .CodeFirst.InitTables(typeof(Todo));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
