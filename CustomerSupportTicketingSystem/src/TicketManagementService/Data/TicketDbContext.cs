using Microsoft.EntityFrameworkCore;
using TicketManagementService.Models;

namespace TicketManagementService.Data;

public class TicketDbContext : DbContext
{
    public TicketDbContext(DbContextOptions<TicketDbContext> options) : base(options)
    {
    }

    public DbSet<Ticket> Tickets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).IsRequired().HasMaxLength(2000);
            entity.Property(e => e.CustomerEmail).IsRequired().HasMaxLength(255);
            entity.Property(e => e.CustomerName).HasMaxLength(100);
            entity.Property(e => e.CustomerPhone).HasMaxLength(20);
            entity.Property(e => e.Category).IsRequired().HasMaxLength(100);
            entity.Property(e => e.SubCategory).HasMaxLength(100);
            entity.Property(e => e.AssignedAgentName).HasMaxLength(100);
            entity.Property(e => e.Tags).HasMaxLength(500);
            entity.Property(e => e.Attachments).HasMaxLength(500);
            entity.Property(e => e.InternalNotes).HasMaxLength(1000);
            entity.Property(e => e.Resolution).HasMaxLength(2000);
            entity.Property(e => e.CustomerFeedback).HasMaxLength(1000);
            entity.Property(e => e.AICategory).HasMaxLength(100);
            entity.Property(e => e.AIPriority).HasMaxLength(50);

            entity.Property(e => e.CreatedAt).IsRequired();
            entity.Property(e => e.Status).IsRequired();
            entity.Property(e => e.Priority).IsRequired();

            // Indexes for better performance
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.Priority);
            entity.HasIndex(e => e.Category);
            entity.HasIndex(e => e.CustomerEmail);
            entity.HasIndex(e => e.AssignedAgentId);
            entity.HasIndex(e => e.CreatedAt);
            entity.HasIndex(e => e.DueDate);
        });
    }
}
