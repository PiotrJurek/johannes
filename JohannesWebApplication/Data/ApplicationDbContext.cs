﻿using JohannesWebApplication.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace JohannesWebApplication.Data;

public class ApplicationDbContext : IdentityDbContext
{
    public DbSet<PrinterModel> Printers { get; set; }
    public DbSet<MaterialModel> Materials { get; set; }
    public DbSet<UserPrinter> UserPrinters { get; set; }
    public DbSet<OrderModel> Orders { get; set; }
    public DbSet<ApplicationUser> ApplicationUsers { get; set; }
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    }
}