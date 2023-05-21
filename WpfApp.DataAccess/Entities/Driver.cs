using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace WpfApp.DataAccess.Entities;

public class Driver
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    public string Name { get; set; }
    
    public DateTimeOffset GeneratedDate { get; set; }
        
    [ForeignKey("Car")]
    public int? CarId { get; set; }

    public virtual Car? Car { get; set; }
}