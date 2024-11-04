using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DataAccess.Models;

public partial class ViroCureFal2024dbContext : DbContext
{
    public ViroCureFal2024dbContext()
    {
    }

    public ViroCureFal2024dbContext(DbContextOptions<ViroCureFal2024dbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Person> People { get; set; }

    public virtual DbSet<PersonVirus> PersonViruses { get; set; }

    public virtual DbSet<ViroCureUser> ViroCureUsers { get; set; }

    public virtual DbSet<Virus> Viruses { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Server=localhost;uid=sa;pwd=12345;database= ViroCureFAL2024DB;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>(entity =>
        {
            entity.HasKey(e => e.PersonId).HasName("PK__Person__543848DFDDB223B6");

            entity.ToTable("Person");

            entity.Property(e => e.PersonId)
                .ValueGeneratedNever()
                .HasColumnName("person_id");
            entity.Property(e => e.BirthDay).HasColumnName("birth_day");
            entity.Property(e => e.Fullname)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("fullname");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.People)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__Person__user_id__3B75D760");
        });

        modelBuilder.Entity<PersonVirus>(entity =>
        {
            entity.HasKey(e => new { e.PersonId, e.VirusId }).HasName("PK__person_v__7BFA2E3F13D07F56");

            entity.ToTable("person_virus");

            entity.Property(e => e.PersonId).HasColumnName("person_id");
            entity.Property(e => e.VirusId).HasColumnName("virus_id");
            entity.Property(e => e.ResistanceRate).HasColumnName("resistance_rate");

            entity.HasOne(d => d.Person).WithMany(p => p.PersonViruses)
                .HasForeignKey(d => d.PersonId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__person_vi__perso__403A8C7D");

            entity.HasOne(d => d.Virus).WithMany(p => p.PersonViruses)
                .HasForeignKey(d => d.VirusId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__person_vi__virus__412EB0B6");
        });

        modelBuilder.Entity<ViroCureUser>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__ViroCure__B9BE370FC41E6427");

            entity.ToTable("ViroCureUser");

            entity.HasIndex(e => e.Email, "UQ__ViroCure__AB6E61649AB77CED").IsUnique();

            entity.Property(e => e.UserId)
                .ValueGeneratedNever()
                .HasColumnName("user_id");
            entity.Property(e => e.Email)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password");
            entity.Property(e => e.Role)
                .HasDefaultValue(2)
                .HasColumnName("role");
        });

        modelBuilder.Entity<Virus>(entity =>
        {
            entity.HasKey(e => e.VirusId).HasName("PK__Virus__FC266E031DD1F624");

            entity.ToTable("Virus");

            entity.Property(e => e.VirusId)
                .ValueGeneratedNever()
                .HasColumnName("virus_id");
            entity.Property(e => e.Treatment)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("treatment");
            entity.Property(e => e.VirusName)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("virus_name");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
