using Microsoft.AspNetCore.Mvc;

using ORM_SqlSugar.Data;
using ORM_SqlSugar.Repositories;

using SqlSugar;

namespace ORM_SqlSugar.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TodoController : ControllerBase
{
    private readonly ISqlSugarClient _sqlSugarClient;
    private readonly ISugarUnitOfWork<ApplicationDbContext> _sugarUnitOfWork;
    private readonly Repository<Todo> _todoRepository;

    public TodoController(ISqlSugarClient sqlSugarClient, ISugarUnitOfWork<ApplicationDbContext> sugarUnitOfWork, Repository<Todo> todoRepository)
    {
        _sqlSugarClient = sqlSugarClient;
        _sugarUnitOfWork = sugarUnitOfWork;
        _todoRepository = todoRepository;
    }

    [HttpGet]
    public async Task<ActionResult<List<Todo>>> Get()
    {
        return await _sqlSugarClient.Queryable<Todo>().ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Todo>> Post(Todo todo)
    {
        using var uow = _sugarUnitOfWork.CreateContext();
        await uow.Todos.InsertAsync(todo);
        uow.Commit();
        //todo.Id = await _sqlSugarClient.Insertable(todo).ExecuteReturnIdentityAsync();
        return todo;
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Todo>> Get(int id)
    {
        return await _sqlSugarClient.Queryable<Todo>().InSingleAsync(id);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Todo>> Put(int id, Todo todo)
    {
        todo.Id = id;
        await _todoRepository.UpdateAsync(todo);
        //await _sqlSugarClient.Updateable(todo).ExecuteCommandAsync();
        return todo;
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        await _sqlSugarClient.Deleteable<Todo>().In(id).ExecuteCommandAsync();
        return NoContent();
    }
}
