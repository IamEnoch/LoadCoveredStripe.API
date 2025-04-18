using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using LoadCoveredStripe.API.Models;

namespace LoadCoveredStripe.API.Domain.Entities;

public class CustomerBilling
{
    [Key]
    public Guid Id { get; set; }

    [ForeignKey("Customer")]
    [Required]
    public int CustomerId { get; set; }
    
    [Required, MaxLength(255)]
    public string StripeCustomerId { get; set; }
    
    [MaxLength(255)]
    public string? StripeSubscriptionId { get; set; }
    
    [MaxLength(255)]
    public string? PriceId { get; set; }
    
    [MaxLength(50)]
    public string? Status { get; set; }
    
    public DateTime? CurrentPeriodStart { get; set; }
    
    public DateTime? CurrentPeriodEnd { get; set; }
    
    public DateTime? CancelAt { get; set; }
    
    public DateTime? PausedFrom { get; set; }
    
    public DateTime? PausedUntil { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Foreign Key relationship to Customer table
    [ForeignKey("CustomerId")]
    public virtual Customer Customer { get; set; }
}
