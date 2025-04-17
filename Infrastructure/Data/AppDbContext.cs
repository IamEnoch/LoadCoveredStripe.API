using System;
using System.Collections.Generic;
using LoadCoveredStripe.API.Models;
using Microsoft.EntityFrameworkCore;

namespace LoadCoveredStripe.API.Data.AppDbContext;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<CustomerBilling> CustomerBillings { get; set; }
    public virtual DbSet<PriceCatalog> PriceCatalogs { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseMySQL("server=localhost;port=3306;database=loadcovered;uid=root;pwd=Data@123456");    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Customer configuration
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.CustomerId).HasName("PRIMARY");

            entity.ToTable("customer");

            entity.Property(e => e.CustomerId).HasColumnName("customer_id");
            entity.Property(e => e.Address1)
                .HasMaxLength(128)
                .HasColumnName("address_1");
            entity.Property(e => e.Address2)
                .HasMaxLength(128)
                .HasColumnName("address_2");
            entity.Property(e => e.Address3)
                .HasMaxLength(50)
                .HasColumnName("address_3");
            entity.Property(e => e.BirthDate)
                .HasColumnType("datetime")
                .HasColumnName("birth_date");
            entity.Property(e => e.CheckoutDate)
                .HasColumnType("datetime")
                .HasColumnName("checkout_date");
            entity.Property(e => e.CheckoutUserId).HasColumnName("checkout_user_id");
            entity.Property(e => e.City)
                .HasMaxLength(50)
                .HasColumnName("city");
            entity.Property(e => e.Classification)
                .HasMaxLength(20)
                .HasColumnName("classification");
            entity.Property(e => e.CompanyId).HasColumnName("company_id");
            entity.Property(e => e.Country)
                .HasMaxLength(50)
                .HasColumnName("country");
            entity.Property(e => e.County)
                .HasMaxLength(50)
                .HasColumnName("county");
            entity.Property(e => e.CreditLimit).HasColumnName("credit_limit");
            entity.Property(e => e.CustomerClassificationCode)
                .HasMaxLength(20)
                .HasColumnName("customer_classification_code");
            entity.Property(e => e.CustomerInfo)
                .HasColumnType("text")
                .HasColumnName("customer_info");
            entity.Property(e => e.CustomerLink)
                .HasMaxLength(50)
                .HasColumnName("customer_link");
            entity.Property(e => e.CustomerNumber)
                .HasMaxLength(20)
                .HasColumnName("customer_number");
            entity.Property(e => e.CustomerPricingCode)
                .HasMaxLength(20)
                .HasColumnName("customer_pricing_code");
            entity.Property(e => e.CustomerType)
                .HasMaxLength(20)
                .HasColumnName("customer_type");
            entity.Property(e => e.DeleteDate)
                .HasColumnType("datetime")
                .HasColumnName("delete_date");
            entity.Property(e => e.DeleteUserId).HasColumnName("delete_user_id");
            entity.Property(e => e.Email)
                .HasMaxLength(128)
                .HasColumnName("email");
            entity.Property(e => e.EntryDate)
                .HasColumnType("datetime")
                .HasColumnName("entry_date");
            entity.Property(e => e.EntryUserId).HasColumnName("entry_user_id");
            entity.Property(e => e.Fein)
                .HasMaxLength(50)
                .HasColumnName("fein");
            entity.Property(e => e.InactiveDate)
                .HasColumnType("datetime")
                .HasColumnName("inactive_date");
            entity.Property(e => e.InactiveReasonCode)
                .HasMaxLength(20)
                .HasColumnName("inactive_reason_code");
            entity.Property(e => e.InitialContactDate)
                .HasColumnType("datetime")
                .HasColumnName("initial_contact_date");
            entity.Property(e => e.InitialContactUserId).HasColumnName("initial_contact_user_id");
            entity.Property(e => e.InterfaceId)
                .HasMaxLength(50)
                .HasColumnName("interface_id");
            entity.Property(e => e.LastContactDate)
                .HasColumnType("datetime")
                .HasColumnName("last_contact_date");
            entity.Property(e => e.LastContactUserId).HasColumnName("last_contact_user_id");
            entity.Property(e => e.LastPaymentAmount).HasColumnName("last_payment_amount");
            entity.Property(e => e.LastPaymentReceivedDate)
                .HasColumnType("datetime")
                .HasColumnName("last_payment_received_date");
            entity.Property(e => e.LastSaleAmount).HasColumnName("last_sale_amount");
            entity.Property(e => e.LastSaleDate)
                .HasColumnType("datetime")
                .HasColumnName("last_sale_date");
            entity.Property(e => e.LeadSource)
                .HasMaxLength(20)
                .HasColumnName("lead_source");
            entity.Property(e => e.LocationCode)
                .HasMaxLength(20)
                .HasColumnName("location_code");
            entity.Property(e => e.MailMergeName)
                .HasMaxLength(128)
                .HasColumnName("mail_merge_name");
            entity.Property(e => e.Name1)
                .HasMaxLength(50)
                .HasColumnName("name_1");
            entity.Property(e => e.Name2)
                .HasMaxLength(50)
                .HasColumnName("name_2");
            entity.Property(e => e.Name3)
                .HasMaxLength(50)
                .HasColumnName("name_3");
            entity.Property(e => e.OwnerUserId).HasColumnName("owner_user_id");
            entity.Property(e => e.ParentCustomer)
                .HasMaxLength(20)
                .HasColumnName("parent_customer");
            entity.Property(e => e.PriceCode)
                .HasMaxLength(20)
                .HasColumnName("price_code");
            entity.Property(e => e.PrimaryContact)
                .HasMaxLength(128)
                .HasColumnName("primary_contact");
            entity.Property(e => e.PrimaryContactTitle)
                .HasMaxLength(50)
                .HasColumnName("primary_contact_title");
            entity.Property(e => e.PrimaryFax)
                .HasMaxLength(20)
                .HasColumnName("primary_fax");
            entity.Property(e => e.PrimaryPhone)
                .HasMaxLength(20)
                .HasColumnName("primary_phone");
            entity.Property(e => e.PrintStatement)
                .HasMaxLength(1)
                .HasColumnName("print_statement");
            entity.Property(e => e.RestrictionCode)
                .HasMaxLength(20)
                .HasColumnName("restriction_code");
            entity.Property(e => e.RouteCode)
                .HasMaxLength(20)
                .HasColumnName("route_code");
            entity.Property(e => e.SalesStage)
                .HasMaxLength(20)
                .HasColumnName("sales_stage");
            entity.Property(e => e.SalesrepCode)
                .HasMaxLength(20)
                .HasColumnName("salesrep_code");
            entity.Property(e => e.ServiceCharge)
                .HasMaxLength(1)
                .HasColumnName("service_charge");
            entity.Property(e => e.Ssn)
                .HasMaxLength(9)
                .HasColumnName("ssn");
            entity.Property(e => e.State)
                .HasMaxLength(3)
                .HasColumnName("state");
            entity.Property(e => e.SubaccountType)
                .HasMaxLength(128)
                .HasColumnName("subaccount_type");
            entity.Property(e => e.SyncDate)
                .HasColumnType("datetime")
                .HasColumnName("sync_date");
            entity.Property(e => e.SyncLinkId).HasColumnName("sync_link_id");
            entity.Property(e => e.TaxGroupCode)
                .HasMaxLength(20)
                .HasColumnName("tax_group_code");
            entity.Property(e => e.TermsCode)
                .HasMaxLength(20)
                .HasColumnName("terms_code");
            entity.Property(e => e.UpdateCount).HasColumnName("update_count");
            entity.Property(e => e.UpdateDate)
                .HasColumnType("datetime")
                .HasColumnName("update_date");
            entity.Property(e => e.UpdateUserId).HasColumnName("update_user_id");
            entity.Property(e => e.Url)
                .HasMaxLength(128)
                .HasColumnName("url");
            entity.Property(e => e.User1)
                .HasMaxLength(1000)
                .HasColumnName("user_1");
            entity.Property(e => e.User10)
                .HasMaxLength(1000)
                .HasColumnName("user_10");
            entity.Property(e => e.User2)
                .HasMaxLength(1000)
                .HasColumnName("user_2");
            entity.Property(e => e.User3)
                .HasMaxLength(1000)
                .HasColumnName("user_3");
            entity.Property(e => e.User4)
                .HasMaxLength(1000)
                .HasColumnName("user_4");
            entity.Property(e => e.User5)
                .HasMaxLength(1000)
                .HasColumnName("user_5");
            entity.Property(e => e.User6)
                .HasMaxLength(1000)
                .HasColumnName("user_6");
            entity.Property(e => e.User7)
                .HasMaxLength(1000)
                .HasColumnName("user_7");
            entity.Property(e => e.User8)
                .HasMaxLength(1000)
                .HasColumnName("user_8");
            entity.Property(e => e.User9)
                .HasMaxLength(1000)
                .HasColumnName("user_9");
            entity.Property(e => e.VerifiedDate)
                .HasColumnType("datetime")
                .HasColumnName("verified_date");
            entity.Property(e => e.VerifiedUserId).HasColumnName("verified_user_id");
            entity.Property(e => e.ZipCode)
                .HasMaxLength(10)
                .HasColumnName("zip_code");
        });       
        
        // CustomerBilling configuration
        modelBuilder.Entity<CustomerBilling>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("customer_billing");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            
            entity.Property(e => e.CustomerId)
                .ValueGeneratedNever()
                .HasColumnName("customer_id");
                
            entity.Property(e => e.StripeCustomerId)
                .IsRequired()
                .HasMaxLength(255)
                .HasColumnName("stripe_customer_id");
                
            entity.Property(e => e.StripeSubscriptionId)
                .HasMaxLength(255)
                .HasColumnName("stripe_subscription_id");
                
            entity.Property(e => e.PriceId)
                .HasMaxLength(255)
                .HasColumnName("price_id");
                
            entity.Property(e => e.Status)
                .HasMaxLength(50)
                .HasColumnName("status");
                
            entity.Property(e => e.CurrentPeriodStart)
                .HasColumnName("current_period_start");
                
            entity.Property(e => e.CurrentPeriodEnd)
                .HasColumnName("current_period_end");
                
            entity.Property(e => e.CancelAt)
                .HasColumnName("cancel_at");
                
            entity.Property(e => e.PausedFrom)
                .HasColumnName("paused_from");
                
            entity.Property(e => e.PausedUntil)
                .HasColumnName("paused_until");
                
            entity.Property(e => e.CreatedAt)
                .HasColumnName("created_at");
                
            entity.Property(e => e.UpdatedAt)
                .HasColumnName("updated_at");
                
            entity.HasOne(d => d.Customer)
                .WithOne()
                .HasForeignKey<CustomerBilling>(d => d.CustomerId);
        });
        
        // PriceCatalog configuration
        modelBuilder.Entity<PriceCatalog>(entity =>
        {
            entity.HasKey(e => e.PriceId);
            entity.ToTable("price_catalog");
            
            entity.Property(e => e.PriceId)
                .HasMaxLength(255)
                .ValueGeneratedNever()
                .HasColumnName("price_id");
                
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100)
                .HasColumnName("name");
                
            entity.Property(e => e.UnitAmount)
                .HasColumnName("unit_amount");
                
            entity.Property(e => e.Currency)
                .IsRequired()
                .HasMaxLength(3)
                .HasColumnName("currency");
                
            entity.Property(e => e.Interval)
                .IsRequired()
                .HasMaxLength(20)
                .HasColumnName("interval");
                
            entity.Property(e => e.TrialDays)
                .HasColumnName("trial_days");
                
            entity.Property(e => e.IsActive)
                .HasColumnName("is_active");
                
            entity.Property(e => e.LastSyncedAt)
                .HasColumnName("last_synced_at");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
