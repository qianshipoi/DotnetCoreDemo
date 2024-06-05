using SqlSugar;

namespace ORM_SqlSugar.Data;

[SugarTable("todo")]
public class Todo
{
    [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
    public int Id { get; set; }
    [SugarColumn(Length = 100, IsNullable = false)]
    public string Title { get; set; } = default!;
    public bool IsCompleted { get; set; }
}
