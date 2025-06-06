using GarageMate.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace GarageMate.Api.Data;

public class GarageMateContext(DbContextOptions<GarageMateContext> options)
    : DbContext(options)
{
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<IndividualCustomer> IndividualCustomers => Set<IndividualCustomer>();
    public DbSet<CompanyCustomer> CompanyCustomers => Set<CompanyCustomer>();
    public DbSet<VehicleOwnership> VehicleOwnerships => Set<VehicleOwnership>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        ConfigureCustomer(modelBuilder);
        ConfigureIndividualCustomer(modelBuilder);
        ConfigureCompanyCustomer(modelBuilder);
        ConfigureVehicleOwnership(modelBuilder);
    }

    private static void ConfigureCustomer(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(c => c.Id);

            entity.Property(c => c.Type)
                .IsRequired()
                .HasConversion<string>();

            entity.Property(c => c.Email)
                .IsRequired()
                .HasMaxLength(256);

            entity.Property(c => c.PhoneNumber)
                .IsRequired()
                .HasMaxLength(20);

            entity.Property(c => c.Address)
                .HasMaxLength(512);

            entity.Property(c => c.Notes)
                .HasMaxLength(1024);

            entity.HasOne(c => c.IndividualCustomer)
                .WithOne(ic => ic.Customer)
                .HasForeignKey<IndividualCustomer>(ic => ic.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(c => c.CompanyCustomer)
                .WithOne(cc => cc.Customer)
                .HasForeignKey<CompanyCustomer>(cc => cc.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }

    private static void ConfigureIndividualCustomer(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<IndividualCustomer>(entity =>
        {
            entity.HasKey(ic => ic.CustomerId);

            entity.Property(ic => ic.FirstName)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(ic => ic.LastName)
                .IsRequired()
                .HasMaxLength(100);
        });
    }

    private static void ConfigureCompanyCustomer(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<CompanyCustomer>(entity =>
        {
            entity.HasKey(cc => cc.CustomerId);

            entity.Property(cc => cc.CompanyName)
                .IsRequired()
                .HasMaxLength(200);

            entity.Property(cc => cc.ContactPerson)
                .HasMaxLength(100);
        });
    }

    private static void ConfigureVehicleOwnership(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<VehicleOwnership>(entity =>
        {
            entity.HasKey(vo => vo.Id);

            entity.Property(vo => vo.IsCurrentOwner)
                .IsRequired();

            entity.Property(vo => vo.Notes)
                .HasMaxLength(1024);

            entity.HasOne(vo => vo.Vehicle)
                .WithMany(v => v.VehicleOwnerships)
                .HasForeignKey(vo => vo.VehicleId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(vo => vo.Customer)
                .WithMany(c => c.VehicleOwnerships)
                .HasForeignKey(vo => vo.CustomerId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
