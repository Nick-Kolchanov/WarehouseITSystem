using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Configuration;
using WarehouseITSystem.Models;

namespace WarehouseITSystem
{
    public partial class WarehouseContext : DbContext
    {
        public WarehouseContext()
        {
        }

        public WarehouseContext(DbContextOptions<WarehouseContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Customer> Customers { get; set; } = null!;
        public virtual DbSet<User> Users { get;  set; } = null!;
        public virtual DbSet<Delivery> Deliveries { get; set; } = null!;
        public virtual DbSet<Discrepancy> Discrepancies { get; set; } = null!;
        public virtual DbSet<DiscrepancyStatus> DiscrepancyStatuses { get; set; } = null!;
        public virtual DbSet<DiscrepancyType> DiscrepancyTypes { get; set; } = null!;
        public virtual DbSet<Inventarization> Inventarizations { get; set; } = null!;
        public virtual DbSet<InventarizationReason> InventarizationReasons { get; set; } = null!;
        public virtual DbSet<MeasurementUnit> MeasurementUnits { get; set; } = null!;
        public virtual DbSet<Nomenclature> Nomenclatures { get; set; } = null!;
        public virtual DbSet<NomenclatureProperty> NomenclatureProperties { get; set; } = null!;
        public virtual DbSet<NomenclatureType> NomenclatureTypes { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductSelling> ProductSellings { get; set; } = null!;
        public virtual DbSet<ProductWorth> ProductWorths { get; set; } = null!;
        public virtual DbSet<Property> Properties { get; set; } = null!;
        public virtual DbSet<Selling> Sellings { get; set; } = null!;
        public virtual DbSet<Supplier> Suppliers { get; set; } = null!;
        public virtual DbSet<Warehouse> Warehouses { get; set; } = null!;
        public virtual DbSet<WarehouseType> WarehouseTypes { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseNpgsql(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.ToTable("customers");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.Phone)
                    .HasMaxLength(15)
                    .HasColumnName("phone");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("users");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.PasswordHash)
                    .HasMaxLength(64)
                    .HasColumnName("password_hash");

                entity.Property(e => e.IsAdmin)
                    .HasColumnName("is_admin");
            });

            modelBuilder.Entity<Delivery>(entity =>
            {
                entity.ToTable("deliveries");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Date).HasColumnName("date");

                entity.Property(e => e.SupplierId).HasColumnName("supplier_id");

                entity.HasOne(d => d.Supplier)
                    .WithMany(p => p.Deliveries)
                    .HasForeignKey(d => d.SupplierId)
                    .HasConstraintName("deliveries_supplier_id_fkey");
            });

            modelBuilder.Entity<Discrepancy>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("discrepancies");

