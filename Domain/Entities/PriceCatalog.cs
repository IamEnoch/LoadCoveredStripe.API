using System;
using System.ComponentModel.DataAnnotations;

namespace LoadCoveredStripe.API.Models;

public class PriceCatalog
{
    [Key]
    [MaxLength(255)]
    public string PriceId { get; set; }
    
    [MaxLength(100)]
    public string Name { get; set; }
    
    public long UnitAmount { get; set; }
    
    [MaxLength(3)]
    public string Currency { get; set; }
    
    [MaxLength(20)]
    public string Interval { get; set; }
    
    public int? TrialDays { get; set; }
    
    public bool IsActive { get; set; } = true;
    
    public DateTime LastSyncedAt { get; set; } = DateTime.UtcNow;
}
