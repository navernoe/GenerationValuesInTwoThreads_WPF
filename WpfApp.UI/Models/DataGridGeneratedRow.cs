using System;

namespace WpfApp.UI.Models;

public class DataGridGeneratedRow
{
    public int? CarId { get; set; }
    
    public int? DriverId { get; set; }
    public string? CarName { get; set; }
    
    public string? DriverName { get; set; }
    
    public DateTimeOffset GeneratedDateTime { get; set; }
}