using SqlSugar;

namespace ORM_SqlSugar.Repositories;

public class Repository<T> : SimpleClient<T> where T : class, new()
{
    public Repository(ISqlSugarClient context) : base(context)
    {
    }
}