                entity.Property(e => e.InventarizationId).HasColumnName("inventarization_id");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.StatusId).HasColumnName("status_id");

                entity.Property(e => e.TypeId).HasColumnName("type_id");

                entity.HasOne(d => d.Inventarization)
                    .WithMany()
                    .HasForeignKey(d => d.InventarizationId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("discrepancies_inventarization_id_fkey");

                entity.HasOne(d => d.Product)
                    .WithMany()
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("discrepancies_product_id_fkey");

                entity.HasOne(d => d.Status)
                    .WithMany()
                    .HasForeignKey(d => d.StatusId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("discrepancies_status_id_fkey");

                entity.HasOne(d => d.Type)
                    .WithMany()
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("discrepancies_type_id_fkey");
            });

            modelBuilder.Entity<DiscrepancyStatus>(entity =>
            {
                entity.ToTable("discrepancy_statuses");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<DiscrepancyType>(entity =>
            {
                entity.ToTable("discrepancy_types");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Inventarization>(entity =>
            {
                entity.ToTable("inventarizations");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.EndDate).HasColumnName("end_date");

                entity.Property(e => e.ReasonId).HasColumnName("reason_id");

                entity.Property(e => e.StartDate).HasColumnName("start_date");

                entity.Property(e => e.WarehouseId).HasColumnName("warehouse_id");

                entity.HasOne(d => d.Reason)
                    .WithMany(p => p.Inventarizations)
                    .HasForeignKey(d => d.ReasonId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("inventarizations_reason_id_fkey");

                entity.HasOne(d => d.Warehouse)
                    .WithMany(p => p.Inventarizations)
                    .HasForeignKey(d => d.WarehouseId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("inventarizations_warehouse_id_fkey");
            });

            modelBuilder.Entity<InventarizationReason>(entity =>
            {
                entity.ToTable("inventarization_reasons");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<MeasurementUnit>(entity =>
            {
                entity.ToTable("measurement_units");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.ShortName)
                    .HasMaxLength(10)
                    .HasColumnName("short_name");
            });

            modelBuilder.Entity<Nomenclature>(entity =>
            {
                entity.ToTable("nomenclatures");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.TypeId).HasColumnName("type_id");

                entity.HasOne(d => d.Type)
                    .WithMany(p => p.Nomenclatures)
                    .HasForeignKey(d => d.TypeId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("nomenclature_types_fkey");
            });

            modelBuilder.Entity<NomenclatureProperty>(entity =>
            {
                entity.HasKey(e => new {e.NomenclatureId, e.PropertyId});

                entity.ToTable("nomenclature_properties");

                entity.Property(e => e.NomenclatureId).HasColumnName("nomenclature_id");

                entity.Property(e => e.PropertyId).HasColumnName("property_id");

                entity.Property(e => e.Value).HasColumnName("value");

                entity.HasOne(d => d.Nomenclature)
                    .WithMany()
                    .HasForeignKey(d => d.NomenclatureId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("nomenclature_properties_nomenclature_id_fkey");

                entity.HasOne(d => d.Property)
                    .WithMany()
                    .HasForeignKey(d => d.PropertyId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("nomenclature_properties_property_id_fkey");
            });

            modelBuilder.Entity<NomenclatureType>(entity =>
            {
                entity.ToTable("nomenclature_types");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("products");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CellAddress)
                    .HasMaxLength(20)
                    .HasColumnName("cell_address");

                entity.Property(e => e.DeliveryId).HasColumnName("delivery_id");

                entity.Property(e => e.NomenclatureId).HasColumnName("nomenclature_id");

                entity.Property(e => e.Status).HasColumnName("status");

                entity.Property(e => e.WarehouseId).HasColumnName("warehouse_id");

                entity.HasOne(d => d.Delivery)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.DeliveryId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("products_delivery_id_fkey");

                entity.HasOne(d => d.Nomenclature)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.NomenclatureId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("products_nomenclature_id_fkey");

                entity.HasOne(d => d.Warehouse)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.WarehouseId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("products_warehouse_id_fkey");
            });

            modelBuilder.Entity<ProductSelling>(entity =>
            {
                entity.HasKey(e => new { e.ProductId, e.SellingId });

                entity.ToTable("product_sellings");

                entity.Property(e => e.ProductId).HasColumnName("product_id");

                entity.Property(e => e.SellingId).HasColumnName("selling_id");

                entity.HasOne(d => d.Product)
                    .WithMany()
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("product_sellings_product_id_fkey");

                entity.HasOne(d => d.Selling)
                    .WithMany()
                    .HasForeignKey(d => d.SellingId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("product_sellings_selling_id_fkey");
            });

            modelBuilder.Entity<ProductWorth>(entity =>
            {
                entity.ToTable("product_worths");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Date)
                    .HasColumnName("date")
                    .HasDefaultValueSql("now()");

                entity.Property(e => e.NomenclatureId).HasColumnName("nomenclature_id");

                entity.Property(e => e.Worth).HasColumnName("worth");

                entity.HasOne(d => d.Nomenclature)
                    .WithMany(p => p.ProductWorths)
                    .HasForeignKey(d => d.NomenclatureId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("product_worths_nomenclature_id_fkey");
            });

            modelBuilder.Entity<Property>(entity =>
            {
                entity.ToTable("properties");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.MeasurementUnitId).HasColumnName("measurement_unit_id");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.HasOne(d => d.MeasurementUnit)
                    .WithMany(p => p.Properties)
                    .HasForeignKey(d => d.MeasurementUnitId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("properties_measurement_unit_id_fkey");
            });

            modelBuilder.Entity<Selling>(entity =>
            {
                entity.ToTable("sellings");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CustomerId).HasColumnName("customer_id");

                entity.Property(e => e.Date).HasColumnName("date");

                entity.Property(e => e.PersonalDiscount).HasColumnName("personal_discount");

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Sellings)
                    .HasForeignKey(d => d.CustomerId)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("sellings_customer_id_fkey");
            });

            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.ToTable("suppliers");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Email)
                    .HasMaxLength(50)
                    .HasColumnName("email");

                entity.Property(e => e.Name)
                    .HasMaxLength(50)
                    .HasColumnName("name");

                entity.Property(e => e.Phone)
                    .HasMaxLength(15)
                    .HasColumnName("phone");

                entity.Property(e => e.Tin).HasColumnName("tin");
            });

            modelBuilder.Entity<Warehouse>(entity =>
            {
                entity.ToTable("warehouses");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Address)
                    .HasMaxLength(50)
                    .HasColumnName("address");

                entity.Property(e => e.Phone)
                    .HasMaxLength(15)
                    .HasColumnName("phone");

                entity.Property(e => e.WarehouseType).HasColumnName("warehouse_type");

                entity.HasOne(d => d.WarehouseTypeNavigation)
                    .WithMany(p => p.Warehouses)
                    .HasForeignKey(d => d.WarehouseType)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("warehouses_warehouse_type_fkey");
            });

            modelBuilder.Entity<WarehouseType>(entity =>
            {
                entity.ToTable("warehouse_types");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .HasMaxLength(30)
                    .HasColumnName("name");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
