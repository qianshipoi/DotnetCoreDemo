using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiniProfilerEfCore.Data;

public class Todo
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required, MaxLength(512)]
    public required string Title { get; set; } 
    public bool IsCompleted { get; set; }
}
