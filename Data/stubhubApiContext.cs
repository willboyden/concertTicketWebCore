using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using concertTicketWebCoreMVC.Models;
using System.Configuration;

namespace concertTicketWebCoreMVC.Data
{
    public partial class stubhubApiContext : DbContext
    {
        public stubhubApiContext()
        {
        }
        //cmd script to generate modals based on DB
        //Scaffold-DbContext "Server=.\SQLExpress;Database=SchoolDB;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -OutputDir Models
        public stubhubApiContext(DbContextOptions<stubhubApiContext> options)
            : base(options)
        {

        }

        public DbSet<StubHubCity> StubHubCity { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["StubHubCityContext"].ConnectionString);
            }
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<StubHubCity>(entity =>
            {
                
                entity.HasKey(e => e.Index);

                entity.ToTable("tblStubHubCity");

                entity.HasIndex(e => e.Index)
                    .HasName("ix_dbo_StubHubCity_index");

                entity.Property(e => e.Index)
                    .HasColumnName("index")
                    .ValueGeneratedNever();

                entity.Property(e => e.city)
                    .HasColumnName("city")
                    .IsUnicode(false);

                entity.Property(e => e.Country)
                    .HasColumnName("country")
                    .IsUnicode(false);

                entity.Property(e => e.CountryCode)
                    .HasColumnName("countryCode")
                    .IsUnicode(false);

                entity.Property(e => e.GeoNameId).HasColumnName("geoNameId");

                entity.Property(e => e.Latitude).HasColumnName("latitude");

                entity.Property(e => e.Longitude).HasColumnName("longitude");

                entity.Property(e => e.Score).HasColumnName("score");

                entity.Property(e => e.State)
                    .HasColumnName("state")
                    .IsUnicode(false);

                entity.Property(e => e.StateCode)
                    .HasColumnName("stateCode")
                    .IsUnicode(false);

                entity.Property(e => e.TimeZoneDisplayOffset)
                    .HasColumnName("timeZoneDisplayOffset")
                    .IsUnicode(false);

                entity.Property(e => e.TimeZoneId)
                    .HasColumnName("timeZoneId")
                    .IsUnicode(false);

                entity.Property(e => e.TimeZoneRawOffset).HasColumnName("timeZoneRawOffset");
            });

            
        

        modelBuilder
                .Entity<StubHubCityVenues>(eb =>
                {
                    eb.HasNoKey();
                    eb.ToTable("StubHubCityVenues");
                });


            OnModelCreatingPartial(modelBuilder);
        }




        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
