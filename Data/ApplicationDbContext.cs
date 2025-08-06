using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pg1.Models;

namespace Pg1.Data
{
    // Hereda de IdentityDbContext para que funcione Identity correctamente
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSets para tus entidades
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<DetallePedido> DetallesPedido { get; set; }
        public DbSet<Pago> Pagos { get; set; }
        public DbSet<Contacto> DbSetContactos { get; set; }
        public DbSet<Rating> DbSetRating { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder); // Obligatorio para Identity

            // Relaciones y configuraciones:

            // Producto - Categoria (uno a muchos)
            modelBuilder.Entity<Producto>()
                .HasOne(p => p.Categoria)
                .WithMany(c => c.Productos)
                .HasForeignKey(p => p.IdCategoria)
                .HasConstraintName("FK_Producto_Categoria")
                .OnDelete(DeleteBehavior.Restrict);

            // Pedido - Cliente (uno a muchos)
            modelBuilder.Entity<Pedido>()
                .HasOne(p => p.Cliente)
                .WithMany(c => c.Pedidos)
                .HasForeignKey(p => p.IdCliente)
                .HasConstraintName("FK_Pedido_Cliente")
                .OnDelete(DeleteBehavior.Restrict);

            // DetallePedido - Pedido (uno a muchos)
            modelBuilder.Entity<DetallePedido>()
                .HasOne(d => d.Pedido)
                .WithMany(p => p.Detalles)
                .HasForeignKey(d => d.IdPedido)
                .HasConstraintName("FK_DetallePedido_Pedido")
                .OnDelete(DeleteBehavior.Cascade);

            // DetallePedido - Producto (muchos a uno)
            modelBuilder.Entity<DetallePedido>()
                .HasOne(d => d.Producto)
                .WithMany()
                .HasForeignKey(d => d.IdProducto)
                .HasConstraintName("FK_DetallePedido_Producto")
                .OnDelete(DeleteBehavior.Restrict);

            // Pago - Pedido (uno a muchos)
            modelBuilder.Entity<Pago>()
                .HasOne(p => p.Pedido)
                .WithMany(p => p.Pagos)
                .HasForeignKey(p => p.IdPedido)
                .HasConstraintName("FK_Pago_Pedido")
                .OnDelete(DeleteBehavior.Cascade);

            // Rating - Producto (muchos a uno)
            modelBuilder.Entity<Rating>()
                .HasOne(r => r.Product)
                .WithMany()
                .HasForeignKey("ProductId") // clave foránea implícita, asumiendo que en Rating usas Product navigation
                .HasConstraintName("FK_Rating_Producto")
                .OnDelete(DeleteBehavior.Cascade);

            // Configura la tabla explícitamente para Rating si lo deseas
            modelBuilder.Entity<Rating>().ToTable("t_rating");

            // Propiedades y restricciones de columnas

            modelBuilder.Entity<Producto>()
                .Property(p => p.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Producto>()
                .Property(p => p.Precio)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Categoria>()
                .Property(c => c.Nombre)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<Cliente>()
                .Property(c => c.Nombre)
                .IsRequired()
                .HasMaxLength(100);

            modelBuilder.Entity<Pedido>()
                .Property(p => p.Estado)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Pendiente");

            modelBuilder.Entity<Pedido>()
                .Property(p => p.Total)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<DetallePedido>()
                .Property(d => d.PrecioUnitario)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<DetallePedido>()
                .Property(d => d.Subtotal)
                .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Pedido>()
                .Property(p => p.FechaPedido)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        }
    }
}
