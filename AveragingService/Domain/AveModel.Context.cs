﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AveragingService.Domain
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class DbAveragingTestEntities : DbContext
    {
        public DbAveragingTestEntities()
            : base("name=DbAveragingTestEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<AccountActivity> AccountActivities { get; set; }
        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<YearAccount> YearAccounts { get; set; }
        public virtual DbSet<DayIndexAccount> DayIndexAccounts { get; set; }
    }
}