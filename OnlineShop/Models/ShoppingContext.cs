using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace OnlineShop.Models
{
    public partial class ShoppingContext : DbContext
    {
        public ShoppingContext()
        {
        }

        public ShoppingContext(DbContextOptions<ShoppingContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<DetailOrder> DetailOrders { get; set; }
        public virtual DbSet<Discount> Discounts { get; set; }
        public virtual DbSet<Manufacturer> Manufacturers { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Ram> Rams { get; set; }
        public virtual DbSet<Rom> Roms { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<WareHouse> WareHouses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Server=.;Database=Shopping;Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.HasNoKey();

                entity.Property(e => e.Cmt).HasMaxLength(250);

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Product)
                    .WithMany()
                    .HasForeignKey(d => d.ProductId)
                    .HasConstraintName("FK_Comment_Product");

                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Comment_User");
            });

            modelBuilder.Entity<DetailOrder>(entity =>
            {
                entity.HasKey(e => e.OrderDetailId)
                    .HasName("PK_DetailOder");

                entity.ToTable("DetailOrder");

                entity.Property(e => e.OrderDetailId).HasColumnName("OrderDetailID");

                entity.Property(e => e.DataCreate).HasColumnType("date");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.DetailOrders)
                    .HasForeignKey(d => d.OrderId)
                    .HasConstraintName("FK_DetailOrder_Order1");
            });

            modelBuilder.Entity<Discount>(entity =>
            {
                entity.ToTable("Discount");

                entity.Property(e => e.DiscountId).HasColumnName("DiscountID");

                entity.Property(e => e.DiscountCode)
                    .HasMaxLength(8)
                    .IsUnicode(false);

                entity.Property(e => e.EndDate).HasColumnType("date");

                entity.Property(e => e.Note).HasMaxLength(250);

                entity.Property(e => e.StartDate).HasColumnType("date");
            });

            modelBuilder.Entity<Manufacturer>(entity =>
            {
                entity.ToTable("Manufacturer");

                entity.Property(e => e.ManufacturerId).HasColumnName("ManufacturerID");

                entity.Property(e => e.ManufacturerName).HasMaxLength(250);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Order");

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.BankAccountNumber)
                    .HasMaxLength(250)
                    .IsUnicode(false);

                entity.Property(e => e.Note).HasMaxLength(250);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK_Order_User");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.Property(e => e.DiscountId).HasColumnName("DiscountID");

                entity.Property(e => e.ManufacturerId).HasColumnName("ManufacturerID");

                entity.Property(e => e.NameProduct).HasMaxLength(250);

                entity.Property(e => e.RamId).HasColumnName("RamID");

                entity.Property(e => e.RomId).HasColumnName("RomID");

                entity.HasOne(d => d.Discount)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.DiscountId)
                    .HasConstraintName("FK_Product_Discount");

                entity.HasOne(d => d.Manufacturer)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.ManufacturerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_Manufacturer1");

                entity.HasOne(d => d.Ram)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.RamId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_Ram");

                entity.HasOne(d => d.Rom)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.RomId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Product_Rom");
            });

            modelBuilder.Entity<Ram>(entity =>
            {
                entity.ToTable("Ram");

                entity.Property(e => e.RamId).HasColumnName("RamID");
            });

            modelBuilder.Entity<Rom>(entity =>
            {
                entity.ToTable("Rom");

                entity.Property(e => e.RomId).HasColumnName("RomID");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("User");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.Code).HasMaxLength(50);

                entity.Property(e => e.Email).HasMaxLength(50);

                entity.Property(e => e.FristName).HasMaxLength(50);

                entity.Property(e => e.LastName).HasMaxLength(50);

                entity.Property(e => e.Password).HasMaxLength(250);

                entity.Property(e => e.Phone)
                    .HasMaxLength(15)
                    .IsFixedLength(true);
            });

            modelBuilder.Entity<WareHouse>(entity =>
            {
                entity.ToTable("WareHouse");

                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.ProductId).HasColumnName("ProductID");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.WareHouses)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_WareHouse_Product");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
