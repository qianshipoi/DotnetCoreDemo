namespace MiniProfilerEfCore.Data.ProfilerModels;

public partial class MiniProfilerClientTimings
{
    public int RowId { get; set; }
    public Guid Id { get; set; }
    public Guid MiniProfilerId { get; set; }
    public string Name { get; set; }
    public decimal Start { get; set; }
    public decimal Duration { get; set; }
}

public partial class MiniProfilers
{
    public int RowId { get; set; }
    public Guid Id { get; set; }
    public Guid? RootTimingId { get; set; }
    public DateTime Started { get; set; }
    public decimal DurationMilliseconds { get; set; }
    public string User { get; set; }
    public bool HasUserViewed { get; set; }
    public string MachineName { get; set; }
    public string CustomLinksJson { get; set; }
    public int? ClientTimingsRedirectCount { get; set; }
}

public partial class MiniProfilerTimings
{
    public int RowId { get; set; }
    public Guid Id { get; set; }
    public Guid MiniProfilerId { get; set; }
    public Guid? ParentTimingId { get; set; }
    public string Name { get; set; }
    public decimal DurationMilliseconds { get; set; }
    public decimal StartMilliseconds { get; set; }
    public bool IsRoot { get; set; }
    public short Depth { get; set; }
    public string CustomTimingsJson { get; set; }
}
