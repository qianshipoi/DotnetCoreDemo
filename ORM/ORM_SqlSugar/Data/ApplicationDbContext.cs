using SqlSugar;

namespace ORM_SqlSugar.Data;

public class ApplicationDbContext : SugarUnitOfWork
{
    public DbSet<Todo> Todos { get; set; } = default!;
}

/// <summary>
/// 自定义仓储
/// </summary>
/// <typeparam name="T"></typeparam>
public class DbSet<T> : SimpleClient<T> where T : class, new()
{
    //可以重写仓储方法
    //可以添加新方法
}
